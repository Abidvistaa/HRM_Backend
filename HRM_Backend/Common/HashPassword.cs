using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HRM_Backend.Common
{
    public class HashPassword
    {
        private const string SecurityKey = "ComplexKeyHere";
        public string Encode(string password)
        {
            byte[] toEncrypt = Encoding.UTF8.GetBytes(password);
            byte[] key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));

            using var tdes = TripleDES.Create();
            tdes.Key = key;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            using var transform = tdes.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

            return Convert.ToBase64String(result);
        }
        public string Decode(string encodedPassword)
        {
            byte[] toDecrypt = Convert.FromBase64String(encodedPassword);
            byte[] key = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));

            using var tdes = TripleDES.Create();
            tdes.Key = key;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            using var transform = tdes.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);

            return Encoding.UTF8.GetString(result);
        }

    }
}
