using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

#if !NET40
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 仓储抽象类。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        #region Implementation of IRepository<T>

        /// <summary>
        /// 表记录。
        /// </summary>
        public abstract IQueryable<T> Table { get; }

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 为 null。</exception>
        public virtual IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return InternalFetch(predicate, null, null, null);
        }

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <param name="order">排序策略。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="order"/> 为 null。</exception>
        public virtual IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (order == null)
                throw new ArgumentNullException("order");

            return InternalFetch(predicate, order, null, null);
        }

        /// <summary>
        /// 抓取数据集合。
        /// </summary>
        /// <param name="predicate">查询表达式。</param>
        /// <param name="order">排序策略。</param>
        /// <param name="skip">需要跳过的记录数目。</param>
        /// <returns>符合查询表达式的记录。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 或 <paramref name="order"/> 为 null。</exception>
        public virtual IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (order == null)
                throw new ArgumentNullException("order");
            if (skip <= 0)
                throw new OutOfMemoryException("需要跳过的记录数目不能小于0.");

            return InternalFetch(predicate, order, skip, null);
        }

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
        public virtual IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip, int count)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (order == null)
                throw new ArgumentNullException("order");
            if (skip <= 0)
                throw new OutOfMemoryException("需要跳过的记录数目不能小于0.");
            if (count <= 0)
                throw new OutOfMemoryException("需要获取的记录数目不能小于0.");

            return InternalFetch(predicate, order, skip, count);
        }

        /// <summary>
        /// 创建。
        /// </summary>
        /// <param name="entity">实体。</param>
        public abstract void Create(T entity);

        /// <summary>
        /// 创建或更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        public abstract void CreateOrUpdate(T entity);

        /// <summary>
        /// 创建多个实体。
        /// </summary>
        /// <param name="collection">实体信息集合。</param>
        public virtual void CreateRange(IEnumerable<T> collection)
        {
            foreach (var entity in collection)
                Create(entity);
        }

        /// <summary>
        /// 创建或更新多个实体。
        /// </summary>
        /// <param name="collection">实体信息集合。</param>
        public virtual void CreateOrUpdateRange(IEnumerable<T> collection)
        {
            foreach (var entity in collection)
                CreateOrUpdate(entity);
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="entity">实体。</param>
        public abstract void Update(T entity);

        /// <summary>
        /// 更新。
        /// </summary>
        /// <param name="predicate">表达式。</param>
        /// <param name="updateAction">根据表达式将筛选到的实体进行更新动作。（找不到实体则不执行更新动作）</param>
        public virtual void Update(Expression<Func<T, bool>> predicate, Action<T> updateAction)
        {
            foreach (var entity in Fetch(predicate))
            {
                updateAction(entity);
                Update(entity);
            }
        }

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entity">实体。</param>
        public abstract void Delete(T entity);

        /// <summary>
        /// 删除。
        /// </summary>
        /// <param name="entities">实体集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="entities"/> 为 null。</exception>
        public abstract void Delete(IEnumerable<T> entities);

        /// <summary>
        /// 删除符合表达式的记录。
        /// </summary>
        /// <param name="predicate">表达式。</param>
        public abstract void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 删除所有记录。
        /// </summary>
        public virtual void DeleteAll()
        {
            Delete(entity => true);
        }

        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        public abstract void Flush();

#if !NET40
        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        /// <returns>表示异步任务保存操作，任务结果包含写入对象的数量。</returns>
        public abstract Task<int> FlushAsync();

        /// <summary>
        /// 刷新缓存区。
        /// </summary>
        /// <param name="cancellationToken">一个System.Threading.CancellationToken以等待任务完成观察。</param>
        /// <returns>表示异步任务保存操作，任务结果包含写入对象的数量。</returns>
        public abstract Task<int> FlushAsync(CancellationToken cancellationToken);
#endif

        #endregion Implementation of IRepository<T>

        private IEnumerable<T> InternalFetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int? skip, int? count)
        {
            var result = Table;

            if (predicate != null)
                result = result.Where(predicate);

            if (order != null)
            {
                var o = new Orderable<T>(result);
                order(o);
                result = o.Queryable;
            }

            if (skip.HasValue && skip.Value > 0)
                result = result.Skip(skip.Value);

            if (count.HasValue && count.Value > 0)
                result = result.Take(count.Value);

            return result;
        }
    }
}