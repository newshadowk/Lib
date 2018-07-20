using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable CSharpWarnings::CS1591
using System;
using System.Windows;

namespace Lib.Base
{
    public static class OtherExtensions
    {
        /// <summary>
        /// Get the description from enum.
        /// </summary>
        public static string ToStringDes(this Enum enumItem)
        {
            return EnumHelper.GetStringFromEnum(enumItem);
        }

        public static double DistanceFromPoint(this Point fromPoint, Point toPoint)
        {
            double value = Math.Sqrt(Math.Abs(fromPoint.X - toPoint.X)*Math.Abs(fromPoint.X - toPoint.X) + Math.Abs(fromPoint.Y - toPoint.Y)*Math.Abs(fromPoint.Y - toPoint.Y));
            return value;
        }

        public static string ToSegmentIp(this string ip, int segmentCount)
        {
            string ret = "";
            var segs = ip.Split('.');
            for (int i = 0; i < segmentCount; i++)
                ret += segs[i];

            return ret;
        }

        public static IAsyncResult AsApm<T>(this Task<T> task, AsyncCallback callback, object state)
        {
            if (task == null) throw new ArgumentNullException("task");
            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    if (t.Exception != null)
                        tcs.TrySetException(t.Exception.InnerExceptions);
                }
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);
                if (callback != null) callback(tcs.Task);
            });
            return tcs.Task;
        }

        public static bool IsIp(this string s)
        {
            IPAddress address;
            return IPAddress.TryParse(s, out address);
        }

        public static string ToLast4Char(this Guid guid)
        {
            var s = guid.ToString();
            return guid.ToString().Substring(s.Length - 4);
        }

        public static byte[] GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            return retVal;
        }

        public static int GetProgress(object o1, object o2)
        {
            var d = Convert.ToDouble(o1) / Convert.ToDouble(o2);
            return (int)(d * 100);
        }
    }
}