using System;
using System.Security.Cryptography;
using System.Text;

namespace Lib.Base
{
    public static class RandomHelper
    {
        public static int NewRandomSeed()
        {
            byte[] bytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}