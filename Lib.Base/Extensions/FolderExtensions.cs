
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class FolderExtensions
    {
        /// <summary>
        /// (),Size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ConvertSize(this long size)
        {
            var mStrSize = "";
            var factSize = size;
            if (factSize < 1024.00)
                mStrSize = factSize.ToString("F2") + " Byte";
            else if (factSize >= 1024.00 && factSize < 1048576)
                mStrSize = (factSize / 1024.00).ToString("F2") + " K";
            else if (factSize >= 1048576 && factSize < 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (factSize >= 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return mStrSize;
        }

        /// <summary>
        /// b -> M
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double ConvertBytesToGegabytes(this long bytes)
        {
            return (bytes / 1024f) / 1024f / 1024f;
        }

        /// <summary>
        /// kb-> M
        /// </summary>
        /// <param name="kilobytes"></param>
        /// <returns></returns>
        public static double ConvertKilobytesToMegabytes(this long kilobytes)
        {
            return kilobytes / 1024f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static long GetDirectoryLength(this string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return 0;
            var di = new DirectoryInfo(dirPath);
            var len = di.GetFiles().Sum(fi => fi.Length);
            var dis = di.GetDirectories();
            if (dis.Length > 0)
                len += dis.Sum(t => GetDirectoryLength(t.FullName));
            return len;
        }

        /// <summary>
        /// ,
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="files"></param>
        /// <param name="recursive"> path 、， true； false。</param>
        /// <returns></returns>
        public static long GetDirectoryLength(this string dirPath, ref List<string> files, bool recursive = false)
        {
            if (!Directory.Exists(dirPath))
                return 0;
            var di = new DirectoryInfo(dirPath);
            var fileInfos = di.GetFiles();
            long len = 0;
            foreach (var fileInfo in fileInfos)
            {
                files.Add(fileInfo.FullName);
                len += fileInfo.Length;
            }
            if (recursive)
            {
                var dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    foreach (var directoryInfo in dis)
                    {
                        len += GetDirectoryLength(directoryInfo.FullName, ref files, recursive);
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void OpenExplorer(this string path)
        {
            Process.Start("explorer.exe", path);
        }

        /// <summary>
        /// /// 
        /// </summary>
        /// <param name="path"></param>
        public static void OpenExplorerWithSelectFileOrFolder(this string path)
        {
            var proc = new Process();
            proc.StartInfo.FileName = "explorer";
            proc.StartInfo.Arguments = @"/select," + path;
            proc.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>true；False</returns>
        public static bool ValidateFileName(this string filename)
        {
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                return false;
            return true;
        }


    }
}
