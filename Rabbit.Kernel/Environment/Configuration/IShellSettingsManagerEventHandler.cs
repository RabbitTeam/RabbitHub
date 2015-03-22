using Rabbit.Kernel.Events;

namespace Rabbit.Kernel.Environment.Configuration
{
    /// <summary>
    /// 一个抽象的外壳设置管理者事件处理程序。
    /// </summary>
    public interface IShellSettingsManagerEventHandler : IEventHandler
    {
        /// <summary>
        /// 外壳设置保存成功之后。
        /// </summary>
        /// <param name="settings">外壳设置信息。</param>
        void Saved(ShellSettings settings);
    }
}