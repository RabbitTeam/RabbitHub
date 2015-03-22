namespace Rabbit.Kernel.Works
{
    /// <summary>
    /// 工作上下文属性。
    /// </summary>
    /// <typeparam name="T">属性类型。</typeparam>
    public sealed class WorkContextProperty<T>
    {
        /// <summary>
        /// 属性值。
        /// </summary>
        public T Value { get; set; }
    }
}