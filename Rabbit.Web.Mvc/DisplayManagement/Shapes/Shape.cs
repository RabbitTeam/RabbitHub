using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.DisplayManagement.Shapes
{
    /// <summary>
    /// 形状。
    /// </summary>
    [DebuggerTypeProxy(typeof(ShapeDebugView))]
    public class Shape : Composite, IShape, IEnumerable<object>
    {
        #region Field

        private const string DefaultPosition = "5";

        private readonly IList<object> _items = new List<object>();
        private readonly IList<string> _classes = new List<string>();
        private readonly IDictionary<string, string> _attributes = new Dictionary<string, string>();

        #endregion Field

        #region Property

        /// <summary>
        /// Id。
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 样式类名集合。
        /// </summary>
        public virtual IList<string> Classes { get { return _classes; } }

        /// <summary>
        /// 属性集合。
        /// </summary>
        public virtual IDictionary<string, string> Attributes { get { return _attributes; } }

        /// <summary>
        /// 项。
        /// </summary>
        public virtual IEnumerable<dynamic> Items { get { return _items; } }

        #endregion Property

        #region Constructor

        /// <summary>
        /// 初始化一个新的形状。
        /// </summary>
        public Shape()
        {
            Metadata = new ShapeMetadata();
        }

        #endregion Constructor

        #region Implementation of IShape

        /// <summary>
        /// 形状元数据。
        /// </summary>
        public ShapeMetadata Metadata { get; set; }

        #endregion Implementation of IShape

        #region Implementation of IEnumerable

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.Generic.IEnumerator`1"/>。
        /// </returns>
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>
        /// 可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator"/> 对象。
        /// </returns>

        public virtual IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion Implementation of IEnumerable

        #region Public Method

        /// <summary>
        /// 添加一个形状。
        /// </summary>
        /// <param name="item">形状项。</param>
        /// <param name="position">位置。</param>
        /// <returns>形状根。</returns>
        public virtual Shape Add(object item, string position = null)
        {
            //忽略空的形状
            if (item == null)
                return this;

            try
            {
                //TODO:这是一个临时的实施，以防止常见的已知的情况下抛出异常。最终的解决方案将需要基于这样的事实，它是一个形状实例来过滤
                if (item is MvcHtmlString ||
                    item is String)
                {
                    //需要实现定位的包装用于非图形对象
                }
                else
                {
                    var o = item as IShape;
                    if (o != null)
                    {
                        ((dynamic)item).Metadata.Position = position;
                    }
                }
            }
            catch
            {
                //需要实现定位的包装用于非图形对象
            }

            _items.Add(item);
            return this;
        }

        /// <summary>
        /// 添加一组形状。
        /// </summary>
        /// <param name="items">形状组。</param>
        /// <param name="position">位置。</param>
        /// <returns>形状根。</returns>
        public virtual Shape AddRange(IEnumerable<object> items, string position = DefaultPosition)
        {
            foreach (var item in items)
                Add(item, position);
            return this;
        }

        #endregion Public Method

        #region Overrides of Composite

        /// <summary>
        /// 提供类型转换运算的实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为将某个对象从一种类型转换为另一种类型的运算指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此运算成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关转换运算的信息。binder.Type 属性提供必须将对象转换为的类型。例如，对于 C# 中的语句 (String)sampleObject（Visual Basic 中为 CType(sampleObject, Type)）（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Type 将返回 <see cref="T:System.String"/> 类型。binder.Explicit 属性提供有关所发生转换的类型的信息。对于显式转换，它返回 true；对于隐式转换，它返回 false。</param><param name="result">类型转换运算的结果。</param>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Items;

            if (binder.ReturnType == typeof(IEnumerable<object>)
                || binder.ReturnType == typeof(IEnumerable<dynamic>))
            {
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        /// <summary>
        /// 为按索引设置值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为按指定索引访问对象的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关该操作的信息。</param><param name="indexes">该操作中使用的索引。例如，对于 C# 中的 sampleObject[3] = 10 操作（Visual Basic 中为 sampleObject(3) = 10）（其中 sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类），<paramref name="indexes[0]"/> 等于 3。</param><param name="value">要为具有指定索引的对象设置的值。例如，对于 C# 中的 sampleObject[3] = 10 操作（Visual Basic 中为 sampleObject(3) = 10）（其中 sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类），<paramref name="value"/> 等于 10。</param>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Count() == 1)
            {
                var name = indexes.Single().ToString();
                if (name.Equals("Id"))
                {
                    //需要变异的实际类型
                    Id = Convert.ToString(value);
                    return true;
                }
                if (name.Equals("Classes"))
                {
                    var args = Arguments.From(new[] { value }, Enumerable.Empty<string>());
                    MergeClasses(args, Classes);
                    return true;
                }
                if (name.Equals("Attributes"))
                {
                    var args = Arguments.From(new[] { value }, Enumerable.Empty<string>());
                    MergeAttributes(args, Attributes);
                    return true;
                }
                if (name.Equals("Items"))
                {
                    var args = Arguments.From(new[] { value }, Enumerable.Empty<string>());
                    MergeItems(args, this);
                    return true;
                }
            }

            return base.TrySetIndex(binder, indexes, value);
        }

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
            var arguments = Arguments.From(args, binder.CallInfo.ArgumentNames);
            if (name.Equals("Id"))
            {
                //需要变异的实际类型
                Id = Convert.ToString(args.FirstOrDefault());
                result = this;
                return true;
            }
            if (name.Equals("Classes") && !arguments.Named.Any())
            {
                MergeClasses(arguments, Classes);
                result = this;
                return true;
            }
            if (name.Equals("Attributes") && arguments.Positional.Count() <= 1)
            {
                MergeAttributes(arguments, Attributes);
                result = this;
                return true;
            }
            if (name.Equals("Items"))
            {
                MergeItems(arguments, this);
                result = this;
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        #endregion Overrides of Composite

        #region Private Method

        private static void MergeAttributes(INamedEnumerable<object> args, IDictionary<string, string> attributes)
        {
            var arg = args.Positional.SingleOrDefault();
            if (arg != null)
            {
                if (arg is IDictionary)
                {
                    var dictionary = arg as IDictionary;
                    foreach (var key in dictionary.Keys)
                    {
                        attributes[Convert.ToString(key)] = Convert.ToString(dictionary[key]);
                    }
                }
                else
                {
                    foreach (var prop in arg.GetType().GetProperties())
                    {
                        attributes[TranslateIdentifier(prop.Name)] = Convert.ToString(prop.GetValue(arg, null));
                    }
                }
            }
            foreach (var named in args.Named)
            {
                attributes[named.Key] = Convert.ToString(named.Value);
            }
        }

        private static string TranslateIdentifier(string name)
        {
            //允许在标识符某些字符来表示不同的
            //在HTML属性（模仿MVC的行为）字符：
            // data_foo ==> data-foo
            // @keyword ==> keyword
            return name.Replace("_", "-").Replace("@", "");
        }

        private static void MergeClasses(IEnumerable<object> args, ICollection<string> classes)
        {
            foreach (var arg in args)
            {
                //查找字符串的第一个，因为“string”类型也是字符的IEnumerable
                if (arg is string)
                {
                    classes.Add(arg as string);
                }
                else if (arg is IEnumerable)
                {
                    foreach (var item in arg as IEnumerable)
                    {
                        classes.Add(Convert.ToString(item));
                    }
                }
                else
                {
                    classes.Add(Convert.ToString(arg));
                }
            }
        }

        private static void MergeItems(IEnumerable<object> args, dynamic shape)
        {
            foreach (var arg in args)
            {
                //查找字符串的第一个，因为“string”类型也是字符的IEnumerable
                if (arg is string)
                {
                    shape.Add(arg as string);
                }
                else if (arg is IEnumerable)
                {
                    foreach (var item in arg as IEnumerable)
                    {
                        shape.Add(item);
                    }
                }
                else
                {
                    shape.Add(Convert.ToString(arg));
                }
            }
        }

        #endregion Private Method
    }
}