using System;

namespace Rabbit.Kernel.Caching
{
    /// <summary>
    /// 弱引用类型持有者。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    public sealed class Weak<T>
    {
        private readonly WeakReference _target;

        /// <summary>
        /// 初始化一个弱引用类型持有者。
        /// </summary>
        /// <param name="target">对象引用的对象（目标）。</param>
        public Weak(T target)
        {
            _target = new WeakReference(target);
        }

        /// <summary>
        /// 初始化一个弱引用类型持有者。
        /// </summary>
        /// <param name="target">对象引用的对象（目标）。</param>
        /// <param name="trackResurrection">指示何时停止跟踪对象。 如果为 true，则在终结后跟踪对象；如果为 false，则仅在终结前跟踪对象。</param>
        public Weak(T target, bool trackResurrection)
        {
            _target = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        /// 获取或设置对象引用的对象（目标）。
        /// </summary>
        public T Target
        {
            get { return (T)_target.Target; }
            set { _target.Target = value; }
        }
    }
}