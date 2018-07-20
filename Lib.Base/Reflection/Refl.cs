using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows;

namespace Lib.Base
{
    public static class Refl
    {
        public static string GetAssemblyPath()
        {
            var assemblypath = Assembly.GetExecutingAssembly().CodeBase;
            assemblypath = assemblypath.Substring(8, assemblypath.Length - 8);
            assemblypath = Path.GetDirectoryName(assemblypath);
            return assemblypath;
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
