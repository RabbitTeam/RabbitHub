using Castle.DynamicProxy;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace Rabbit.Web.Mvc.DisplayManagement.Shapes
{
    /// <summary>
    /// 一个混合的动态对象。
    /// </summary>
    public class Composite : DynamicObject
    {
        #region Field

        private readonly IDictionary _props = new HybridDictionary();

        #endregion Field

        #region Property

        /// <summary>
        /// 属性字典。
        /// </summary>
        public IDictionary Properties
        {
            get { return _props; }
        }

        #endregion Property

        #region Overrides of DynamicObject

        /// <summary>
        /// 为获取成员值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如获取属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于 Console.WriteLine(sampleObject.SampleProperty) 语句（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleProperty”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="result">获取操作的结果。例如，如果为某个属性调用该方法，则可以为 <paramref name="result"/> 指派该属性值。</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMemberImpl(binder.Name, out result);
        }

        /// <summary>
        /// 为设置成员值的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如设置属性值这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用了动态操作的对象的信息。binder.Name 属性提供将该值分配到的成员的名称。例如，对于语句 sampleObject.SampleProperty = "Test"（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleProperty”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="value">要为成员设置的值。例如，对于 sampleObject.SampleProperty = "Test"（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），<paramref name="value"/> 为“Test”。</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySetMemberImpl(binder.Name, value);
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
            if (!args.Any())
            {
                return TryGetMemberImpl(binder.Name, out result);
            }

            //方法调用同一个参数会指定属性
            if (args.Count() == 1)
            {
                result = this;
                return TrySetMemberImpl(binder.Name, args.Single());
            }

            if (!base.TryInvokeMember(binder, args, out result))
            {
                if (binder.Name == "ToString")
                {
                    result = string.Empty;
                    return true;
                }

                return false;
            }

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
            {
                return base.TryGetIndex(binder, indexes, out result);
            }

            var index = indexes.Single();

            if (_props.Contains(index))
            {
                result = _props[index];
                return true;
            }

            //尝试访问一个现有成员
            var strinIndex = index as string;

            if (strinIndex != null && TryGetMemberImpl(strinIndex, out result))
            {
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);
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
            if (indexes.Count() != 1)
            {
                return base.TrySetIndex(binder, indexes, value);
            }

            var index = indexes.Single();

            //尝试访问一个现有成员
            var strinIndex = index as string;

            if (strinIndex != null && TrySetMemberImpl(strinIndex, value))
            {
                return true;
            }

            _props[indexes.Single()] = value;
            return true;
        }

        #endregion Overrides of DynamicObject

        #region Virtual Method

        /// <summary>
        /// 尝试对成员进行赋值。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <param name="value">值。</param>
        /// <returns>如果成功返回true，否则返回false。</returns>
        protected virtual bool TrySetMemberImpl(string name, object value)
        {
            _props[name] = value;
            return true;
        }

        /// <summary>
        /// 尝试获取成员值。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <param name="result">成员值。</param>
        /// <returns>如果成功返回true，否则返回false。</returns>
        protected virtual bool TryGetMemberImpl(string name, out object result)
        {
            if (_props.Contains(name))
            {
                result = _props[name];
                return true;
            }

            result = null;
            return true;
        }

        #endregion Virtual Method

        /// <summary>
        /// <paramref name="a"/> 与 <paramref name="b"/> 是否相等。
        /// </summary>
        /// <param name="a">对象1.</param>
        /// <param name="b">对象2.</param>
        /// <returns>如果相等则返回true，否则返回false。</returns>
        public static bool operator ==(Composite a, Nil b)
        {
            return null == a;
        }

        /// <summary>
        /// <paramref name="a"/> 与 <paramref name="b"/> 是否不相等。
        /// </summary>
        /// <param name="a">对象1.</param>
        /// <param name="b">对象2.</param>
        /// <returns>如果不相等则返回true，否则返回false。</returns>
        public static bool operator !=(Composite a, Nil b)
        {
            return !(a == b);
        }

        /// <summary>
        /// 确定指定的 <see cref="T:Rabbit.Web.Mvc.DisplayManagement.Shapes.Composite"/> 是否等于当前的 <see cref="T:System.Object"/>。
        /// </summary>
        /// <returns>
        /// 如果指定的 <see cref="T:Rabbit.Web.Mvc.DisplayManagement.Shapes.Composite"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param>
        protected bool Equals(Composite obj)
        {
            return Equals(_props, obj._props);
        }

        #region Overrides of Object

        /// <summary>
        /// 确定指定的 <see cref="T:System.Object"/> 是否等于当前的 <see cref="T:System.Object"/>。
        /// </summary>
        /// <returns>
        /// 如果指定的 <see cref="T:System.Object"/> 等于当前的 <see cref="T:System.Object"/>，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">与当前的 <see cref="T:System.Object"/> 进行比较的 <see cref="T:System.Object"/>。</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Composite)obj);
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        /// 当前 <see cref="T:System.Object"/> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            return (_props != null ? _props.GetHashCode() : 0);
        }

        #endregion Overrides of Object

        #region InterfaceProxyBehavior

        private static readonly IProxyBuilder ProxyBuilder = new DefaultProxyBuilder();
        private static readonly MethodInfo DynamicMetaObjectProviderGetMetaObject = typeof(IDynamicMetaObjectProvider).GetMethod("GetMetaObject");

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var type = binder.ReturnType;

            if (type.IsInterface && type != typeof(IDynamicMetaObjectProvider))
            {
                var proxyType = ProxyBuilder.CreateInterfaceProxyTypeWithoutTarget(
                    type,
                    new[] { typeof(IDynamicMetaObjectProvider) },
                    ProxyGenerationOptions.Default);

                var interceptors = new IInterceptor[] { new Interceptor(this) };
                var proxy = Activator.CreateInstance(proxyType, new object[] { interceptors, this });
                result = proxy;
                return true;
            }

            result = null;
            return false;
        }

        private class Interceptor : IInterceptor
        {
            private object Self { get; set; }

            public Interceptor(object self)
            {
                Self = self;
            }

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method == DynamicMetaObjectProviderGetMetaObject)
                {
                    var expression = (Expression)invocation.Arguments.Single();
                    invocation.ReturnValue = new ForwardingMetaObject(
                        expression,
                        BindingRestrictions.Empty,
                        invocation.Proxy,
                        (IDynamicMetaObjectProvider)Self,
                        exprProxy => Expression.Field(exprProxy, "__target"));

                    return;
                }

                var invoker = BindInvoker(invocation);
                invoker(invocation);
            }

            private static readonly ConcurrentDictionary<MethodInfo, Action<IInvocation>> Invokers = new ConcurrentDictionary<MethodInfo, Action<IInvocation>>();

            private static Action<IInvocation> BindInvoker(IInvocation invocation)
            {
                return Invokers.GetOrAdd(invocation.Method, CompileInvoker);
            }

            private static Action<IInvocation> CompileInvoker(MethodInfo method)
            {
                var methodParameters = method.GetParameters();
                var invocationParameter = Expression.Parameter(typeof(IInvocation), "invocation");

                var targetAndArgumentInfos = Pack(
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    methodParameters.Select(
                        mp => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, mp.Name))).ToArray();

                var targetAndArguments = Pack<Expression>(
                    Expression.Property(invocationParameter, invocationParameter.Type, "InvocationTarget"),
                    methodParameters.Select(
                        (mp, index) =>
                        Expression.Convert(
                            Expression.ArrayIndex(
                                Expression.Property(invocationParameter, invocationParameter.Type,
                                                    "Arguments"),
                                Expression.Constant(index)), mp.ParameterType))).ToArray();

                Expression body = null;
                if (method.IsSpecialName)
                {
                    if (method.Name.Equals("get_Item"))
                    {
                        body = Expression.Dynamic(
                            Binder.GetIndex(
                                CSharpBinderFlags.InvokeSpecialName,
                                typeof(object),
                                targetAndArgumentInfos),
                            typeof(object),
                            targetAndArguments);
                    }

                    if (body == null && method.Name.Equals("set_Item"))
                    {
                        var targetAndArgumentInfosWithoutTheNameValue = Pack(
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                            methodParameters.Select(
                                mp => mp.Name == "value" ? CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) : CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, mp.Name)));

                        body = Expression.Dynamic(
                            Binder.SetIndex(
                                CSharpBinderFlags.InvokeSpecialName,
                                typeof(object),
                                targetAndArgumentInfosWithoutTheNameValue),
                            typeof(object),
                            targetAndArguments);
                    }

                    if (body == null && method.Name.StartsWith("get_"))
                    {
                        //  构建包含以下调用网站的lambda:
                        //  (IInvocation invocation) => {
                        //      invocation.ReturnValue = (object) ((dynamic)invocation.InvocationTarget).{method.Name};
                        //  }
                        body = Expression.Dynamic(
                            Binder.GetMember(
                                CSharpBinderFlags.InvokeSpecialName,
                                method.Name.Substring("get_".Length),
                                typeof(object),
                                targetAndArgumentInfos),
                            typeof(object),
                            targetAndArguments);
                    }

                    if (body == null && method.Name.StartsWith("set_"))
                    {
                        body = Expression.Dynamic(
                            Binder.SetMember(
                                CSharpBinderFlags.InvokeSpecialName,
                                method.Name.Substring("set_".Length),
                                typeof(object),
                                targetAndArgumentInfos),
                            typeof(object),
                            targetAndArguments);
                    }
                }
                if (body == null)
                {
                    //  构建包含以下调用网站的lambda:
                    //  (IInvocation invocation) => {
                    //      invocation.ReturnValue = (object) ((dynamic)invocation.InvocationTarget).{method.Name}(
                    //          {methodParameters[*].Name}: ({methodParameters[*].Type})invocation.Arguments[*],
                    //          ...);
                    //  }

                    body = Expression.Dynamic(
                        Binder.InvokeMember(
                            CSharpBinderFlags.None,
                            method.Name,
                            null,
                            typeof(object),
                            targetAndArgumentInfos),
                        typeof(object),
                        targetAndArguments);
                }

                if (method.ReturnType != typeof(void))
                {
                    body = Expression.Assign(
                        Expression.Property(invocationParameter, invocationParameter.Type, "ReturnValue"),
                        Expression.Convert(body, typeof(object)));
                }

                var lambda = Expression.Lambda<Action<IInvocation>>(body, invocationParameter);
                return lambda.Compile();
            }
        }

        private static IEnumerable<T> Pack<T>(T t1, IEnumerable<T> t2)
        {
            if (!Equals(t1, default(T)))
                yield return t1;
            foreach (var t in t2)
                yield return t;
        }

        /// <summary>
        /// http://blog.tomasm.net/2009/11/07/forwarding-meta-object/
        /// </summary>
        private sealed class ForwardingMetaObject : DynamicMetaObject
        {
            private readonly DynamicMetaObject _metaForwardee;

            public ForwardingMetaObject(Expression expression, BindingRestrictions restrictions, object forwarder,
                IDynamicMetaObjectProvider forwardee, Func<Expression, Expression> forwardeeGetter)
                : base(expression, restrictions, forwarder)
            {
                // We'll use forwardee's meta-object to bind dynamic operations.
                _metaForwardee = forwardee.GetMetaObject(
                    forwardeeGetter(
                        Expression.Convert(expression, forwarder.GetType())   // [1]
                    )
                );
            }

            // Restricts the target object's type to TForwarder.
            // The meta-object we are forwarding to assumes that it gets an instance of TForwarder (see [1]).
            // We need to ensure that the assumption holds.
            private DynamicMetaObject AddRestrictions(DynamicMetaObject result)
            {
                var restricted = new DynamicMetaObject(
                    result.Expression,
                    BindingRestrictions.GetTypeRestriction(Expression, Value.GetType()).Merge(result.Restrictions),
                    _metaForwardee.Value
                    );
                return restricted;
            }

            // Forward all dynamic operations or some of them as needed //

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return AddRestrictions(_metaForwardee.BindGetMember(binder));
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                return AddRestrictions(_metaForwardee.BindSetMember(binder, value));
            }

            public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
            {
                return AddRestrictions(_metaForwardee.BindDeleteMember(binder));
            }

            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
            {
                return AddRestrictions(_metaForwardee.BindGetIndex(binder, indexes));
            }

            public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
            {
                return AddRestrictions(_metaForwardee.BindSetIndex(binder, indexes, value));
            }

            public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
            {
                return AddRestrictions(_metaForwardee.BindDeleteIndex(binder, indexes));
            }

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                return AddRestrictions(_metaForwardee.BindInvokeMember(binder, args));
            }

            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
            {
                return AddRestrictions(_metaForwardee.BindInvoke(binder, args));
            }

            public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
            {
                return AddRestrictions(_metaForwardee.BindCreateInstance(binder, args));
            }

            public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
            {
                return AddRestrictions(_metaForwardee.BindUnaryOperation(binder));
            }

            public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
            {
                return AddRestrictions(_metaForwardee.BindBinaryOperation(binder, arg));
            }

            public override DynamicMetaObject BindConvert(ConvertBinder binder)
            {
                return AddRestrictions(_metaForwardee.BindConvert(binder));
            }
        }

        #endregion InterfaceProxyBehavior
    }

    /// <summary>
    /// 一个空的动态对象。
    /// </summary>
    public class Nil : DynamicObject
    {
        private static readonly Nil Singleton = new Nil();

        /// <summary>
        /// 实例。
        /// </summary>
        public static Nil Instance { get { return Singleton; } }

        private Nil()
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Instance;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Instance;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Instance;
            return true;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Equal:
                    result = ReferenceEquals(arg, Instance) || arg == null;
                    return true;

                case ExpressionType.NotEqual:
                    result = !ReferenceEquals(arg, Instance) && arg != null;
                    return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public static bool operator ==(Nil a, Nil b)
        {
            return true;
        }

        public static bool operator !=(Nil a, Nil b)
        {
            return false;
        }

        public static bool operator ==(Nil a, object b)
        {
            return ReferenceEquals(a, b) || b == null;
        }

        public static bool operator !=(Nil a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj == null || ReferenceEquals(obj, Instance);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            return true;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}