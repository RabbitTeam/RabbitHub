using Rabbit.Components.Command.Services;
using Rabbit.Kernel.Bus;
using Rabbit.Kernel.Environment;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rabbit.Components.Command
{
    internal sealed class ShellEvents : IShellEvents
    {
        private readonly ICommandHost _commandHost;
        private readonly IBus _bus;

        private const string CommandPrefix = "Rabbit.Components.Command";

        private static bool _isInit;
        private static readonly object SyncLock = new object();

        public ShellEvents(IBus bus, ICommandHost commandHost)
        {
            _bus = bus;
            _commandHost = commandHost;
        }

        #region Implementation of IShellEvents

        /// <summary>
        /// 激活外壳完成后执行。
        /// </summary>
        public void Activated()
        {
            if (_isInit)
                return;

            lock (SyncLock)
            {
                if (_isInit)
                    return;
                _isInit = true;
                if (_bus == null || _commandHost == null)
                    return;

                var localApplicationIdentity = ConfigurationManager.AppSettings["ApplicationIdentity"];
                if (string.IsNullOrWhiteSpace(localApplicationIdentity))
                    throw new ArgumentException("请配置应用程序标识 'ApplicationIdentity'。");

                var bus = _bus;

                bus.Subscribe<CommandMessage>((message, c) =>
                {
                    var parameters = SplitParameters(message.Parameters);
                    if (!parameters.HasValue)
                        return;

                    var parameter = parameters.Value;

                    var serviceIdentity = parameter.Key.Item1;
                    var clientIdentity = parameter.Key.Item2;

                    //接收命令消息并处理。
                    if (!message.CommandName.StartsWith(CommandPrefix) || !localApplicationIdentity.Equals(serviceIdentity, StringComparison.OrdinalIgnoreCase))
                        return;

                    var args = parameter.Value;

                    //处理命令。
                    var result = HandCommand(bus, clientIdentity, args);

                    c.Post(r => r.Result = result);
                });
            }
        }

        /// <summary>
        /// 终止外壳前候执行。
        /// </summary>
        public void Terminating()
        {
        }

        #endregion Implementation of IShellEvents

        #region Private Method

        /// <summary>
        /// 处理命令。
        /// </summary>
        /// <param name="bus">总线。</param>
        /// <param name="clientIdentity">客户端标识。</param>
        /// <param name="args">参数。</param>
        private bool HandCommand(IBus bus, string clientIdentity, string[] args)
        {
            //创建一个工作上下文范围。
            /*            using (var workScope = _workContextScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                        {*/
            var commandHost = _commandHost;

            var context = new CommandContext
            {
                Args = args,
                Writer = new BusTextWriter(bus, clientIdentity)
            };
            context.Reader = new BusTextReader(bus, context, clientIdentity);

            return commandHost.Execute(context);
            //            }
        }

        private static KeyValuePair<Tuple<string, string>, string[]>? SplitParameters(IEnumerable<object> parameters)
        {
            if (parameters == null)
                return null;

            var args = parameters.Select(i => i is string ? i as string : i == null ? null : i.ToString()).ToArray();

            if (args.Count() < 2)
                return null;

            return new KeyValuePair<Tuple<string, string>, string[]>(new Tuple<string, string>(args.First(), args.Skip(1).First()), args.Skip(2).ToArray());
        }

        #endregion Private Method

        #region Help Class

        private sealed class BusTextWriter : StringWriter
        {
            private readonly IBus _bus;
            private readonly string _clientIdentity;

            public BusTextWriter(IBus bus, string clientIdentity)
            {
                _bus = bus;
                _clientIdentity = clientIdentity;
            }

            #region Overrides of StringWriter

            /// <summary>
            /// 将一个字符写入到 StringWriter 的此实例中。
            /// </summary>
            /// <param name="value">要写入的字符。</param><exception cref="T:System.ObjectDisposedException">编写器已关闭。</exception>
            public override void Write(char value)
            {
                Write(value.ToString(CultureInfo.InvariantCulture));
            }

            /// <summary>
            /// 将字符数组的指定区域写入到 StringWriter 的此实例中。
            /// </summary>
            /// <param name="buffer">将从中读取数据的字符数组。</param><param name="index">从 <paramref name="buffer"/> 的索引处开始读取。</param><param name="count">要写入的最大字符数。</param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative.</exception><exception cref="T:System.ArgumentException">(<paramref name="index"/> + <paramref name="count"/>)&gt; <paramref name="buffer"/>.Length.</exception><exception cref="T:System.ObjectDisposedException">编写器已关闭。</exception>
            public override void Write(char[] buffer, int index, int count)
            {
                Write(new string(buffer.Skip(index).Take(count).ToArray()));
            }

            /// <summary>
            /// 将字符串写入到 StringWriter 的此实例中。
            /// </summary>
            /// <param name="value">要写入的字符串。</param><exception cref="T:System.ObjectDisposedException">编写器已关闭。</exception>
            public override void Write(string value)
            {
                _bus.Publish(new CommandMessage(CommandPrefix + ".Write", new object[] { _clientIdentity, value }));
            }

            #endregion Overrides of StringWriter
        }

        private sealed class BusTextReader : StringReader
        {
            private readonly IBus _bus;
            private readonly CommandContext _context;
            private readonly string _clientIdentity;

            public BusTextReader(IBus bus, CommandContext context, string clientIdentity)
                : this(string.Empty)
            {
                _bus = bus;
                _context = context;
                _clientIdentity = clientIdentity;
            }

            /// <summary>
            /// 初始化从指定字符串进行读取的 <see cref="T:System.IO.StringReader"/> 类的新实例。
            /// </summary>
            /// <param name="s">应将 <see cref="T:System.IO.StringReader"/> 初始化为的字符串。</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> 参数为 null。</exception>
            private BusTextReader(string s)
                : base(s)
            {
            }

            #region Overrides of StringReader

            /// <summary>
            /// 从基础字符串中读取一行。
            /// </summary>
            /// <returns>
            /// 基础字符串中的下一行；或者如果到达了基础字符串的末尾，则为 null。
            /// </returns>
            /// <exception cref="T:System.ObjectDisposedException">当前读取器已关闭。</exception><exception cref="T:System.OutOfMemoryException">内存不足，无法为返回的字符串分配缓冲区。</exception>
            public override string ReadLine()
            {
                try
                {
                    var result = _bus.PublishRequest<CommandMessage, string>(new CommandMessage(CommandPrefix + ".ReadLine", new object[] { _clientIdentity }),
                        c =>
                        {
                            c.Timeout(TimeSpan.FromMinutes(1));
                            c.Handle(exception =>
                            {
                                _context.WriteLine("等待命令超时，将以默认的命令执行。");
                                return true;
                            });
                        });
                    return result;
                }
                catch
                {
                    _context.WriteLine("等待命令时发生了错误，将以默认的命令执行。");
                    return null;
                }
            }

            #endregion Overrides of StringReader
        }

        #endregion Help Class
    }
}