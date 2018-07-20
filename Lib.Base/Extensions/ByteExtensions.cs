using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class ByteExtensions
    {
        #region 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// （，）
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> bytes)
        {
            return ToHex(bytes, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> bytes, string split)
        {
            var isNull = split.IsNullOrEmpty();
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
                if (!isNull)
                {
                    sb.Append(split);
                }
            }
            return isNull
                       ? sb.ToString()
                       : sb.ToString().DelLastChar(split);
        }

        /// <summary>
        /// 16
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static byte[] HexStrToBytes(this string hexString, string split)
        {
            hexString = hexString.Replace(split, "");

            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];

            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;
        }

        #endregion

        #region Hash

        /// <summary>
        /// Hash
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hashName"></param>
        /// <returns></returns>
        public static byte[] Hash(this byte[] data, string hashName = null)
        {
            var algorithm = hashName.IsNullOrEmpty()
                                ? HashAlgorithm.Create()
                                : HashAlgorithm.Create(hashName);
            return algorithm.ComputeHash(data);
        }

        #endregion

        #region 

        /// <summary>
        /// index0
        /// index1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBit(this byte b, int index)
        {
            return (b & (1 << index)) > 0;
        }

        /// <summary>
        /// index1
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte SetBit(this byte b, int index)
        {
            b |= (byte)(1 << index);
            return b;
        }

        /// <summary>
        /// index0
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte ClearBit(this byte b, int index)
        {
            b &= (byte)((1 << 8) - 1 - (1 << index));
            return b;
        }

        /// <summary>
        /// index
        /// </summary>
        /// <param name="b"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static byte ReverseBit(this byte b, int index)
        {
            b ^= (byte)(1 << index);
            return b;
        }

        #endregion

        #region 

        /// <summary>
        /// bytes。
        /// </summary>
        public static byte[] ToBytes(this string str, int size, Encoding encoding)
        {
            var bytes = encoding.GetBytes(str);
            byte[] ret = new byte[size];
            bytes.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// bytes。
        /// </summary>
        public static byte[] ToBytes(this string str, int size)
        {
            return str.ToBytes(size, Encoding.Default);
        }

        /// <summary>
        /// bytes。
        /// </summary>
        public static byte[] ToBytesUTF8(this string str, int size)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            byte[] ret = new byte[size];
            bytes.CopyTo(ret, 0);
            return ret;
        }

        /// <summary>
        /// Base64
        /// </summary>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToDefaultCodingString(this byte[] bytes)
        {
            string s = Encoding.Default.GetString(bytes);
            return s.TrimEndZero();
        }

        public static string ToDefaultCodingString(this ushort[] bytes)
        {
            byte[] bs = new byte[bytes.Length];
            bytes.CopyTo(bs, 0);
            string s = Encoding.Default.GetString(bs);
            return s.TrimEndZero();
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decode(this byte[] data, Encoding encoding)
        {
            return encoding.GetString(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(this byte[] data)
        {
            return new MemoryStream(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Int16 ConvertInt16(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 2);
            return BitConverter.ToInt16(datas, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Int32 ConvertInt32(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 4);
            return BitConverter.ToInt32(datas, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Int64 ConvertInt64(this byte[] data)
        {
            var datas = ConvertBytesToLength(data, 8);
            return BitConverter.ToInt64(datas, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataSize"></param>
        /// <returns></returns>
        private static byte[] ConvertBytesToLength(byte[] data, int dataSize)
        {
            if (data.Length == dataSize)
                return data;
            var bytes = new byte[dataSize];
            for (int i = 0; i < dataSize; i++)
            {
                if (data.Length == i + 1)
                {
                    bytes[i] = data[i];
                }
                else
                {
                    bytes[i] = 0;
                }
            }
            return bytes;
        }

        #endregion

        #region 

        public static byte[] ToBytes(this object obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                return stream.ToArray();
            }
        }

        public static TObject ToObject<TObject>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes, 0, bytes.Length, false))
            {
                var formatter = new BinaryFormatter();
                var data = formatter.Deserialize(stream);
                stream.Flush();
                return (TObject)data;
            }
        }

        #endregion

        #region 

        /// <summary>
        /// byte
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns>byte</returns>
        public static byte[] StructToBytes<TObject>(this TObject structObj) where TObject : struct
        {
            //
            int size = Marshal.SizeOf(structObj);
            //byte
            byte[] bytes = new byte[size];
            //
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //
            Marshal.StructureToPtr(structObj, structPtr, false);
            //byte
            Marshal.Copy(structPtr, bytes, 0, size);
            //
            Marshal.FreeHGlobal(structPtr);
            //byte
            return bytes;
        }

        /// <summary>
        /// byte
        /// </summary>
        /// <param name="bytes">byte</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object BytesToStuct(this byte[] bytes, Type type)
        {
            //
            int size = Marshal.SizeOf(type);
            //byte
            if (size > bytes.Length)
            {
                //
                return null;
            }
            //
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //byte
            Marshal.Copy(bytes, 0, structPtr, size);
            //
            object obj = Marshal.PtrToStructure(structPtr, type);
            //
            Marshal.FreeHGlobal(structPtr);
            //
            return obj;
        }

        #endregion

        #region 

        /// <summary>
        /// bytebyte。
        /// </summary>
        /// <param name="src">。</param>
        /// <param name="target">。</param>
        /// <param name="srcIndex">。</param>
        /// <returns>。</returns>
        public static void CopyIndex(this byte[] src, byte[] target, long srcIndex)
        {
            int p = 0;
            for (long i = srcIndex; i < src.Length; i++)
            {
                target[p] = src[i];
                p++;
            }
        }

        #endregion
    }
}