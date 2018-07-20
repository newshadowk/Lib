using System;
using System.Security.Cryptography;
using System.Text;

namespace Lib.Base
{
    public static class DEncrypt
    {
        private const string DefaultKey = "E53E692A-D0F6-47DA-A9DD-8AB1D9CAF535";

        public static string Encrypt(string original, string key = DefaultKey)
        {
            if (key == null)
                key = DefaultKey;

            byte[] buff = Encoding.UTF8.GetBytes(original);
            byte[] kb = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(Encrypt(buff, kb));
        }

        public static string Decrypt(string encrypted, string key = DefaultKey)
        {
            if (key == null)
                key = DefaultKey;

            byte[] buff = Convert.FromBase64String(encrypted);
            byte[] kb = Encoding.UTF8.GetBytes(key);
            return Encoding.UTF8.GetString(Decrypt(buff, kb));
        }

        public static byte[] Encrypt(byte[] original, string key = DefaultKey)
        {
            if (key == null)
                key = DefaultKey;

            byte[] kb = Encoding.UTF8.GetBytes(key);
            return Encrypt(original, kb);
        }

        public static byte[] Decrypt(byte[] encrypted, string key = DefaultKey)
        {
            if (key == null)
                key = DefaultKey;

            byte[] kb = Encoding.UTF8.GetBytes(key);
            return Decrypt(encrypted, kb);
        }

        public static byte[] EncryptSHA256(string original)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(original);
            using (HashAlgorithm hash = new SHA256Managed())
            {
                byte[] hashBytes = hash.ComputeHash(plainTextBytes);
                return hashBytes;
            }
        }

        private static byte[] CreateMD5(byte[] original)
        {
            using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
            {
                byte[] keyhash = hashmd5.ComputeHash(original);
                return keyhash;
            }
        }

        private static byte[] Encrypt(byte[] original, byte[] key)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = CreateMD5(key);
                des.Mode = CipherMode.ECB;
                return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
            }
        }

        private static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            using (TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider())
            {
                des.Key = CreateMD5(key);
                des.Mode = CipherMode.ECB;
                return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
            }
        }

        public static string CreateMD5(string encypStr)
        {
            using (MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider())
            {
                var inputBye = Encoding.UTF8.GetBytes(encypStr);
                var outputBye = m5.ComputeHash(inputBye);
                var retStr = BitConverter.ToString(outputBye);
                retStr = retStr.Replace("-", "").ToUpper();
                return retStr;
            }
        }
    }
}