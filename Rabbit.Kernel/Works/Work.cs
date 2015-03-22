using System;

namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 工作实例代理。
    /// </summary>
    /// <typeparam name="T">实例类型。</typeparam>
    public sealed class Work<T> where T : class
    {
        #region Field

        private readonly Func<Work<T>, T> _resolve;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的工作实例代理。
        /// </summary>
        /// <param name="resolve">解析器。</param>
        public Work(Func<Work<T>, T> resolve)
        {
            _resolve = resolve;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 实例值。
        /// </summary>
        public T Value
        {
            get { return _resolve(this); }
        }

        #endregion Property
    }
}