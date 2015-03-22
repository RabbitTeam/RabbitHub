using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rabbit.Components.Command
{
    /// <summary>
    /// 命令上下文。
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// 文本读取器。
        /// </summary>
        public TextReader Reader { get; set; }

        /// <summary>
        /// 文本写入器。
        /// </summary>
        public TextWriter Writer { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public string[] Args { get; set; }
    }

    /// <summary>
    /// 命令执行上下文。
    /// </summary>
    public sealed class CommandExecuteContext : CommandContext
    {
        /// <summary>
        /// 含命令名称的完全参数。
        /// </summary>
        public string[] FullArgs { get; set; }

        /// <summary>
        /// 执行的命令名称。
        /// </summary>
        public string CommandName { get; set; }
    }

    /// <summary>
    /// 命令上下文扩展方法。
    /// </summary>
    public static class CommandContextExtensions
    {
        /// <summary>
        /// 写入。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="obj">对象。</param>
        public static void Write(this CommandContext context, object obj)
        {
            if (context.Writer == null)
                return;
            context.Writer.Write(obj);
        }

        /// <summary>
        /// 写入。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="format">格式。</param>
        /// <param name="args">参数。</param>
        public static void Write(this CommandContext context, string format, params object[] args)
        {
            if (args == null || !args.Any())
            {
                context.Write(format as object);
                return;
            }

            if (context.Writer == null)
                return;
            context.Writer.Write(format, args);
        }

        /// <summary>
        /// 写入一行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="obj">对象。</param>
        public static void WriteLine(this CommandContext context, object obj)
        {
            if (context.Writer == null)
                return;
            context.Writer.WriteLine(obj);
        }

        /// <summary>
        /// 写入一行。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="format">格式。</param>
        /// <param name="args">参数。</param>
        public static void WriteLine(this CommandContext context, string format, params object[] args)
        {
            if (args == null || !args.Any())
            {
                context.WriteLine(format as object);
                return;
            }

            if (context.Writer == null)
                return;
            context.Writer.WriteLine(format, args);
        }

        /// <summary>
        /// 读取。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="def">读取失败的默认值。</param>
        /// <returns>读取的字符串。</returns>
        public static string Read(this CommandContext context, string def = null)
        {
            var reader = context.Reader;
            return reader == null ? def : reader.ReadLine() ?? def;
        }

        /// <summary>
        /// 如果当前命令执行上下文中的命令名称在 <paramref name="commandNames"/> 中存在则执行 <paramref name="action"/>。
        /// </summary>
        /// <param name="context">命令上下文。</param>
        /// <param name="action">执行的动作。</param>
        /// <param name="commandNames">命令名称。</param>
        public static void Is(this CommandExecuteContext context, Action action, params string[] commandNames)
        {
            if (action == null || !context.IsIn(commandNames))
                return;
            action();
        }

        /// <summary>
        /// 如果当前命令执行上下文中的命令名称在 <paramref name="commandNames"/> 中存在则执行 <paramref name="func"/>。
        /// </summary>
        /// <typeparam name="T">返回值类型。</typeparam>
        /// <param name="context">命令上下文。</param>
        /// <param name="func">执行的动作。</param>
        /// <param name="def">如果不属于 <paramref name="commandNames"/> 返回的默认值。</param>
        /// <param name="commandNames">命令名称。</param>
        /// <returns>返回值。</returns>
        public static T Is<T>(this CommandExecuteContext context, Func<T> func, T def, params string[] commandNames)
        {
            if (func == null || !context.IsIn(commandNames))
                return def;
            return func();
        }

        private static bool IsIn(this CommandExecuteContext context, IEnumerable<string> commandNames)
        {
            return commandNames.Any(i => string.Equals(i, context.CommandName, StringComparison.OrdinalIgnoreCase));
        }
    }
}