using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AlchemistHan.Services
{
    public sealed class Runtime
    {
        static Runtime()
        {
        }

        public static byte[] AesDecrypt(byte[] key, byte[] data)
        {
            var iv = data.Take(16).ToArray();
            var se = data.Skip(16).ToArray();
            
            RijndaelManaged rDel = new RijndaelManaged { KeySize = 0x80, BlockSize = 0x80, Key = key, IV = iv, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
            var resultArray = rDel.CreateDecryptor().TransformFinalBlock(se, 0, se.Length);
            return resultArray;
        }

        public static byte[] GetEncryptionApp(string acion)
        {
            var app = new byte[] { 0x5F, 0x3E, 0x18, 0xC9, 0xC7, 0xD7, 0x43, 0xE8, 0xC7, 0x0B, 0x55, 0xDD, 0xED, 0xC8, 0x3B, 0xC9 };
            var ac = Encoding.ASCII.GetBytes(acion);
            var ha = app.Concat(ac).ToArray();
            return new SHA256Managed().ComputeHash(ha).Take(16).ToArray();
        }
    }
}
