using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

#if !NET40

using System.Threading;
using System.Threading.Tasks;
using Rabbit.Kernel.Utility.Extensions;

#endif

namespace Rabbit.Components.Data.EntityFramework
{
    internal sealed class EntityFrameworkRepository<T> : Repository<T> where T : class
    {
        #region Field

        internal readonly DbContext DbContext;
        internal readonly DbSet<T> DbSet;

        #endregion Field

        #region Constructor

        public EntityFrameworkRepository(IDbContextFactory dbContextFactory)
        {
            DbContext = dbContextFactory.CreateDbContext();
            DbSet = DbContext.Set<T>();
        }

        #endregion Constructor

        #region Overrides of RepositoryBase<T>

        /// <summary>
        /// 表记录。
        /// </summary>
        public override IQueryable<T> Table
        {
            get { return DbSet; }
        }

        /// <summary>
        /// 创建。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DbSet.Add(entity);
        }

        /// <summary>
        /// 创建或更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void CreateOrUpdate(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DbSet.AddOrUpdate(entity);
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entry = DbContext.Entry(entity);

            if (DbSet.Local.Any(i => i == entity))
                return;

            entry.State = EntityState.Modified;
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entity">实体。</param>
        public override void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DbSet.Remove(entity);
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> 为 null。</exception>
        public override void Delete(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        /// <summary>
        /// 删除符合表达式的记录。
        /// </summary>
        /// <param name="predicate">表达式。</param>
        public override void Delete(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            DbSet.RemoveRange(Fetch(predicate));
        }

        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        public override void Flush()
        {
            DbContext.SaveChanges();
        }

#if !NET40
        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        /// <returns>表示异步任务保存操作，任务结果包含写入对象的数量。</returns>
        public override Task<int> FlushAsync()
        {
            return DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        /// <param name="cancellationToken">一个System.Threading.CancellationToken以等待任务完成观察。</param>
        /// <returns>表示异步任务保存操作，任务结果包含写入对象的数量。</returns>
        public override Task<int> FlushAsync(CancellationToken cancellationToken)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }
#endif

        #endregion Overrides of RepositoryBase<T>
    }

    /// <summary>
    /// 仓储扩展方法。
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 更新实体的单个属性。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="entity">实体。</param>
        /// <param name="expression">选择属性表达式。</param>
        public static void UpdateProperty<TEntity, TProperty>(this IRepository<TEntity> repository, TEntity entity, Expression<Func<TEntity, TProperty>> expression) where TEntity : class, IEntity, new()
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.Id <= 0)
                throw new ArgumentOutOfRangeException("entity", "Id不能是小等于0的数字。");

            var efRepository = (repository as EntityFrameworkRepository<TEntity>);

            if (efRepository == null)
                throw new ArgumentException("repository不是类型为EntityFrameworkRepository的类型。");

            var context = efRepository.DbContext;
            var set = efRepository.DbSet;

            if (expression == null)
                throw new ArgumentNullException("expression");

            var member = expression.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("只能指定一个属性", "expression");

            var efEntity = set.Local.FirstOrDefault(i => i.Id == entity.Id);
            var propertyName = member.Member.Name;

            if (efEntity == null)
            {
                efEntity = set.Attach(entity);
                context.Entry(efEntity).Property(propertyName).IsModified = true;
            }
            else
            {
                var entry = context.Entry(efEntity);
                var property = entry.Property(propertyName);
                property.CurrentValue = entry.Property(propertyName).CurrentValue;
                property.IsModified = true;
            }
        }
    }
}