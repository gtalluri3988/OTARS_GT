using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace MTAoarsGeneral.Utilities.Extensions
{
    public class EncryptionManager
    {
        private byte[] key = { };
        private byte[] IV = {
            0x12,
            0x34,
            0x56,
            0x78,
            0x90,
            0xab,
            0xcd,
            0xef
        };

        public string Decrypt(string stringToDecrypt)
        {
            return Decrypt(stringToDecrypt, false);
        }
        public string Decrypt(string stringToDecrypt, bool isDecoded)
        {
            return Decrypt(stringToDecrypt, "a@B#c$90", isDecoded);
        }
        public string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            return Decrypt(stringToDecrypt, sEncryptionKey, false);
        }

        public string Decrypt(string stringToDecrypt, string sEncryptionKey, bool isDecoded)
        {
            if (string.IsNullOrEmpty(stringToDecrypt))
                return stringToDecrypt;

            if (isDecoded)
                stringToDecrypt = HttpUtility.UrlDecode(stringToDecrypt);

            byte[] inputByteArray = new byte[stringToDecrypt.Length + 1];
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace("_", "/").Replace("-", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string Encrypt(string stringToEncrypt)
        {
            return Encrypt(stringToEncrypt, false);
        }
        public string Encrypt(string stringToEncrypt, bool isEncoded)
        {
            return Encrypt(stringToEncrypt, "a@B#c$90", isEncoded);
        }
        public string Encrypt(string stringToEncrypt, string sEncryptionKey)
        {
            return Encrypt(stringToEncrypt, sEncryptionKey, false);
        }
        public string Encrypt(string stringToEncrypt, string sEncryptionKey, bool isEncoded)
        {
            try
            {
                if (stringToEncrypt == null)
                    return null;

                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                if (isEncoded)
                    return HttpUtility.UrlEncode(Convert.ToBase64String(ms.ToArray()).Replace("/", "_").Replace("+", "-"));
                else
                    return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}
