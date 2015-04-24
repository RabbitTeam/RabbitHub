using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System;
using System.Dynamic;
using System.Linq;

namespace Rabbit.Web.Mvc.UI.Zones
{
    /// <summary>
    /// 提供有一个区域属性的形状的行为。
    /// 例子包括布局和项目
    /// * 为区返回一个假的父对象
    /// Foo.Zones
    /// *
    /// Foo.Zones.Alpha :
    /// Foo.Zones["Alpha"]
    /// Foo.Alpha :同上
    /// </summary>
    public sealed class ZoneHolding : Shape
    {
        private readonly Func<dynamic> _zoneFactory;

        /// <summary>
        /// 初始化一个新的区域属性行为。
        /// </summary>
        /// <param name="zoneFactory"></param>
        public ZoneHolding(Func<dynamic> zoneFactory)
        {
            _zoneFactory = zoneFactory;
        }

        private Zones _zones;

        /// <summary>
        /// 区域。
        /// </summary>
        public Zones Zones
        {
            get
            {
                if (_zones == null)
                {
                    return _zones = new Zones(_zoneFactory, this);
                }

                return _zones;
            }
        }

        #region Overrides of Composite

        /// <summary>
        /// 为获取成员值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于 Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleProperty”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="result">获取操作的结果。例如，如果为某个属性调用该方法，则可以为 <paramref name="result"/> 指派该属性值。</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;

            if (base.TryGetMember(binder, out result) && (null != result))
                return true;
            result = new ZoneOnDemand(_zoneFactory, this, name);
            TrySetMemberImpl(name, result);

            return true;
        }

