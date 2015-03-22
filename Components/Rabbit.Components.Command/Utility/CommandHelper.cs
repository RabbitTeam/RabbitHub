using CommandLine.Text;
using Rabbit.Components.Command.Services;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Components.Command.Utility
{
    /// <summary>
    /// 命令助手。
    /// </summary>
    public static class CommandHelper
    {
        internal static HelpText GetHelpText()
        {
            return new HelpText(new ChineseSentenceBuilder());
        }

        /// <summary>
        /// 需要转义的字符字典表。
        /// </summary>
        public static readonly IDictionary<string, string> EscapeDictionary = new Dictionary<string, string>
        {
            {":","\\$"},
            {" ","\\@"}
        };

        /// <summary>
        /// 将文本转义。
        /// </summary>
        /// <param name="text">需要专业的文本。</param>
        /// <returns>转义之后的文本。</returns>
        public static string Escape(this string text)
        {
            if (text == null || string.IsNullOrWhiteSpace(text))
                return text;

            return EscapeDictionary.Aggregate(text, (current, item) => current.Replace(item.Value, item.Key));
        }

        /// <summary>
        /// 获取命令名称。
        /// </summary>
        /// <param name="action">命令动作。</param>
        /// <param name="name">命令名称。</param>
        /// <returns>命令名称。</returns>
        public static string GetCommandName(CommandAction action, string name)
        {
            return (action == CommandAction.None ? name : string.Format("{0}-{1}", action, name)).Trim();
        }
    }
}