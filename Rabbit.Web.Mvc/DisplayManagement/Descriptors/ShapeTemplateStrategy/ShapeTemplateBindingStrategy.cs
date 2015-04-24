using Rabbit.Kernel.Caching;
using Rabbit.Kernel.Environment.Descriptor.Models;
using Rabbit.Kernel.Extensions;
using Rabbit.Kernel.Extensions.Models;
using Rabbit.Kernel.FileSystems.VirtualPath;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Works;
using Rabbit.Web.Mvc.DisplayManagement.Implementation;
using Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness;
using Rabbit.Web.Mvc.Utility.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapeTemplateStrategy
{
    internal sealed class ShapeTemplateBindingStrategy : IShapeTableProvider
    {
        private readonly ShellDescriptor _shellDescriptor;
        private readonly IExtensionManager _extensionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IVirtualPathMonitor _virtualPathMonitor;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IEnumerable<IShapeTemplateHarvester> _harvesters;
        private readonly IEnumerable<IShapeTemplateViewEngine> _shapeTemplateViewEngines;
        private readonly IParallelCacheContext _parallelCacheContext;
        private readonly Work<ILayoutAwareViewEngine> _viewEngine;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ShapeTemplateBindingStrategy(
            IEnumerable<IShapeTemplateHarvester> harvesters,
            ShellDescriptor shellDescriptor,
            IExtensionManager extensionManager,
            ICacheManager cacheManager,
            IVirtualPathMonitor virtualPathMonitor,
            IVirtualPathProvider virtualPathProvider,
            IEnumerable<IShapeTemplateViewEngine> shapeTemplateViewEngines,
            IParallelCacheContext parallelCacheContext,
            Work<ILayoutAwareViewEngine> viewEngine,
            IWorkContextAccessor workContextAccessor)
        {
            _harvesters = harvesters;
            _shellDescriptor = shellDescriptor;
            _extensionManager = extensionManager;
            _cacheManager = cacheManager;
            _virtualPathMonitor = virtualPathMonitor;
            _virtualPathProvider = virtualPathProvider;
            _shapeTemplateViewEngines = shapeTemplateViewEngines;
            _parallelCacheContext = parallelCacheContext;
            _viewEngine = viewEngine;
            _workContextAccessor = workContextAccessor;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool DisableMonitoring { get; set; }

        private static IEnumerable<ExtensionDescriptorEntry> Once(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            var once = new ConcurrentDictionary<string, object>();
            return featureDescriptors.Select(fd => fd.Extension).Where(ed => once.TryAdd(ed.Id, null)).ToList();
        }

        public void Discover(ShapeTableBuilder builder)
        {
            Logger.Information("开始发现形状");

            var harvesterInfos = _harvesters.Select(harvester => new { harvester, subPaths = harvester.SubPaths() });

            var availableFeatures = _extensionManager.AvailableFeatures();
            var activeFeatures = availableFeatures.Where(FeatureIsEnabled);
            var activeExtensions = Once(activeFeatures);

            var hits = _parallelCacheContext.RunInParallel(activeExtensions, extensionDescriptor =>
            {
                Logger.Information("开始发现候选视图文件名称");
                var pathContexts = harvesterInfos.SelectMany(harvesterInfo => harvesterInfo.subPaths.Select(subPath =>
                {
                    var basePath = Path.Combine(extensionDescriptor.Location, extensionDescriptor.Id).Replace(Path.DirectorySeparatorChar, '/');
                    var virtualPath = Path.Combine(basePath, subPath).Replace(Path.DirectorySeparatorChar, '/');
                    var fileNames = _cacheManager.Get(virtualPath, ctx =>
                    {
                        if (!_virtualPathProvider.DirectoryExists(virtualPath))
                            return new string[0];

                        if (!DisableMonitoring)
                        {
                            Logger.Debug("监控虚拟路径 \"{0}\"", virtualPath);
                            ctx.Monitor(_virtualPathMonitor.WhenPathChanges(virtualPath));
                        }

                        return _virtualPathProvider.ListFiles(virtualPath).Select(Path.GetFileName).ToArray();
                    });
                    return new { harvesterInfo.harvester, basePath, subPath, virtualPath, fileNames };
                })).ToList();
                Logger.Information("发现候选视图文件名称完成");

                var fileContexts = pathContexts.SelectMany(pathContext => _shapeTemplateViewEngines.SelectMany(ve =>
                {
                    var fileNames = ve.DetectTemplateFileNames(pathContext.fileNames);
                    return fileNames.Select(
                        fileName => new
                        {
                            fileName = Path.GetFileNameWithoutExtension(fileName),
                            fileVirtualPath = Path.Combine(pathContext.virtualPath, fileName).Replace(Path.DirectorySeparatorChar, '/'),
                            pathContext
                        });
                }));

                var shapeContexts = fileContexts.SelectMany(fileContext =>
                {
                    var harvestShapeInfo = new HarvestShapeInfo
                    {
                        SubPath = fileContext.pathContext.subPath,
                        FileName = fileContext.fileName,
                        TemplateVirtualPath = fileContext.fileVirtualPath
                    };
                    var harvestShapeHits = fileContext.pathContext.harvester.HarvestShape(harvestShapeInfo);
                    return harvestShapeHits.Select(harvestShapeHit => new { harvestShapeInfo, harvestShapeHit, fileContext });
                });

                return shapeContexts.Select(shapeContext => new { extensionDescriptor, shapeContext }).ToList();
            }).SelectMany(hits2 => hits2);

            foreach (var iter in hits)
            {
                //模板总是与模块或主题的同名特征相关联
                var hit = iter;
                var featureDescriptors = iter.extensionDescriptor.Descriptor.Features.Where(fd => fd.Id == hit.extensionDescriptor.Id);
                foreach (var featureDescriptor in featureDescriptors)
                {
                    Logger.Debug("为特性 {2} 绑定 {0} 到形状 [{1}]",
                        hit.shapeContext.harvestShapeInfo.TemplateVirtualPath,
                        iter.shapeContext.harvestShapeHit.ShapeType,
                        featureDescriptor.Id);

                    builder.Describe(iter.shapeContext.harvestShapeHit.ShapeType)
                        .From(new Feature { Descriptor = featureDescriptor })
                        .BoundAs(
                            hit.shapeContext.harvestShapeInfo.TemplateVirtualPath,
                            shapeDescriptor => displayContext => Render(displayContext, hit.shapeContext.harvestShapeInfo));
                }
            }

            Logger.Information("形状发现完成。");
        }

        private bool FeatureIsEnabled(FeatureDescriptor fd)
        {
            return (DefaultExtensionTypes.IsTheme(fd.Extension.ExtensionType) && (fd.Id == "TheAdmin" || fd.Id == "SafeMode")) ||
                _shellDescriptor.Features.Any(sf => sf.Name == fd.Id);
        }

        private IHtmlString Render(DisplayContext displayContext, HarvestShapeInfo harvestShapeInfo)
        {
            Logger.Information("渲染模板文件 '{0}'", harvestShapeInfo.TemplateVirtualPath);
            IHtmlString result;

            if (displayContext.ViewContext.View != null)
            {
                var htmlHelper = new HtmlHelper(displayContext.ViewContext, displayContext.ViewDataContainer);
                result = htmlHelper.Partial(harvestShapeInfo.TemplateVirtualPath, displayContext.Value);
            }
            else
            {
                //如果视图为空，则表示该造型是从一个非视图产地/在没有的ViewContext成立了由视图引擎，但是手动执行。
                //手动创建的ViewContext工程与形状的方法工作时，而不是当形状被实现为一个Razor视图模板。
                //可怕的，但它会做现在。
                result = RenderRazorViewToString(harvestShapeInfo.TemplateVirtualPath, displayContext);
            }

            Logger.Information("完成目标文件 '{0}' 的渲染。", harvestShapeInfo.TemplateVirtualPath);
            return result;
        }

        private IHtmlString RenderRazorViewToString(string path, DisplayContext context)
        {
            using (var sw = new StringWriter())
            {
                var controllerContext = CreateControllerContext();
                var viewResult = _viewEngine.Value.FindPartialView(controllerContext, path, false);

                context.ViewContext.ViewData = new ViewDataDictionary(context.Value);
                context.ViewContext.TempData = new TempDataDictionary();
                viewResult.View.Render(context.ViewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return new HtmlString(sw.GetStringBuilder().ToString());
            }
        }

        private ControllerContext CreateControllerContext()
        {
            var controller = new StubController();
            var httpContext = _workContextAccessor.GetContext().Resolve<HttpContextBase>();
            var requestContext = _workContextAccessor.GetContext().Resolve<RequestContext>();
            var routeData = requestContext.RouteData;

            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", string.Empty));

            controller.ControllerContext = new ControllerContext(httpContext, routeData, controller)
            {
                RequestContext = requestContext
            };
            return controller.ControllerContext;
        }

        private class StubController : Controller { }
    }
}