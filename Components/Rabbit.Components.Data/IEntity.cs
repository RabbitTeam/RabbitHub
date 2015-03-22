namespace Rabbit.Components.Data
{
    /// <summary>
    /// 表示一个抽象的带有键的实体。
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 键值。
        /// </summary>
        long Id { get; set; }
    }
}