using Rabbit.Web.Mvc.DisplayManagement.Shapes;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rabbit.Web.Mvc.DisplayManagement.Implementation
{
    /// <summary>
    /// 显示助手。
    /// </summary>
    public sealed class DisplayHelper : DynamicObject
    {
        #region Field

        private readonly IDisplayManager _displayManager;
        private readonly IShapeFactory _shapeFactory;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的显示助手。
        /// </summary>
        /// <param name="displayManager">显示管理者。</param>
        /// <param name="shapeFactory">形状工厂。</param>
        /// <param name="viewContext">视图上下文。</param>
        /// <param name="viewDataContainer">视图数据容器。</param>
        public DisplayHelper(
            IDisplayManager displayManager,
            IShapeFactory shapeFactory,
            ViewContext viewContext,
            IViewDataContainer viewDataContainer)
        {
            _displayManager = displayManager;
            _shapeFactory = shapeFactory;
            ViewContext = viewContext;
            ViewDataContainer = viewDataContainer;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 视图上下文。
        /// </summary>
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// 视图数据容器。
        /// </summary>
        public IViewDataContainer ViewDataContainer { get; set; }

        #endregion Property

        #region Overrides of DynamicObject

        /// <summary>
        /// 为调用对象的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如调用对象或委托这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关调用操作的信息。</param><param name="args">调用操作期间传递给对象的参数。例如，对于 sampleObject(100) 操作（其中 sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类），<paramref name="args"/> 等于 100。</param><param name="result">对象调用的结果。</param>
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = Invoke(null, Arguments.From(args, binder.CallInfo.ArgumentNames));
            return true;
        }

        /// <summary>
        /// 为调用成员的操作提供实现。从 <see cref="T:System.Dynamic.DynamicObject"/> 类派生的类可以重写此方法，以便为诸如调用方法这样的操作指定动态行为。
        /// </summary>
        /// <returns>
        /// 如果此操作成功，则为 true；否则为 false。如果此方法返回 false，则该语言的运行时联编程序将决定行为。（大多数情况下，将引发语言特定的运行时异常。）
        /// </returns>
        /// <param name="binder">提供有关动态操作的信息。binder.Name 属性提供针对其执行动态操作的成员的名称。例如，对于语句 sampleObject.SampleMethod(100)（其中 sampleObject 是派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类的类的一个实例），binder.Name 将返回“SampleMethod”。binder.IgnoreCase 属性指定成员名称是否区分大小写。</param><param name="args">调用操作期间传递给对象成员的参数。例如，对于语句 sampleObject.SampleMethod(100)（其中 sampleObject 派生自 <see cref="T:System.Dynamic.DynamicObject"/> 类），<paramref name="args"/> 等于 100。</param><param name="result">成员调用的结果。</param>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Invoke(binder.Name, Arguments.From(args, binder.CallInfo.ArgumentNames));
            return true;
        }

        #endregion Overrides of DynamicObject

        #region Public Method

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="parameters">参数。</param>
        /// <returns>结果。</returns>
        public object Invoke(string name, INamedEnumerable<object> parameters)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return ShapeTypeExecute(name, parameters);
            }

            if (parameters.Positional.Count() == 1)
            {
                return ShapeExecute(parameters.Positional.Single());
            }

            return parameters.Positional.Any() ? new Combined(ShapeExecute(parameters.Positional)) : null;
        }

        /// <summary>
        /// 执行形状。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>结果。</returns>
        public object ShapeExecute(Shape shape)
        {
            return ShapeExecute((object)shape);
        }

        /// <summary>
        /// 执行形状。
        /// </summary>
        /// <param name="shape">形状。</param>
        /// <returns>形状结果。</returns>
        public object ShapeExecute(object shape)
        {
            if (shape == null)
            {
                return new HtmlString(string.Empty);
            }

            var context = new DisplayContext { Display = this, Value = shape, ViewContext = ViewContext, ViewDataContainer = ViewDataContainer };
            return _displayManager.Execute(context);
        }

        /// <summary>
        /// 执行形状。
        /// </summary>
        /// <param name="shapes">形状集合。</param>
        /// <returns>结果集合。</returns>
        public IEnumerable<object> ShapeExecute(IEnumerable<object> shapes)
        {
            return shapes.Select(ShapeExecute).ToArray();
        }

        #endregion Public Method

        #region Private Method

        private object ShapeTypeExecute(string name, INamedEnumerable<object> parameters)
        {
            var shape = _shapeFactory.Create(name, parameters);
            return ShapeExecute(shape);
        }

        #endregion Private Method

        #region Help Class

        private sealed class Combined : IHtmlString
        {
            private readonly IEnumerable<object> _fragments;

            public Combined(IEnumerable<object> fragments)
            {
                _fragments = fragments;
            }

            public string ToHtmlString()
            {
                return _fragments.Aggregate("", (a, b) => a + b);
            }

            public override string ToString()
            {
                return ToHtmlString();
            }
        }

        #endregion Help Class
    }
}