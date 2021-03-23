using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Magician.Common.Util
{
    public class EncryptUtil
    {
        private const string DEFAULT_ENCRYPT_STR = "It's a secretKey";

        /// <summary>
        /// 获取md5字符串(小写)
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="encoding" cref="StringUtil.DefaultEncoding">编码，默认使用StringUtil.DefaultEncoding</param>
        /// <returns>md5字符串</returns>
        public static string GetMd5(string str, Encoding encoding = null)
        {
            var output = GetMd5Buffer(str, encoding);
            var sb = new StringBuilder();
            foreach (var t in output)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取md5字符串(小写)
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>md5字符串</returns>
        public static string GetMd5(Stream stream)
        {
            var output = GetMd5Buffer(stream);
            var sb = new StringBuilder();
            foreach (var t in output)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取base64编码的md5加密串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="encoding" cref="StringUtil.DefaultEncoding">编码，默认使用StringUtil.DefaultEncoding</param>
        /// <returns>base64编码串</returns>
        public static string GetMd5Base64(string str, Encoding encoding = null)
        {
            return Convert.ToBase64String(GetMd5Buffer(str, encoding));
        }

        /// <summary>
        /// 获取base64编码的md5加密串
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>base64编码串</returns>
        public static string GetMd5Base64(Stream stream)
        {
            return Convert.ToBase64String(GetMd5Buffer(stream));
        }

        /// <summary>
        /// 获取md5加密字符数组
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="encoding" cref="StringUtil.DefaultEncoding">编码，默认使用StringUtil.DefaultEncoding</param>
        /// <returns>md5加密字符数组</returns>
        private static byte[] GetMd5Buffer(string str, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = StringUtil.DefaultEncoding;
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(encoding.GetBytes(str));
        }

        /// <summary>
        /// 获取md5加密字符数组
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>md5加密字符数组</returns>
        private static byte[] GetMd5Buffer(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(stream);
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key = DEFAULT_ENCRYPT_STR)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var toEncryptArray = Encoding.UTF8.GetBytes(str);

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">密文</param>
        /// <param name="key">明文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key = DEFAULT_ENCRYPT_STR)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var toEncryptArray = Convert.FromBase64String(str);

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
