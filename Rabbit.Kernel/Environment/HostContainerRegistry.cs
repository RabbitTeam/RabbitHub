using Rabbit.Kernel.Caching;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment
{
    /// <summary>
    /// 主机容器注册处。
    /// </summary>
    public static class HostContainerRegistry
    {
        #region Field

        private static readonly IList<Weak<IShim>> Shims = new List<Weak<IShim>>();
        private static IHostContainer _hostContainer;
        private static readonly object SyncLock = new object();

        #endregion Field

        /// <summary>
        /// 注册垫片。
        /// </summary>
        /// <param name="shim">垫片实例。</param>
        public static void RegisterShim(IShim shim)
        {
            lock (SyncLock)
            {
                CleanupShims();

                Shims.Add(new Weak<IShim>(shim));
                shim.HostContainer = _hostContainer;
            }
        }

        /// <summary>
        /// 注册主机容器。
        /// </summary>
        /// <param name="container">主机容器。</param>
        public static void RegisterHostContainer(IHostContainer container)
        {
            lock (SyncLock)
            {
                CleanupShims();

                _hostContainer = container;
                RegisterContainerInShims();
            }
        }

        #region Private Method

        private static void RegisterContainerInShims()
        {
            foreach (var target in Shims.Select(shim => shim.Target).Where(target => target != null))
            {
                target.HostContainer = _hostContainer;
            }
        }

        private static void CleanupShims()
        {
            for (var i = Shims.Count - 1; i >= 0; i--)
            {
                if (Shims[i].Target == null)
                    Shims.RemoveAt(i);
            }
        }

        #endregion Private Method
    }
}