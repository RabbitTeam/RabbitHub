using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Rabbit.Kernel.Bus;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Rabbit.Components.Bus.SignalR
{
    internal sealed class SignalRBus : Kernel.Bus.Bus
    {
        #region Field

        private static readonly string HostUrl = BusBuilderExtensions.HostUrl;
        private static readonly string Path = BusBuilderExtensions.Path;

        private readonly IMessageDispatcher _messageDispatcher;
        private static Connection _connection;
        private static readonly object SyncLock = new object();

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的Bug。
        /// </summary>
        /// <param name="messageDispatcher">消息调度员。</param><exception cref="T:System.ArgumentNullException"><paramref name="messageDispatcher"/> 为 null。</exception>
        public SignalRBus(IMessageDispatcher messageDispatcher)
            : base(messageDispatcher)
        {
            _messageDispatcher = messageDispatcher;
            Init();
        }

        #endregion Constructor

        #region Overrides of Bus

        /// <summary>
        /// 发布一个消息到总线。
        /// </summary>
        /// <typeparam name="TMessage">消息类型。</typeparam>
        /// <param name="message">需要发布的消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> 为null。</exception>
        protected override void Publish<TMessage>(TMessage message)
        {
            if (message == null)
                return;
            //异步打开连接。
            var openConnectionTask = Task.Factory.StartNew(OpenConnection);
            base.Publish(message);

            var bytes = Serialize(message);
            if (bytes.Length <= 0)
                return;

            var m = new SignalRMessage(bytes);

            //等待连接打开完成。
            openConnectionTask.Wait();
            _connection.Send(m).Wait();
        }

        #endregion Overrides of Bus

        #region Private Method

        private void Init()
        {
            //已经初始化完成则不再进行初始化。
            if (_connection != null)
            {
                OpenConnection();
                return;
            }

            lock (SyncLock)
            {
                try
                {
                    //已经初始化完成则不再进行初始化。
                    if (_connection != null)
                    {
                        OpenConnection();
                        return;
                    }

                    var connection = _connection = GetConnection();
                    connection.Closed += OpenConnection;

                    connection.Received += data =>
                    {
                        var message = DeserializeSignalRMessage(data);
                        if (message == null)
                            return;
                        var m = Deserialize(message.Message);
                        if (m == null)
                            return;
                        _messageDispatcher.DispatchMessage(m);
                    };
                }
                catch
                {
                    if (_connection != null)
                    {
                        _connection.Stop();
                        _connection = null;
                    }
                    throw;
                }
            }

            OpenConnection();
        }

        private static void OpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Disconnected)
                return;

            lock (SyncLock)
            {
                if (_connection == null || _connection.State != ConnectionState.Disconnected)
                    return;

                _connection.Start().Wait();
            }
        }

        private static Connection GetConnection()
        {
            return new Connection(GetConnectionString());
            //得到授权Cookie。
            Cookie returnCookie;
            if (!AuthenticateUser(out returnCookie))
                throw new Exception("Bus安全验证失败。");

            //创建连接。
            var connection = new Connection(GetConnectionString()) { CookieContainer = new CookieContainer() };
            //添加授权Cookie。
            connection.CookieContainer.Add(returnCookie);

            return connection;
        }

        /// <summary>
        /// 授权用户。
        /// </summary>
        /// <param name="authCookie">授权后的Cooike。</param>
        /// <returns>是否授权成功。</returns>
        private static bool AuthenticateUser(out Cookie authCookie)
        {
            authCookie = null;

            var request = WebRequest.Create(GetAuthenticateUrl()) as HttpWebRequest;

            if (request == null)
                return false;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();

            const string authCredentials = "ticket=D155423D61E24D1FA697D3B595E85A19";
            var bytes = Encoding.UTF8.GetBytes(authCredentials);
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(bytes, 0, bytes.Length);

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response == null)
                    return false;

                authCookie = response.Cookies[FormsAuthentication.FormsCookieName];
            }

            return authCookie != null;
        }

        /// <summary>
        /// 获取SignalR连接字符串。
        /// </summary>
        /// <returns>连接字符串。</returns>
        private static string GetConnectionString()
        {
            var url = HostUrl;
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("找不到主机Url，SignalRHostUrl。");
            return url + Path;
        }

        /// <summary>
        /// 获取授权地址。
        /// </summary>
        /// <returns>授权地址。</returns>
        private static string GetAuthenticateUrl()
        {
            var uri = new Uri(GetConnectionString());
            return GetUrlPrefix(uri) + "/Account/SignIn";
        }

        private static string GetUrlPrefix(Uri uri)
        {
            var builder = new StringBuilder();

            builder.Append(uri.Scheme);
            builder.Append("://");
            builder.Append(uri.Host);

            if (uri.IsDefaultPort)
                return builder.ToString();

            builder.Append(":");
            builder.Append(uri.Port);

            return builder.ToString();
        }

        private static SignalRMessage DeserializeSignalRMessage(string data)
        {
            return JsonConvert.DeserializeObject<SignalRMessage>(data);
        }

        private static object Deserialize(byte[] bytes)
        {
            object message = null;
            using (var stream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter();
                try
                {
                    message = binaryFormatter.Deserialize(stream);
                    return message;
                }
                catch
                {
                    return message;
                }
            }
        }

        private static byte[] Serialize(object message)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, message);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        #endregion Private Method

        #region Help Class

        public sealed class SignalRMessage
        {
            public SignalRMessage(byte[] message)
            {
                Message = message;
            }

            public byte[] Message { get; set; }
        }

        #endregion Help Class
    }
}