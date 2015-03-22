namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的用户。
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 用户标识。
        /// </summary>
        string Identity { get; set; }

        /// <summary>
        /// 用户名称。
        /// </summary>
        string UserName { get; set; }
    }
}