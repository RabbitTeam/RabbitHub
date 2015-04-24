using System;
using System.Collections.Concurrent;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.ThemeAwareness.Impl
{
    internal sealed class ConfiguredEnginesCache : IConfiguredEnginesCache
    {
        private IViewEngine _bare;
        private readonly ConcurrentDictionary<string, IViewEngine> _shallow = new ConcurrentDictionary<string, IViewEngine>();
        private readonly ConcurrentDictionary<string, IViewEngine> _deep = new ConcurrentDictionary<string, IViewEngine>();

        public ConfiguredEnginesCache()
        {
            _shallow = new ConcurrentDictionary<string, IViewEngine>();
        }

        #region Implementation of IConfiguredEnginesCache

        public IViewEngine BindBareEngines(Func<IViewEngine> factory)
        {
            return _bare ?? (_bare = factory());
        }

        public IViewEngine BindShallowEngines(string themeName, Func<IViewEngine> factory)
        {
            return _shallow.GetOrAdd(themeName, key => factory());
        }

        public IViewEngine BindDeepEngines(string themeName, Func<IViewEngine> factory)
        {
            return _deep.GetOrAdd(themeName, key => factory());
        }

        #endregion Implementation of IConfiguredEnginesCache
    }
}