using System;
using System.Linq;
using System.Linq.Expressions;

namespace Rabbit.Components.Data
{
    /// <summary>
    /// 可以排序的对象。
    /// </summary>
    /// <typeparam name="T">Item类型。</typeparam>
    public class Orderable<T>
    {
        #region Constructor

        /// <summary>
        /// 创建一个可排序的对象。
        /// </summary>
        /// <param name="enumerable">可提供查询的枚举对象。</param>
        public Orderable(IQueryable<T> enumerable)
        {
            Queryable = enumerable;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 课查询的对象。
        /// </summary>
        public IQueryable<T> Queryable { get; private set; }

        #endregion Property

        #region Public Method

        #region Asc

        /// <summary>
        /// 使用Asc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey">键值类型。</typeparam>
        /// <param name="keySelector">键值选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Asc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            Queryable = Queryable
                .OrderBy(keySelector);
            return this;
        }

        /// <summary>
        /// 使用Asc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey1">键值1类型。</typeparam>
        /// <typeparam name="TKey2">键值2类型。</typeparam>
        /// <param name="keySelector1">键值1选择器。</param>
        /// <param name="keySelector2">键值2选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Asc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                              Expression<Func<T, TKey2>> keySelector2)
        {
            Queryable = Queryable
                .OrderBy(keySelector1)
                .ThenBy(keySelector2);
            return this;
        }

        /// <summary>
        /// 使用Asc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey1">键值1类型。</typeparam>
        /// <typeparam name="TKey2">键值2类型。</typeparam>
        /// <typeparam name="TKey3">键值3类型。</typeparam>
        /// <param name="keySelector1">键值1选择器。</param>
        /// <param name="keySelector2">键值2选择器。</param>
        /// <param name="keySelector3">键值3选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Asc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                     Expression<Func<T, TKey2>> keySelector2,
                                                     Expression<Func<T, TKey3>> keySelector3)
        {
            Queryable = Queryable
                .OrderBy(keySelector1)
                .ThenBy(keySelector2)
                .ThenBy(keySelector3);
            return this;
        }

        #endregion Asc

        #region Desc

        /// <summary>
        /// 使用Desc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey">键值1类型。</typeparam>
        /// <param name="keySelector">键值1选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Desc<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            Queryable = Queryable
                .OrderByDescending(keySelector);
            return this;
        }

        /// <summary>
        /// 使用Desc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey1">键值1类型。</typeparam>
        /// <typeparam name="TKey2">键值2类型。</typeparam>
        /// <param name="keySelector1">键值1选择器。</param>
        /// <param name="keySelector2">键值2选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Desc<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1,
                                               Expression<Func<T, TKey2>> keySelector2)
        {
            Queryable = Queryable
                .OrderByDescending(keySelector1)
                .ThenByDescending(keySelector2);
            return this;
        }

        /// <summary>
        /// 使用Desc规则进行排序。
        /// </summary>
        /// <typeparam name="TKey1">键值1类型。</typeparam>
        /// <typeparam name="TKey2">键值2类型。</typeparam>
        /// <typeparam name="TKey3">键值3类型。</typeparam>
        /// <param name="keySelector1">键值1选择器。</param>
        /// <param name="keySelector2">键值2选择器。</param>
        /// <param name="keySelector3">键值3选择器。</param>
        /// <returns>可排序的对象。</returns>
        public Orderable<T> Desc<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1,
                                                      Expression<Func<T, TKey2>> keySelector2,
                                                      Expression<Func<T, TKey3>> keySelector3)
        {
            Queryable = Queryable
                .OrderByDescending(keySelector1)
                .ThenByDescending(keySelector2)
                .ThenByDescending(keySelector3);
            return this;
        }

        #endregion Desc

        #endregion Public Method
    }
}