        #endregion Overrides of Composite
    }

    /// <remarks>
    /// InterfaceProxyBehavior()
    /// ZonesBehavior(_zoneFactory, self, _layoutShape) => 如果有成员访问则创建ZoneOnDemand
    /// </remarks>
    public sealed class Zones : Composite
    {
        #region Field

        private readonly Func<dynamic> _zoneFactory;
        private readonly object _parent;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的区域。
        /// </summary>
        /// <param name="zoneFactory">区域工厂。</param>
        /// <param name="parent">父级区域。</param>
        public Zones(Func<dynamic> zoneFactory, object parent)
        {
            _zoneFactory = zoneFactory;
            _parent = parent;
        }

        #endregion Constructor

        #region Overrides of Composite

        /*/// <summary>
        /// 为获取成员值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于 Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleProperty”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="result">获取操作的结果。例如，如果为某个属性调用该方法，则可以为 <paramref name="result"/> 指派该属性值。</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMemberImpl(binder.Name, out result);
        }*/

        /// <summary>
        /// 尝试获取成员值。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <param name="result">成员值。</param>
        /// <returns>如果成功返回true，否则返回false。</returns>
        protected override bool TryGetMemberImpl(string name, out object result)
        {
            var parentMember = ((dynamic)_parent)[name];
            if (parentMember == null)
            {
                result = new ZoneOnDemand(_zoneFactory, _parent, name);
                return true;
            }

            result = parentMember;
            return true;
        }

        /// <summary>
        /// 为按索引获取值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为索引操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关该操作的信息。</param><param name="indexes">该操作中使用的索引。例如，对于 C# 中的 sampleObject[3] 操作（Visual Basic 中为 sampleObject(3)）（其中 sampleObject 派生自 DynamicObject 类），<paramref name="indexes[0]"/> 等于 3。</param><param name="result">索引操作的结果。</param>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count() != 1)
                return base.TryGetIndex(binder, indexes, out result);

            var key = Convert.ToString(indexes.Single());
            return TryGetMemberImpl(key, out result);
        }

        #endregion Overrides of Composite
    }

    internal sealed class ZoneOnDemand : Shape
    {
        #region Field

        private readonly Func<dynamic> _zoneFactory;
        private readonly object _parent;
        private readonly string _potentialZoneName;

        #endregion Field

        #region Constructor

        public ZoneOnDemand(Func<dynamic> zoneFactory, object parent, string potentialZoneName)
        {
            _zoneFactory = zoneFactory;
            _parent = parent;
            _potentialZoneName = potentialZoneName;
        }

        #endregion Constructor

        #region Overrides of Shape

        /// <summary>
        /// 为调用成员的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如调用方法这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关动态操作的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于语句 sampleObject.SampleMethod(100)（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleMethod”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="args">调用操作期间传递给对象成员的参数。例如，对于语句 sampleObject.SampleMethod(100)（其中 sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类），<paramref name="args[0]"/> 等于 100。</param><param name="result">成员调用的结果。</param>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name;

            if (args.Any() || name == "ToString")
                return base.TryInvokeMember(binder, args, out result);
            result = Nil.Instance;
            return true;
        }

        /// <summary>
        /// 提供类型转换运算的实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为将某个对象从一种类型转换为另一种类型的运算指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此运算成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关转换运算的信息。binder.Type 属性提供必须将对象转换为的类型。例如，对于 C# 中的语句 (String)sampleObject（Visual Basic 中为 CType(sampleObject, Type)）（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Type 将返回 <see cref="T:System.String"/> 类型。binder.Explicit 属性提供有关所发生转换的类型的信息。对于显式转换，它返回 true；对于隐式转换，它返回 false。</param><param name="result">类型转换运算的结果。</param>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == typeof(string))
            {
                result = null;
            }
            else if (binder.ReturnType.IsValueType)
            {
                result = Activator.CreateInstance(binder.ReturnType);
            }
            else
            {
                result = null;
            }

            return true;
        }

        /// <summary>
        /// 添加一个形状。
        /// </summary>
        /// <param name="item">形状项。</param>
        /// <param name="position">位置。</param>
        /// <returns>形状根。</returns>
        public override Shape Add(object item, string position = null)
        {
            if (item == null)
            {
                return (Shape)_parent;
            }

            dynamic parent = _parent;

            var zone = _zoneFactory();
            zone.Parent = _parent;
            zone.ZoneName = _potentialZoneName;
            parent[_potentialZoneName] = zone;

            return position == null ? zone.Add(item) : zone.Add(item, position);
        }

        #endregion Overrides of Shape

        #region Overrides of Composite

        /// <summary>
        /// 为获取成员值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于 Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleProperty”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="result">获取操作的结果。例如，如果为某个属性调用该方法，则可以为 <paramref name="result"/> 指派该属性值。</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Nil.Instance;
            return true;
        }

        /// <summary>
        /// 为按索引获取值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为索引操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关该操作的信息。</param><param name="indexes">该操作中使用的索引。例如，对于 C# 中的 sampleObject[3] 操作（Visual Basic 中为 sampleObject(3)）（其中 sampleObject 派生自 DynamicObject 类），<paramref name="indexes[0]"/> 等于 3。</param><param name="result">索引操作的结果。</param>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Nil.Instance;
            return true;
        }

        /// <summary>
        /// 确定指定的 <see cref="T:System.Object"/> 是否等于当前的 <see cref="T:System.Object"/>。
        /// </summary>
        /// <returns>
        /// 如果指定的 <see cref="T:System.Object"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(null, obj) || ReferenceEquals(this, obj);
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_parent != null ? _parent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_potentialZoneName != null ? _potentialZoneName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion Overrides of Composite

        #region Overrides of Object

        /// <summary>
        /// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
        /// </summary>
        /// <returns>
        /// <see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
        /// </returns>
        public override string ToString()
        {
            return string.Empty;
        }

        #endregion Overrides of Object

        /// <summary>
        /// <paramref name="a"/> 与 <paramref name="b"/> 是否相等。
        /// </summary>
        /// <param name="a">对象1.</param>
        /// <param name="b">对象2.</param>
        /// <returns>如果相等则返回true，否则返回false。</returns>
        public static bool operator ==(ZoneOnDemand a, object b)
        {
            return b == null || ReferenceEquals(b, Nil.Instance);
        }

        /// <summary>
        /// <paramref name="a"/> 与 <paramref name="b"/> 是否不相等。
        /// </summary>
        /// <param name="a">对象1.</param>
        /// <param name="b">对象2.</param>
        /// <returns>如果不相等则返回true，否则返回false。</returns>
        public static bool operator !=(ZoneOnDemand a, object b)
        {
            return !(a == b);
        }
    }
}