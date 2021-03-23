using System;
using System.Text;

namespace Magician.Common.Util
{
    internal class StringUtil
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private static readonly char[] randomCharPool =
            "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly Random rnd = new Random();

        public static string GetRandomStr(int length)
        {
            if (length < 1) length = 6;
            var poolLen = randomCharPool.Length;
            var sb = new StringBuilder(length);
            for (var i = 0; i < length; i++) sb.Append(randomCharPool[rnd.Next(poolLen)]);
            return sb.ToString();
        }

        public static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null) return null;
            var ret = new StringBuilder();
            foreach (var b in bytes) ret.AppendFormat("{0:x2}", b);

            return ret.ToString();
        }

        public static byte[] HexStringToBytes(string hs)
        {
            if (string.IsNullOrEmpty(hs)) return null;
            var bytes = new byte[hs.Length / 2];
            for (var i = 0; i < hs.Length; i++) bytes[i] = Convert.ToByte(hs[i].ToString(), 16);

            return bytes;
        }

        public static string StringToHexString(string s, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;
            return BytesToHexString(encoding.GetBytes(s));
        }

        public static string HexStringToString(string hs, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;
            return encoding.GetString(HexStringToBytes(hs));
        }
    }
}