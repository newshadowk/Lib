using System;
using System.IO;

namespace Lib.Base
{
    public static class PathExtensions
    {
        private const int FileNameMaxLength = 244;

        private const int FullPathMaxLength = 260;

        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <param name="isSlash">true is /, else is \</param>
        public static string PathCombine(this string s0, string s1, bool isSlash = true)
        {
            if (s0 == null)
                s0 = "";

            if (s1 == null)
                s1 = "";

            if (!s0.EndsWith("/") && !s0.EndsWith("\\") && !s1.StartsWith("/") && !s1.StartsWith("\\"))
            {
                return $"{s0}{(isSlash ? "/" : "\\")}{s1}";
            }

            if ((s0.EndsWith("/") || s0.EndsWith("\\")) &&
                (s1.StartsWith("/") || s1.StartsWith("\\"))
            )
            {
                s0 = s0.TrimEndString(1);
                return s0 + s1;
            }

            return s0 + s1;
        }

        public static string PathCombine(this string s0, string s1, string s2, bool isSlash = true)
        {
            return PathCombine(PathCombine(s0, s1, isSlash), s2, isSlash);
        }

        /// <summary>
        /// d:\1.txt => d:\1 (1).txt
        /// </summary>
        public static string GetNotExistFilePath(this string fullPath)
        {
            if (!File.Exists(fullPath))
                return fullPath;

            for (int i = 1; i < 10000; i++)
            {
                var suffix = $" ({i})";
                var newPath = AddFileNameSuffix(fullPath, suffix);
                if (!File.Exists(newPath))
                    return newPath;
            }

            throw new Exception("Can not get valid Path.");
        }

        /// <summary>
        /// d:\1 => d:\1 (1)
        /// </summary>
        public static string GetNotExistDirPath(this string fullPath)
        {
            if (!Directory.Exists(fullPath))
                return fullPath;

            for (int i = 1; i < 10000; i++)
            {
                var suffix = $" ({i})";
                var newPath = AddDirNameSuffix(fullPath, suffix);
                if (!Directory.Exists(newPath))
                    return newPath;
            }

            throw new Exception("Can not get valid Path.");
        }

        private static string AddSuffix(this string s, int absLength, string suffix)
        {
            var fileName = Path.GetFileName(s);
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            var fullPathMaxLength = FullPathMaxLength - absLength;
            var fileNameMaxLength = FileNameMaxLength - absLength;


            string toFileNameWithoutExtension;
            if (s.Length + suffix.Length > fullPathMaxLength)
            {
                if (s.Length < suffix.Length)
                    throw new Exception("Path is too long.");

                var subLength = s.Length + suffix.Length - fullPathMaxLength;
                toFileNameWithoutExtension = s.Substring(0, s.Length - subLength) + suffix;
            }
            else if (fileName.Length + suffix.Length > fileNameMaxLength)
            {
                if (s.Length < suffix.Length)
                    throw new Exception("Path is too long.");

                var subLength = fileName.Length + suffix.Length - fileNameMaxLength;
                toFileNameWithoutExtension = s.Substring(0, s.Length - subLength) + suffix;
            }
            else
                toFileNameWithoutExtension = s + suffix;

            var directoryName = Path.GetDirectoryName(s);
            if (directoryName == null)
                throw new ArgumentNullException(nameof(directoryName));

            return Path.Combine(directoryName, toFileNameWithoutExtension);
        }

        /// <summary>
        /// c:\11 => c:\11_
        /// </summary>
        public static string AddDirNameSuffix(this string s, string suffix)
        {
            return AddSuffix(s, 0, suffix);
        }

        /// <summary>
        /// c:\1.docx => c:\1_.docx
        /// </summary>
        public static string AddFileNameSuffix(this string s, string suffix)
        {
            var extension = Path.GetExtension(s);
            if (extension == null)
                extension = "";
            var fullPathWithoutExtension = GetFullPathWithoutExtension(s);
            return AddSuffix(fullPathWithoutExtension, extension.Length, suffix) + extension;
        }

        public static string GetFullPathWithoutExtension(this string s)
        {
            var dir = Path.GetDirectoryName(s);
            var name = Path.GetFileNameWithoutExtension(s);
            return Path.Combine(dir, name);
        }
    }
}