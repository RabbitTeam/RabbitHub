using Rabbit.Kernel;
using Rabbit.Kernel.Utility.Extensions;
using System.Text;

namespace Rabbit.Components.Security
{
    /// <summary>
    /// 一个抽象的加密服务。
    /// </summary>
    public interface IEncryptionService : ISingletonDependency
    {
        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="data">需要加密的数据。</param>
        /// <returns>加密后的数据。</returns>
        byte[] Encode(byte[] data);

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="encodedData">需要解密的数据。</param>
        /// <returns>解密后的数据。</returns>
        byte[] Decode(byte[] encodedData);
    }

    /// <summary>
    /// 加密服务扩展方法。
    /// </summary>
    public static class EncryptionServiceExtensions
    {
        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="encryptionService">加密服务。</param>
        /// <param name="text">需要加密的文本。</param>
        /// <returns>加密后的文本。</returns>
        public static string Encode(this IEncryptionService encryptionService, string text)
        {
            var bytes = encryptionService.NotNull("encryptionService").Encode(Encoding.UTF8.GetBytes(text));
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="encryptionService">加密服务。</param>
        /// <param name="text">需要解密的文本。</param>
        /// <returns>解密后的文本。</returns>
        public static string Decode(this IEncryptionService encryptionService, string text)
        {
            var bytes = encryptionService.NotNull("encryptionService").Decode(Encoding.UTF8.GetBytes(text));
            return Encoding.UTF8.GetString(bytes);
        }
    }
}