using System.Collections.Generic;

namespace Rabbit.Kernel.Caching.Impl
{
    internal sealed class Signals : ISignals
    {
        #region Field

        private readonly IDictionary<object, Token> _tokens = new Dictionary<object, Token>();

        #endregion Field

        #region Implementation of ISignals

        /// <summary>
        /// 触发信号。
        /// </summary>
        /// <typeparam name="T">信号类型。</typeparam>
        /// <param name="signal">信号值。</param>
        public void Trigger<T>(T signal)
        {
            lock (_tokens)
            {
                Token token;
                if (!_tokens.TryGetValue(signal, out token))
                    return;
                _tokens.Remove(signal);
                token.Trigger();
            }
        }

        /// <summary>
        /// 根据 <paramref name="signal"/> 获取挥发令牌。
        /// </summary>
        /// <typeparam name="T">信号类型。</typeparam>
        /// <param name="signal">信号值。</param>
        /// <returns>挥发令牌。</returns>
        public IVolatileToken When<T>(T signal)
        {
            lock (_tokens)
            {
                Token token;
                if (_tokens.TryGetValue(signal, out token))
                    return token;
                token = new Token();
                _tokens[signal] = token;
                return token;
            }
        }

        #endregion Implementation of ISignals

        #region Help Class

        /// <summary>
        /// 令牌。
        /// </summary>
        private class Token : IVolatileToken
        {
            public Token()
            {
                IsCurrent = true;
            }

            public bool IsCurrent { get; private set; }

            public void Trigger()
            {
                IsCurrent = false;
            }
        }

        #endregion Help Class
    }
}