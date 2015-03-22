using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 表示实现者一个仓储模式的实现。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    /// <remarks>查询使用Linq，全局瞬态。</remarks>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 表记录。
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #region Query

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 null。</exception>
        IEnumerable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <param name="order">排序策略。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="order"/> 为 null。</exception>
        IEnumerable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order);

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <param name="order">排序策略。</param>
        /// <param name="skip">需要跳过的记录数目。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="order"/> 为 null。</exception>
        /// <exception cref="OutOfMemoryException"><paramref name="skip"/> 小于0。</exception>
        IEnumerable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int skip);

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <param name="order">排序策略。</param>
        /// <param name="skip">需要跳过的记录数目。</param>
        /// <param name="count">需要获取的记录数目。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="order"/> 为 null。</exception>
        /// <exception cref="OutOfMemoryException"><paramref name="skip"/> 或 <paramref name="count"/> 小于0。</exception>
        IEnumerable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int skip, int count);

        #endregion Query

        #region Create

        /// <summary>
        /// 创建。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> 为 null。</exception>
        void Create(TEntity entity);

        /// <summary>
        /// 创建或更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> 为 null。</exception>
        void CreateOrUpdate(TEntity entity);

        /// <summary>
        /// 创建多个实体。
        /// </summary>
        /// <param name="collection">实体信息集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 null。</exception>
        void CreateRange(IEnumerable<TEntity> collection);

        /// <summary>
        /// 创建或更新多个实体。
        /// </summary>
        /// <param name="collection">实体信息集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 null。</exception>
        void CreateOrUpdateRange(IEnumerable<TEntity> collection);

        #endregion Create

        #region Update

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        void Update(TEntity entity);

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="predicate">表达式。</param>
        /// <param name="updateAction">根据表达式将筛选到的实体进行更新动作。（找不到实体则不执行更新动作）</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="updateAction"/> 为 null。</exception>
        void Update(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction);

        #endregion Update

        #region Delete

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/> 为 null。</exception>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> 为 null。</exception>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除符合表达式的记录。
        /// </summary>
        /// <param name="predicate">表达式。</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 null。</exception>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        void DeleteAll();

        #endregion Delete

        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        void Flush();
    }

    /// <summary>
    /// 仓储扩展。
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 创建多个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="entities">实体信息集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 或 <paramref name="entities"/> 为 null。</exception>
        public static void CreateRange<T>(this IRepository<T> repository, params T[] entities) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (entities == null)
                throw new ArgumentNullException("entities");

            if (!entities.Any())
                return;

            repository.CreateRange(entities.AsEnumerable());
        }

        /// <summary>
        /// 创建或更新多个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="entities">实体集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 或 <paramref name="entities"/> 为 null。</exception>
        public static void CreateOrUpdateRange<T>(this IRepository<T> repository, params T[] entities) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (entities == null)
                throw new ArgumentNullException("entities");

            if (!entities.Any())
                return;

            repository.CreateOrUpdateRange(entities.AsEnumerable());
        }

        /// <summary>
        /// 是否存在。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="id">标识。</param>
        /// <returns>是否存在。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 为 null。</exception>
        public static bool Contains<T>(this IRepository<T> repository, long id) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return repository.Table.Any(entity => entity.Id == id);
        }

        /// <summary>
        /// 获取一个实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="id">标识。</param>
        /// <returns>实体。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 为 null。</exception>
        public static T Get<T>(this IRepository<T> repository, long id) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            return repository.Table.FirstOrDefault(entity => entity.Id == id);
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="ids">标识组。</param>
        /// <param name="repository">仓储。</param>
        /// <param name="updateAction">根据实体主键获取到对应实体进行更新动作。（找不到实体则不执行更新动作）</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 或 <paramref name="updateAction"/> 或 <paramref name="ids"/> 为 null。</exception>
        public static void Update<T>(this IRepository<T> repository, Action<T> updateAction, params long[] ids) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (updateAction == null)
                throw new ArgumentNullException("updateAction");
            if (ids == null)
                throw new ArgumentNullException("ids");

            if (!ids.Any())
                return;

            repository.Update(entity => ids.Contains(entity.Id), updateAction);
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="repository">仓储。</param>
        /// <param name="ids">标识组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> 或 <paramref name="ids"/> 为 null。</exception>
        public static void Delete<T>(this IRepository<T> repository, params long[] ids) where T : class,IEntity
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (ids == null)
                throw new ArgumentNullException("ids");

            if (!ids.Any())
                return;

            repository.Delete(entity => ids.Contains(entity.Id));
        }
    }
}