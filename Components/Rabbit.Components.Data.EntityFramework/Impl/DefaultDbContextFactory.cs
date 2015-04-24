using Rabbit.Components.Data.EntityFramework.Providers;
using Rabbit.Components.Data.Models;
using Rabbit.Components.Data.Utility.Extensions;
using Rabbit.Kernel;
using Rabbit.Kernel.Environment.Configuration;
using Rabbit.Kernel.Environment.ShellBuilders;
using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Rabbit.Components.Data.EntityFramework.Impl
{
    internal interface IDbContextFactoryEventHandler
    {
        void Created();
    }

    internal class AutomaticMigrationsDbContextFactoryEventHandler : IDbContextFactoryEventHandler
    {
        private readonly Lazy<IEnumerable<IRepositoryInitializer>> _repositoryInitializers;
        private static readonly object SyncLock = new object();
        private static bool _isRepositoryInitializer;

        public AutomaticMigrationsDbContextFactoryEventHandler(Lazy<IEnumerable<IRepositoryInitializer>> repositoryInitializers)
        {
            _repositoryInitializers = repositoryInitializers;
        }

        #region Implementation of IDbContextFactoryEventHandler

        public void Created()
        {
            if (_isRepositoryInitializer)
                return;
            lock (SyncLock)
            {
                if (_isRepositoryInitializer)
                    return;
                _isRepositoryInitializer = true;

                foreach (var repositoryInitializer in _repositoryInitializers.Value)
                {
                    repositoryInitializer.Initialize();
                }
            }
        }

        #endregion Implementation of IDbContextFactoryEventHandler
    }

    internal sealed class DefaultDbContextFactory : Component, IDbContextFactory, ITransactionManager, IDisposable
    {
        #region Field

        private DefaultDbContext _dbContext;
        private DbContextTransaction _transaction;
        private bool _cancelled;
        private readonly EntityFrameworkDbContextFactory _entityFrameworkDbContextFactory;
        private readonly Lazy<IEnumerable<IDbContextFactoryEventHandler>> _dbContextFactoryEventHandlers;

        #endregion Field

        #region Constructor

        public DefaultDbContextFactory(EntityFrameworkDbContextFactory entityFrameworkDbContextFactory, Lazy<IEnumerable<IDbContextFactoryEventHandler>> dbContextFactoryEventHandlers)
        {
            _entityFrameworkDbContextFactory = entityFrameworkDbContextFactory;
            _dbContextFactoryEventHandlers = dbContextFactoryEventHandlers;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Implementation of IDbContextFactory

        public DbContext CreateDbContext()
        {
            Logger.Debug("获取DbContext");
            Demand();

            foreach (var dbContextFactoryEventHandler in _dbContextFactoryEventHandlers.Value)
            {
                dbContextFactoryEventHandler.Created();
            }

            return _dbContext;
        }

        #endregion Implementation of IDbContextFactory

        #region Private Method

        private void EnsureSession()
        {
            if (_dbContext != null)
                return;

            Logger.Information("创建DbContext");

            _dbContext = _entityFrameworkDbContextFactory.Create();
        }

        #endregion Private Method

        #region Implementation of ITransactionManager

        /// <summary>
        /// 索取一个事务。
        /// </summary>
        public void Demand()
        {
            EnsureSession();

            if (_transaction != null)
                return;

            Logger.Debug("创建事务请求");
            _transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 索取一个锁行为为 <see cref="IsolationLevel.ReadCommitted"/> 的新事务。
        /// </summary>
        public void RequireNew()
        {
            RequireNew(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 索取一个锁行为为 <paramref name="level"/> 的新事务。
        /// </summary>
        /// <param name="level">事务锁定行为。</param>
        public void RequireNew(IsolationLevel level)
        {
            EnsureSession();

            if (_transaction != null)
            {
                if (_cancelled)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
                else
                {
                    _dbContext.SaveChanges();
                    _transaction.Commit();
                }
            }
            else
            {
                _dbContext.SaveChanges();
            }

            Logger.Debug("创建新的事务，隔离级别 {0}", level);
            _transaction = _dbContext.Database.BeginTransaction(level);
        }

        /// <summary>
        /// 取消事务。
        /// </summary>
        public void Cancel()
        {
            Logger.Debug("标记事务取消");
            _cancelled = true;
        }

        #endregion Implementation of ITransactionManager

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (!_cancelled)
                {
                    Logger.Debug("标记事务为完成");
                    if (_dbContext != null)
                        _dbContext.SaveChanges();
                    if (_transaction != null)
                        _transaction.Commit();
                }
                else
                {
                    if (_transaction == null)
                        return;
                    Logger.Debug("回滚事务");
                    _transaction.Rollback();
                }
            }
            catch (DbEntityValidationException dbEntityValidationException)
            {
                var builder = new StringBuilder();
                builder.AppendLine("处置事务时发生了错误");
                foreach (var dbEntityValidationResult in dbEntityValidationException.EntityValidationErrors)
                {
                    foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                    {
                        var property = dbValidationError.PropertyName;
                        var currentValues = dbEntityValidationResult.Entry.CurrentValues;
                        var propertyValue = currentValues.PropertyNames.Contains(property) ? currentValues[property] : null;
                        builder.AppendLine(string.Format("属性：{0}，值：{1}，错误信息：{2}。", property, propertyValue == null ? "null" : propertyValue.ToString(), dbValidationError.ErrorMessage));
                    }
                }
                Logger.Error(dbEntityValidationException, builder.ToString());
#if DEBUG
                throw;
#endif
            }
            catch (Exception e)
            {
                Logger.Error(e, "处置事务时发生了错误");
#if DEBUG
                throw;
#endif
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    Logger.Debug("事务被释放");
                    _transaction = null;
                }
                _cancelled = false;
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                }
            }
        }

        #endregion Implementation of IDisposable
    }

    internal sealed class EntityFrameworkDbContextFactory
    {
        private readonly IServiceTypeHarvester _serviceTypeHarvester;
        private readonly IEnumerable<IEntityFrameworkDataServicesProvider> _dataServicesProviders;
        private readonly IEnumerable<IMapping> _mappings;
        private readonly IEnumerable<RecordBlueprint> _recordBlueprints;
        private readonly ShellSettings _shellSettings;

        #region Field

        private static readonly ConcurrentDictionary<string, DbCompiledModel> DbCompiledModelCacheDictionary = new ConcurrentDictionary<string, DbCompiledModel>();

        #endregion Field

        #region Property

        public ILogger Logger { get; set; }

        #endregion Property

        #region Constructor

        public EntityFrameworkDbContextFactory(IServiceTypeHarvester serviceTypeHarvester, IEnumerable<IEntityFrameworkDataServicesProvider> dataServicesProviders, IEnumerable<IMapping> mappings, ShellBlueprint shellBlueprint)
        {
            _serviceTypeHarvester = serviceTypeHarvester;
            _dataServicesProviders = dataServicesProviders;
            _mappings = mappings;
            _recordBlueprints = shellBlueprint.GetRecords();
            _shellSettings = shellBlueprint.Settings;

            Logger = NullLogger.Instance;
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// 创建派生 <see cref="T:System.Data.Entity.DbContext"/> 类型的新实例。
        /// </summary>
        /// <returns>TContext 的一个实例</returns>
        public DefaultDbContext Create()
        {
            Logger.Information("创建DbContext");

            var dbCompiledModel = DbCompiledModelCacheDictionary.GetOrAdd(_shellSettings.Name, GetDbCompiledModel(_shellSettings));
            return new DefaultDbContext(_shellSettings.GetDataConnectionString(), dbCompiledModel);
        }

        #endregion Public Method

        #region Private Method

        private DbCompiledModel GetDbCompiledModel(ShellSettings shellSettings)
        {
            if (DbCompiledModelCacheDictionary.ContainsKey(shellSettings.Name))
                return DbCompiledModelCacheDictionary[shellSettings.Name];

            var records = _recordBlueprints.ToArray();
            var modelBuilder = new DbModelBuilder();

            //设置数据库架构名称。
            //            modelBuilder.HasDefaultSchema(shellSettings.Name);

            //将记录类型添加至DbContext。
            foreach (var recordBlueprint in records)
                Entity(recordBlueprint.Type, modelBuilder);

            //默认配置信息。
            modelBuilder.Types().Configure(config =>
            {
                var record = records.FirstOrDefault(i => i.Type == config.ClrType);
                if (record == null)
                    return;
                config.ToTable(record.TableName);
            });

            //应用约定。
            modelBuilder.Conventions.Add(GetConventions());

            var dataServiceProvider = GetDataServicesProvider(shellSettings);
            var connectionString = shellSettings.GetDataConnectionString();

            string providerManifestToken;

            using (var connection = dataServiceProvider.CreateConnection(connectionString))
                providerManifestToken = dataServiceProvider.Instance.GetProviderManifestToken(connection);

            var dbProviderInfo = new DbProviderInfo(dataServiceProvider.ProviderInvariantName, providerManifestToken);
            var dbModel = modelBuilder.Build(dbProviderInfo);

            return dbModel.Compile();
        }

        private IConvention[] GetConventions()
        {
            var conventions = _serviceTypeHarvester.GetTypes(typeof(EntityFrameworkDbContextFactory).Assembly).Where(t => typeof(IConvention).IsAssignableFrom(t));
            var list = new List<IConvention>();
            foreach (var convention in conventions)
            {
                var c = convention.GetConstructor(new Type[0]);
                if (c == null)
                    continue;
                var item = c.Invoke(new object[0]) as IConvention;
                if (item == null)
                    continue;
                list.Add(item);
            }

            return list.ToArray();
        }

        private void Entity(Type type, DbModelBuilder modelBuilder)
        {
            const string methodName = "Entity";
            var method = typeof(DbModelBuilder).GetMethod(methodName);
            var result = method.MakeGenericMethod(type).Invoke(modelBuilder, new object[0]);
            var mapping = _mappings.FirstOrDefault(m => m.Type == type);
            if (mapping != null)
                mapping.Mapping(result);
        }

        private IEntityFrameworkDataServicesProvider GetDataServicesProvider(ShellSettings shellSettings)
        {
            var providerName = shellSettings.GetDataProvider();

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("因为数据服务提供名称为空，所以无法确定数据服务提供者。");

            var provider = _dataServicesProviders.FirstOrDefault(
                p => string.Equals(p.ProviderName, providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
                throw new NotSupportedException(string.Format("找不到名称为：{0}的数据服务提供程序。", providerName));

            return provider;
        }

        #endregion Private Method
    }
}