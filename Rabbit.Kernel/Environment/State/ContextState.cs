using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace Rabbit.Kernel.Environment.State
{
    /// <summary>
    /// 保存一些状态在HttpContxt中或线程中。
    /// </summary>
    /// <typeparam name="T">需要存储的数据类型。</typeparam>
    public class ContextState<T> where T : class
    {
        #region Field

        private readonly string _name;
        private readonly Func<T> _defaultValue;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的上下文状态实例。
        /// </summary>
        /// <param name="name">上下文名称。</param>
        public ContextState(string name)
        {
            _name = name;
        }

        /// <summary>
        /// 初始化一个新的上下文状态实例。
        /// </summary>
        /// <param name="name">上下文名称。</param>
        /// <param name="defaultValue">默认值。</param>
        public ContextState(string name, Func<T> defaultValue)
        {
            _name = name;
            _defaultValue = defaultValue;
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// 获取一个状态数据。
        /// </summary>
        /// <returns>数据。</returns>
        public T GetState()
        {
            if (HttpContext.Current == null)
            {
                var data = CallContext.GetData(_name);

                if (data != null)
                    return data as T;
                if (_defaultValue == null)
                    return null;
                CallContext.SetData(_name, data = _defaultValue());
                return data as T;
            }

            if (HttpContext.Current.Items[_name] == null)
                HttpContext.Current.Items[_name] = _defaultValue == null ? null : _defaultValue();

            return HttpContext.Current.Items[_name] as T;
        }

        /// <summary>
        /// 设置一个状态值。
        /// </summary>
        /// <param name="state">状态值。</param>
        public void SetState(T state)
        {
            if (HttpContext.Current == null)
            {
                CallContext.SetData(_name, state);
            }
            else
            {
                HttpContext.Current.Items[_name] = state;
            }
        }

        #endregion Public Method
    }
}