using Autofac.Features.OwnedInstances;
using Rabbit.Kernel.Logging;
using Rabbit.Kernel.Tasks;
using System;
using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.Impl
{
    internal sealed class DefaultShell : IShell
    {
        private readonly Func<Owned<IEnumerable<IShellEvents>>> _eventsFactory;
        private readonly ISweepGenerator _sweepGenerator;

        public ILogger Logger { get; set; }

        public DefaultShell(Func<Owned<IEnumerable<IShellEvents>>> eventsFactory, ISweepGenerator sweepGenerator)
        {
            _eventsFactory = eventsFactory;
            _sweepGenerator = sweepGenerator;

            Logger = NullLogger.Instance;
        }

        #region Implementation of IShell

        /// <summary>
        /// 激活外壳。
        /// </summary>
        public void Activate()
        {
            using (var events = _eventsFactory())
            {
                foreach (var eventse in events.Value)
                {
                    eventse.Activated();
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
                using (var events = _eventsFactory())
                {
                    foreach (var eventse in events.Value)
                    {
                        SafelyTerminate(() => eventse.Terminating());
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