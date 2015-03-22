using Autofac;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Tasks;
using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class DefaultShell : IShell
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ISweepGenerator _sweepGenerator;

        public ILogger Logger { get; set; }

        public DefaultShell(ILifetimeScope lifetimeScope, ISweepGenerator sweepGenerator)
        {
            _lifetimeScope = lifetimeScope;
            _sweepGenerator = sweepGenerator;

            Logger = NullLogger.Instance;
        }

        #region Implementation of IShell

        /// <summary>
        /// 激活外壳。
        /// </summary>
        public void Activate()
        {
            using (var scope = _lifetimeScope.CreateWorkContextScope())
            {
                foreach (var shellEventse in scope.Resolve<IEnumerable<IShellEvents>>())
                {
                    shellEventse.Activated();
                }
            }

            _sweepGenerator.Activate();
        }

        /// <summary>
        /// 终止外壳。
        /// </summary>
        public void Terminate()
        {
            SafelyTerminate(() =>
            {
                using (var scope = _lifetimeScope.CreateWorkContextScope())
                {
                    foreach (var shellEventse in scope.Resolve<IEnumerable<IShellEvents>>())
                    {
                        shellEventse.Terminating();
                    }
                }
            });

            SafelyTerminate(() => _sweepGenerator.Terminate());
        }

        #endregion Implementation of IShell

        #region Private Method

        private void SafelyTerminate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error(e, "终止外壳的同时发生意外的错误");
            }
        }

        #endregion Private Method
    }
}