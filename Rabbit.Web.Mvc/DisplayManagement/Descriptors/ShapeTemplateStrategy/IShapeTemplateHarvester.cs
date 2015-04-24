using Rabbit.Kernel;
using System;
using System.Collections.Generic;

namespace Rabbit.Web.Mvc.DisplayManagement.Descriptors.ShapeTemplateStrategy
{
    /// <summary>
    /// 一个抽象的星座模板收集者。
    /// </summary>
    public interface IShapeTemplateHarvester : IDependency
    {
        /// <summary>
        /// 子路径。
        /// </summary>
        /// <returns>路径集合。</returns>
        IEnumerable<string> SubPaths();

        /// <summary>
        /// 收集形状。
        /// </summary>
        /// <param name="info">收集形状信息。</param>
        /// <returns>收集形状。</returns>
        IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info);
    }

    internal sealed class BasicShapeTemplateHarvester : IShapeTemplateHarvester
    {
        public IEnumerable<string> SubPaths()
        {
            return new[] { "Views", "Views/Items", "Views/Parts", "Views/Fields" };
        }

        public IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info)
        {
            var lastDash = info.FileName.LastIndexOf('-');
            var lastDot = info.FileName.LastIndexOf('.');
            if (lastDot <= 0 || lastDot < lastDash)
            {
                yield return new HarvestShapeHit
                {
                    ShapeType = Adjust(info.SubPath, info.FileName, null)
                };
            }
            else
            {
                var displayType = info.FileName.Substring(lastDot + 1);
                yield return new HarvestShapeHit
                {
                    ShapeType = Adjust(info.SubPath, info.FileName.Substring(0, lastDot), displayType),
                    DisplayType = displayType
                };
            }
        }

        private static string Adjust(string subPath, string fileName, string displayType)
        {
            var leader = string.Empty;
            if (subPath.StartsWith("Views/") && subPath != "Views/Items")
            {
                leader = subPath.Substring("Views/".Length) + "_";
            }

            var shapeType = leader + fileName.Replace("--", "__").Replace("-", "__").Replace('.', '_');

            if (string.IsNullOrEmpty(displayType))
            {
                return shapeType.ToLowerInvariant();
            }
            var firstBreakingSeparator = shapeType.IndexOf("__", StringComparison.Ordinal);
            if (firstBreakingSeparator <= 0)
            {
                return (shapeType + "_" + displayType).ToLowerInvariant();
            }

            return (shapeType.Substring(0, firstBreakingSeparator) + "_" + displayType + shapeType.Substring(firstBreakingSeparator)).ToLowerInvariant();
        }
    }

    /// <summary>
    /// 收集形状信息。
    /// </summary>
    public sealed class HarvestShapeInfo
    {
        /// <summary>
        /// 子路径。
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 模板虚拟路径。
        /// </summary>
        public string TemplateVirtualPath { get; set; }
    }

    /// <summary>
    /// 收集形状。
    /// </summary>
    public class HarvestShapeHit
    {
        /// <summary>
        /// 形状类型。
        /// </summary>
        public string ShapeType { get; set; }

        /// <summary>
        /// 显示类型。
        /// </summary>
        public string DisplayType { get; set; }
    }

    /// <summary>
    /// 一个抽象的星座模板视图引擎。
    /// </summary>
    public interface IShapeTemplateViewEngine : IDependency
    {
        /// <summary>
        /// 检测模板文件名称。
        /// </summary>
        /// <param name="fileNames">文件名称集合。</param>
        /// <returns>文件名称集合。</returns>
        IEnumerable<string> DetectTemplateFileNames(IEnumerable<string> fileNames);
    }
}