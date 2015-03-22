using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Rabbit.Kernel.Exceptions
{
    /// <summary>
    /// 异常扩展方法。
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 是否是一个致命的异常。
        /// </summary>
        /// <param name="exception">异常信息。</param>
        /// <returns>如果是致命异常则返回true，否则返回false。</returns>
        public static bool IsFatal(this Exception exception)
        {
            return exception is StackOverflowException ||
                exception is OutOfMemoryException ||
                exception is AccessViolationException ||
                exception is AppDomainUnloadedException ||
                exception is ThreadAbortException ||
                exception is SecurityException ||
                exception is SEHException;
        }
    }
}