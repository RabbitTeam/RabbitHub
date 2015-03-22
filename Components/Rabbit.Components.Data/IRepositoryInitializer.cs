using Rabbit.Kernel;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 一个抽象的数据库初始化器。
    /// </summary>
    public interface IRepositoryInitializer : IDependency
    {
        /// <summary>
        /// 初始化动作。
        /// </summary>
        void Initialize();
    }
}