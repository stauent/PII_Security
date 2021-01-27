using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PII_Security
{

    /// <summary>
    /// Provides encryption/decryption support for database column data.
    /// NOTE: Database column must be of type varbinary[] . It is up
    ///       to the database designer to make the column large enough to
    ///       contain the encrypted data. varbinary[MAX] will allow the largest
    ///       possible data in that column.
    /// </summary>
    public static class PIISecure
    {
        /// <summary>
        /// Default symmetric key to be used if one is not provided as a method parameter
        /// </summary>
        public static string _symmetricKey { get; set; }

        /// <summary>
        /// Extension method that encrypts a string into a Base64 encoded value
        /// </summary>
        /// <param name="plainText">original string</param>
        /// <param name="symmetricKey">Symmetric key used for encryption/decryption</param>
        /// <returns>Base64 encode encrypted string</returns>
        public static string EncryptString(this string plainText, string symmetricKey = null)
        {
            byte[] array = plainText.Encrypt(symmetricKey);
            return Convert.ToBase64String(array);
        }


        /// <summary>
        /// Extension method that decrypts a Base64 value into a string value
        /// </summary>
        /// <param name="cipherText">Base64 value</param>
        /// <param name="symmetricKey">Symmetric key used for encryption/decryption</param>
        /// <returns>Decoded/decrypted string</returns>
        public static string DecryptString(this string cipherText, string symmetricKey = null)
        {
            byte[] buffer = Convert.FromBase64String(cipherText);
            return buffer.Decrypt(symmetricKey);
        }

        /// <summary>
        /// Extension method used to convert an object to string, then create an encrypted byte array
        /// </summary>
        /// <param name="valueToEncrypt">value to be encrypted</param>
        /// <param name="symmetricKey">Symmetric key used for encryption/decryption</param>
        /// <returns>byte[] containing the encrypted data</returns>
        public static byte[] Encrypt(this object valueToEncrypt, string symmetricKey = null)
        {
            string plainText = valueToEncrypt.ToString();

            symmetricKey ??= _symmetricKey;

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(symmetricKey);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// Extension method used to convert an encrypted byte array into a string
        /// </summary>
        /// <param name="cipherText">byte[] containing the encrypted data</param>
        /// <param name="symmetricKey">Symmetric key used for encryption/decryption</param>
        /// <returns>Decrypted data as a string</returns>
        public static string Decrypt(this byte[] cipherText, string symmetricKey = null)
        {
            symmetricKey ??= _symmetricKey;

            byte[] iv = new byte[16];

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(symmetricKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }

}
