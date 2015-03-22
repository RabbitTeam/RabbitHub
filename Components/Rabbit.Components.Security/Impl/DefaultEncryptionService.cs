using Rabbit.Kernel.Environment.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Rabbit.Components.Security.Impl
{
    internal sealed class DefaultEncryptionService : IEncryptionService
    {
        private readonly ShellSettings _shellSettings;

        public DefaultEncryptionService(ShellSettings shellSettings)
        {
            _shellSettings = shellSettings;
        }

        #region Implementation of IEncryptionService

        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="data">需要加密的数据。</param>
        /// <returns>加密后的数据。</returns>
        public byte[] Encode(byte[] data)
        {
            byte[] encryptedData;
            byte[] iv;

            using (var ms = new MemoryStream())
            {
                using (var symmetricAlgorithm = CreateSymmetricAlgorithm())
                {
                    symmetricAlgorithm.GenerateIV();
                    iv = symmetricAlgorithm.IV;

                    using (var cs = new CryptoStream(ms, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                    }

                    encryptedData = ms.ToArray();
                }
            }

            byte[] signedData;

            using (var hashAlgorithm = CreateHashAlgorithm())
            {
                signedData = hashAlgorithm.ComputeHash(iv.Concat(encryptedData).ToArray());
            }

            return iv.Concat(encryptedData).Concat(signedData).ToArray();
        }

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="encodedData">需要解密的数据。</param>
        /// <returns>解密后的数据。</returns>
        public byte[] Decode(byte[] encodedData)
        {
            using (var symmetricAlgorithm = CreateSymmetricAlgorithm())
            {
                using (var hashAlgorithm = CreateHashAlgorithm())
                {
                    var iv = new byte[symmetricAlgorithm.BlockSize / 8];
                    var signature = new byte[hashAlgorithm.HashSize / 8];
                    var data = new byte[encodedData.Length - iv.Length - signature.Length];

                    Array.Copy(encodedData, 0, iv, 0, iv.Length);
                    Array.Copy(encodedData, iv.Length, data, 0, data.Length);
                    Array.Copy(encodedData, iv.Length + data.Length, signature, 0, signature.Length);

                    //验证签名。
                    var mac = hashAlgorithm.ComputeHash(iv.Concat(data).ToArray());

                    if (!mac.SequenceEqual(signature))
                    {
                        //消息被串改。
                        throw new ArgumentException();
                    }

                    symmetricAlgorithm.IV = iv;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        #endregion Implementation of IEncryptionService

        #region Private Method

        private SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            var encryptionAlgorithm = _shellSettings["EncryptionAlgorithm"];
            var encryptionKey = _shellSettings["EncryptionKey"];

            var algorithm = SymmetricAlgorithm.Create(encryptionAlgorithm);
            algorithm.Key = ToByteArray(encryptionKey);
            return algorithm;
        }

        private HMAC CreateHashAlgorithm()
        {
            var hashAlgorithm = _shellSettings["HashAlgorithm"];
            var hashKey = _shellSettings["HashKey"];
            var algorithm = HMAC.Create(hashAlgorithm);
            algorithm.Key = ToByteArray(hashKey);
            return algorithm;
        }

        private static byte[] ToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).
                Where(x => 0 == x % 2).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                ToArray();
        }

        #endregion Private Method
    }
}