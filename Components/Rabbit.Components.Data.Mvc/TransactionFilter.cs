using System.Web.Mvc;
using IFilterProvider = Rabbit.Web.Mvc.Mvc.Filters.IFilterProvider;

namespace Rabbit.Components.Data.Mvc
{
    /// <summary>
    /// 事务过滤器。
    /// </summary>
    internal sealed class TransactionFilter : IFilterProvider, IExceptionFilter
    {
        private readonly ITransactionManager _transactionManager;

        public TransactionFilter(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        #region Implementation of IExceptionFilter

        /// <summary>
        /// 在发生异常时调用。
        /// </summary>
        /// <param name="filterContext">筛选器上下文。</param>
        public void OnException(ExceptionContext filterContext)
        {
            _transactionManager.Cancel();
        }

        #endregion Implementation of IExceptionFilter
    }
}