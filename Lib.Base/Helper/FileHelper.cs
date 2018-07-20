using System;
using System.IO;
using System.Reflection;

namespace Lib.Base
{
    public static class FileHelper
    {
        public static string GetFileNameIfFileExist(string fullPath)
        {
            int count = 1;
            string targetFullPath = fullPath;
            string fileNameWithoutExtension = GetFirstFileNameWithoutExtension(fullPath);
            string extension = GetExtensionWithoutFirstFileName(fullPath);
            string path = Path.GetDirectoryName(fullPath);

            while (File.Exists(targetFullPath))
            {
                string newFileNameWithoutExtension = string.Format("{0} ({1})", fileNameWithoutExtension, count);
                string newFileName = string.Format("{0}{1}", newFileNameWithoutExtension, extension);
                targetFullPath = Path.Combine(path, newFileName);
                count++;
            }

            return targetFullPath;
        }

        public static string GetFirstFileNameWithoutExtension(string fullPath)
        {
            string fileName = Path.GetFileName(fullPath);

            if (fileName == null)
                return "";

            if (!fileName.Contains("."))
                return fileName;

            return fileName.Substring(0, fileName.IndexOf(".", StringComparison.Ordinal));
        }

        public static string GetExtensionWithoutFirstFileName(string fullPath)
        {
            var fileName = Path.GetFileName(fullPath);

            if (fileName == null)
                return "";

            if (!fileName.Contains("."))
                return "";

            return fileName.Substring(fileName.IndexOf(".", StringComparison.Ordinal));
        }

        public static void CopyFileFromEmbeddedResource(Assembly assembly, string targetFullPath, string resourceName)
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) 
                return;

            string path = Path.GetDirectoryName(targetFullPath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            stream.Close();

            using (var file = new FileStream(targetFullPath, FileMode.Create))
                file.Write(buffer, 0, buffer.Length);
        }
    }
}