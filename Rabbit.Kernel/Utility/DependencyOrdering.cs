using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Utility
{
    /// <summary>
    /// 依赖项排序。
    /// </summary>
    public static class DependencyOrdering
    {
        /// <summary>
        /// 排序元素的集合“依赖顺序”。通过传递一个lambda确定如果一个元素是一个依赖另一个，
        /// 此算法将返回的元素的排序条件，以便一个给定的序列中的元素不依赖于任何其他元素序列中进一步收集。
        /// </summary>
        public static IEnumerable<T> OrderByDependenciesAndPriorities<T>(this IEnumerable<T> elements, Func<T, T, bool> hasDependency, Func<T, int> getPriority)
        {
            var population = elements.Select(d => new Linkage<T>
            {
                Element = d
            }).OrderBy(item => getPriority(item.Element)).ToArray();//通过优先执行初始的排序可以优化性能

            var result = new List<T>();
            foreach (var item in population)
            {
                Add(item, result, population, hasDependency);
            }

            // 在优先级和依赖转向元素向前越好
            for (var i = 1; i < result.Count; i++)
            {
                var bestPosition = BestPriorityPosition(result, i, hasDependency, getPriority);
                SwitchAndShift(result, i, bestPosition);
            }

            return result;
        }

        #region Private Method

        private static void Add<T>(Linkage<T> item, ICollection<T> list, IEnumerable<Linkage<T>> population, Func<T, T, bool> hasDependency)
        {
            population = population.ToArray();
            if (item.Used)
                return;

            item.Used = true;

            foreach (var dependency in population.Where(dep => hasDependency(item.Element, dep.Element)))
            {
                Add(dependency, list, population, hasDependency);
            }

            list.Add(item.Element);
        }

        private static int BestPriorityPosition<T>(IList<T> list, int index, Func<T, T, bool> hasDependency, Func<T, int> getPriority)
        {
            var bestPriority = getPriority(list[index]);
            var bestIndex = index;

            for (var i = index - 1; i >= 0; i--)
            {
                if (hasDependency(list[index], list[i]))
                {
                    return bestIndex;
                }

                var currentPriority = getPriority(list[i]);
                if (currentPriority > bestPriority)
                {
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static void SwitchAndShift<T>(IList<T> list, int initialPosition, int finalPosition)
        {
            if (initialPosition < finalPosition)
                throw new ArgumentException("finalPosition");

            if (initialPosition == finalPosition)
                return;

            var temp = list[initialPosition];

            for (; initialPosition > finalPosition; initialPosition--)
                list[initialPosition] = list[initialPosition - 1];

            list[finalPosition] = temp;
        }

        #endregion Private Method

        #region Help Class

        private sealed class Linkage<T>
        {
            public T Element { get; set; }

            public bool Used { get; set; }
        }

        #endregion Help Class
    }
}