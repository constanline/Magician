using System;
using System.IO;
using System.Text;

namespace Magician.Common.Util
{
    public class FileUtil
    {
        public static string GetFileNameNoPath(string path)
        {
            var idx = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal),
                path.LastIndexOf("\\", StringComparison.Ordinal));

            return path.Substring(idx);
        }

        public static string GetFileDirectory(string path)
        {
            var idx = Math.Max(path.LastIndexOf("/", StringComparison.Ordinal),
                path.LastIndexOf("\\", StringComparison.Ordinal));
            return idx <= -1 ? null : path.Substring(0, idx);
        }

        private static void CreateDirectoryByFilePath(string path)
        {
            var dic = GetFileDirectory(path);
            if (!string.IsNullOrEmpty(dic) && !Directory.Exists(dic)) Directory.CreateDirectory(dic);
        }
        public static void Write(string path, byte[] buffer, FileMode mode = FileMode.Append, int offset = 0,
            int count = -1)
        {
            CreateDirectoryByFilePath(path);

            var fs = !File.Exists(path) ? File.Create(path) : new FileStream(path, mode, FileAccess.Write);
            if (count == -1) count = buffer.Length;
            fs.Write(buffer, offset, count);
            fs.Close();
        }

        public static void Write(string path, string info, Encoding encoding = null, FileMode mode = FileMode.Append)
        {
            if (encoding == null) encoding = StringUtil.DefaultEncoding;
            var buffer = encoding.GetBytes(info);
            Write(path, buffer, mode);
        }

        public static byte[] Read(string path, bool isCreate = false, int offset = 0, int count = -1)
        {
            FileStream fs;
            if (!File.Exists(path))
            {
                if (isCreate)
                {
                    CreateDirectoryByFilePath(path);
                    fs = File.Create(path);
                    fs.Close();
                }
                return new byte[] { };
            }
            
            fs = File.OpenRead(path);
            if (count < 0 || count < fs.Length) count = Convert.ToInt32(fs.Length);
            var buffer = new byte[count];
            fs.Read(buffer, offset, count);
            fs.Close();
            return buffer;
        }
    }
}