using System.Collections.Generic;
using System.IO;

namespace Lib.Base
{
    public static class FileScan
    {
        public static List<FileDir> GetFiles(string dirPath)
        {
            List<FileDir> ret = new List<FileDir>();
            GetFiles(dirPath, ret);
            return ret;
        }

        private static void GetFiles(string dirPath, List<FileDir> result)
        {
            try
            {
                foreach (string item in Directory.GetFiles(dirPath))
                {
                    result.Add(new FileDir(item, true));
                }
            }
            catch
            {
                return;
            }

            foreach (string item in Directory.GetDirectories(dirPath))
            {
                result.Add(new FileDir(item, false));
                GetFiles(item, result);
            }
        }
    }
}