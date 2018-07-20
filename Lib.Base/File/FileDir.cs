namespace Lib.Base
{
    public class FileDir
    {
        public FileDir(string fullPath, bool isFile)
        {
            FullPath = fullPath;
            IsFile = isFile;
        }

        public string FullPath { get; private set; }

        public bool IsFile { get; private set; }

        public override string ToString()
        {
            return string.Format("[{0}], {1}", IsFile ? "File" : "Dir", FullPath);  
        }
    }
}