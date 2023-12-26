using System;
using System.Diagnostics;
using System.IO;

namespace OverhaulMod.Utils
{
    internal static class ModIOUtils
    {
        public static void WriteText(string @string, string path)
        {
            FileMode fileMode = File.Exists(path) ? FileMode.OpenOrCreate | FileMode.Truncate : FileMode.Create;
            using (FileStream fileStream = new FileStream(path, fileMode, FileAccess.Write))
            {
                byte[] bytes = GetBytes(@string);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static void WriteBytes(byte[] bytes, string path)
        {
            FileMode fileMode = File.Exists(path) ? FileMode.OpenOrCreate | FileMode.Truncate : FileMode.Create;
            using (FileStream fileStream = new FileStream(path, fileMode, FileAccess.Write))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public static string ReadText(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fileStream.Length];
                _ = fileStream.Read(bytes, 0, bytes.Length);
                return GetString(bytes);
            }
        }

        public static byte[] ReadBytes(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fileStream.Length];
                _ = fileStream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static string TryReadText(string path, out Exception exception)
        {
            exception = null;
            try
            {
                string content = ReadText(path);
                return content;
            }
            catch (Exception exc)
            {
                exception = exc;
                return string.Empty;
            }
        }

        public static void CreateFolderIfNotCreated(string path)
        {
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }
        }

        public static bool OpenFileExplorer(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            _ = Process.Start(path);
            return true;
        }

        public static byte[] GetBytes(string @string)
        {
            return ModCache.utf8Encoding.GetBytes(@string);
        }

        public static string GetString(byte[] bytes)
        {
            return ModCache.utf8Encoding.GetString(bytes);
        }
    }
}
