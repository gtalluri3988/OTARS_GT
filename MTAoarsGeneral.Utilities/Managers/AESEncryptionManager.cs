using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MTAoarsGeneral.Utilities.Managers
{
    public static class AESEncryptionManagerExtension
    {
        public static string AESEncrypt(this object item)
        {
            return new AESEncryptionManager().Encrypt(item);
        }
        public static string AESEncrypt(this object item, bool isEncoded)
        {
            return new AESEncryptionManager().Encrypt(item, isEncoded);
        }
        public static string AESEncrypt(this object item, string sEncryptionKey)
        {
            return new AESEncryptionManager().Encrypt(item, sEncryptionKey);
        }

        public static string AESDecrypt(this string item)
        {
            return new AESEncryptionManager().Decrypt(item);
        }
        public static string AESDecrypt(this string item, bool isDecoded)
        {
            return new AESEncryptionManager().Decrypt(item, isDecoded);
        }
    }

    public class AESEncryptionManager
    {
        private static readonly string encryptionKey = "cm9#kdWN0%X2lkIjoiO@DQw";
        private static readonly byte[] IV = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        private static byte[] GetKey(string encryptionKey)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
            var plainTextBase64 = Convert.ToBase64String(plainTextBytes);
            return Encoding.UTF8.GetBytes(plainTextBase64);
        }

        public string Decrypt(string stringToDecrypt)
        {
            return Decrypt(stringToDecrypt, false);
        }
        public string Decrypt(string stringToDecrypt, bool isDecoded)
        {
            return Decrypt(stringToDecrypt, encryptionKey, isDecoded);
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
            string plaintext = null;

            try
            {
                inputByteArray = Convert.FromBase64String(stringToDecrypt.Replace("_", "/").Replace("-", "+"));
                using (Aes aesAlg = GetAes(sEncryptionKey))
                {
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(inputByteArray))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Encrypt(object stringToEncrypt)
        {
            return Encrypt(stringToEncrypt, false);
        }
        public string Encrypt(object stringToEncrypt, bool isEncoded)
        {
            return Encrypt(stringToEncrypt, encryptionKey, isEncoded);
        }
        public string Encrypt(object stringToEncrypt, string sEncryptionKey)
        {
            return Encrypt(stringToEncrypt, sEncryptionKey, false);
        }
        public string Encrypt(object stringToEncrypt, string sEncryptionKey, bool isEncoded)
        {
            if (stringToEncrypt == null)
                return null;

            try
            {
                byte[] encrypted;
                using (Aes aesAlg = GetAes(sEncryptionKey))
                {
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(stringToEncrypt);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                if (isEncoded)
                    return HttpUtility.UrlEncode(Convert.ToBase64String(encrypted.ToArray()).Replace("/", "_").Replace("+", "-"));
                else
                    return Convert.ToBase64String(encrypted.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public Aes GetAes(string sEncryptionKey)
        {
            var aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.BlockSize = 128;
            aesAlg.KeySize = 256;
            aesAlg.Key = GetKey(sEncryptionKey);
            aesAlg.IV = IV;

            return aesAlg;
        }
    }
}
