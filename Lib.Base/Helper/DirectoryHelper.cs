using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lib.Base
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="dstPath"></param>
        public static bool CopyFolder(string srcPath, string dstPath)
        {
            try
            {
                if (!Directory.Exists(dstPath))
                {
                    Directory.CreateDirectory(dstPath);
                }

                DirectoryInfo sDir = new DirectoryInfo(srcPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(dstPath + "\\" + file.Name, true);
                }

                new DirectoryInfo(dstPath);
                DirectoryInfo[] subDirArray = sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, dstPath + "\\" + subDir.Name);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CreateFolder(string folderPath, bool isDeleteIfHas)
        {
            if (Directory.Exists(folderPath))
            {

            }
        }
    }
}
