using System.Collections.Generic;

namespace Rabbit.Kernel.Environment.Configuration
{
    /// <summary>
    /// 一个抽象的外壳设置管理者。
    /// </summary>
    public interface IShellSettingsManager
    {
        /// <summary>
        /// 加载所有外壳设置。
        /// </summary>
        /// <returns>外壳设置集合。</returns>
        IEnumerable<ShellSettings> LoadSettings();

        /// <summary>
        /// 保存一个外壳设置。
        /// </summary>
        /// <param name="settings">外壳设置信息。</param>
        void SaveSettings(ShellSettings settings);

        /// <summary>
        /// 删除一个外壳设置。
        /// </summary>
        /// <param name="name">外壳名称。</param>
        void DeleteSettings(string name);
    }
}