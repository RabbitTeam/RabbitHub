using Rabbit.Kernel.Environment.ShellBuilders.Models;
using Rabbit.Kernel.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Kernel.Environment.ShellBuilders
{
    /// <summary>
    /// 组合策略应用上下文。
    /// </summary>
    public sealed class CompositionStrategyApplyContext
    {
        /// <summary>
        /// 特性集合。
        /// </summary>
        public IEnumerable<Feature> Features { get; set; }

        /// <summary>
        /// 排除的类型。
        /// </summary>
        public IEnumerable<string> ExcludedTypes { get; set; }

        /// <summary>
        /// 外壳蓝图。
        /// </summary>
        public ShellBlueprint ShellBlueprint { get; set; }

        /// <summary>
        /// 建造蓝图。
        /// </summary>
        /// <param name="predicate">筛选器。</param>
        /// <param name="selector">选择器。</param>
        /// <returns>项集合。</returns>
        public IEnumerable<T> BuildBlueprint<T>(Func<Type, bool> predicate, Func<Type, Feature, T> selector)
        {
            return Features.SelectMany(
                feature => feature.ExportedTypes
                               .Where(predicate)
                               .Where(type => !ExcludedTypes.Contains(type.FullName))
                               .Select(type => selector(type, feature)))
                .ToArray();
        }
    }

    /// <summary>
    /// 一个抽象的组合策略提供者。
    /// </summary>
    public interface ICompositionStrategyProvider
    {
        /// <summary>
        /// 应用。
        /// </summary>
        /// <param name="context">组合策略应用上下文。</param>
        void Apply(CompositionStrategyApplyContext context);
    }
}