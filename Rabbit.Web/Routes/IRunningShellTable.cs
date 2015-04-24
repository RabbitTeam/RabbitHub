using Rabbit.Kernel.Environment.Configuration;
using System.Web;

namespace Rabbit.Web.Routes
{
    /// <summary>
    /// 一个抽象的正在运行中的外壳表。
    /// </summary>
    public interface IRunningShellTable
    {
        /// <summary>
        /// 添加。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        void Add(ShellSettings settings);

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        void Remove(ShellSettings settings);

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        void Update(ShellSettings settings);

        /// <summary>
        /// 匹配。
        /// </summary>
        /// <param name="httpContext">Http上下文。</param>
        /// <returns>外壳设置。</returns>
        ShellSettings Match(HttpContextBase httpContext);

        /// <summary>
        /// 匹配。
        /// </summary>
        /// <param name="host">主机名称。</param>
        /// <param name="appRelativeCurrentExecutionFilePath">应用程序相对当前执行文件路径。</param>
        /// <returns>外壳设置，</returns>
        ShellSettings Match(string host, string appRelativeCurrentExecutionFilePath);
    }
}