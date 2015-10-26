using Rabbit.Components.DataBase.Providers;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Rabbit.Components.DataBase
{
    /// <summary>
    /// 一个抽象的数据连接工厂。
    /// </summary>
    public interface IDbConnectionFactory : IDependency
    {
        /// <summary>
        /// 请求一个数据连接。
        /// </summary>
        /// <returns>数据连接。</returns>
        IDbConnection DemandDbConnection();
    }

    internal sealed class DbConnectionFactory : IDbConnectionFactory
    {
        #region Field

        private readonly ShellSettings _shellSettings;
        private readonly IEnumerable<IDbConnectionProvider> _providers;
        private IDbConnection _dbConnection;

        #endregion Field

        #region Constructor

        public DbConnectionFactory(ShellSettings shellSettings, IEnumerable<IDbConnectionProvider> providers)
        {
            _shellSettings = shellSettings;
            _providers = providers;
        }

        #endregion Constructor

        #region Implementation of IDbConnectionFactory

        /// <summary>
        /// 请求一个数据连接。
        /// </summary>
        /// <returns>数据连接。</returns>
        public IDbConnection DemandDbConnection()
        {
            if (_dbConnection == null)
            {
                lock (this)
                {
                    if (_dbConnection == null)
                    {
                        var dataProvider = _shellSettings["DataProvider"];
                        var provider = _providers.SingleOrDefault(
                            i => string.Equals(i.Name, dataProvider, StringComparison.OrdinalIgnoreCase));
                        if (provider == null)
                            throw new NotSupportedException($"不支持的数据提供程序：{dataProvider}。");

                        return _dbConnection = provider.CreateDbConnection(GetParameters());
                    }
                }
            }
            return _dbConnection;
        }

        #endregion Implementation of IDbConnectionFactory

        #region Private Method

        private IDictionary<string, object> GetParameters()
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var key in _shellSettings.Keys)
            {
                dictionary[key] = _shellSettings[key];
            }
            return dictionary;
        }

        #endregion Private Method
    }
}