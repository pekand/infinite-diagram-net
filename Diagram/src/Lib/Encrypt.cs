using System.Security;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace Diagram
{

    /// <summary>
    /// repository for encryption related functions</summary>
    public class Encrypt 
    {
        /*************************************************************************************************************************/
        // HASHES

        /// <summary>
        /// get sha hash from inputString</summary>
        public static string CalculateSHA512Hash(string inputString)
        {
   
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = SHA512.HashData(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// get md5 hash from inputString</summary>
        public static string CalculateMD5Hash(string inputString)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = MD5.HashData(inputBytes);

            StringBuilder sb = new();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetMd5Hash(byte[] buffer)
        {
            byte[] data = MD5.HashData(buffer);

            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /*************************************************************************************************************************/
        // ENCRYPTION

        /// <summary>
        /// convert salt to stryng base64 encoded array</summary>
        public static string GetSalt(byte[] salt)
        {
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// convert salt to byte array</summary>
        public static byte[] SetSalt(string salt)
        {
            return Convert.FromBase64String(salt);
        }

        /// <summary>
        /// generate random crypto secure salt</summary>
        public static byte[] CreateSalt(int size)
        {
            byte[] buff = new byte[size];
            RandomNumberGenerator.Fill(buff);
            return buff;
        }

        /// <summary>
        /// encrypt plainText with sharedSecret password using salt</summary>
        public static string EncryptStringAES(string plainText, SecureString sharedSecret, byte[] salt = null)
        {
            String password = ConvertFromSecureString(sharedSecret);


            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(sharedSecret));

            string outStr = null;                       // Encrypted string to return


            Aes aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new(Encrypt.CalculateSHA512Hash(password), salt, 100000, HashAlgorithmName.SHA512);

                // Create a RijndaelManaged object
                aesAlg = Aes.Create();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();

                // prepend the IV
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using StreamWriter swEncrypt = new(csEncrypt);

                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                }
                outStr = Convert.ToBase64String(msEncrypt.ToArray());
            }
            finally
            {
                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            return outStr;
        }

        /// <summary>
        /// decrypt cipherText with sharedSecret password using salt</summary>
        public static string DecryptStringAES(string cipherText, string sharedSecret, byte[] salt = null, string version = "3")
        {
            if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(sharedSecret)) throw new ArgumentNullException(nameof(sharedSecret));

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            Aes aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                Rfc2898DeriveBytes key = null;
                // generate the key from the shared secret and the salt

                if (version == "3")
                {
                    key = new Rfc2898DeriveBytes(CalculateSHA512Hash(sharedSecret), salt, 100000, HashAlgorithmName.SHA512);
                }
                else if (version == "2")
                {
                    //legacy backward compatibility
                    key = new Rfc2898DeriveBytes(CalculateSHA512Hash(sharedSecret), salt, 1000, HashAlgorithmName.SHA1);
                }
                else
                {
                    throw new Exception("Invalid version of DecryptStringAES");
                }

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);

                using MemoryStream msDecrypt = new(bytes);

                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = Aes.Create();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                // Get the initialization vector from the encrypted stream
                aesAlg.IV = ReadByteArray(msDecrypt);
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            return plaintext;
        }

        /// <summary>
        /// helper function for DecryptStringAES</summary>
        private static byte[] ReadByteArray(MemoryStream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        /*************************************************************************************************************************/
        // SECURE STRING

        /// <summary>
        /// Protect string by encryption</summary>
        public static SecureString ConvertToSecureString(string str)
        {
            var secureStr = new SecureString();

            if (str.Length > 0)
            {
                foreach (var c in str.ToCharArray()) secureStr.AppendChar(c);
            }

            return secureStr;
        }

        /// <summary>
        /// Decrypt secure string</summary>
        private static string ConvertFromSecureString(SecureString value)
        {
            if (value == null)
            {
                return "";
            }

            return new System.Net.NetworkCredential(string.Empty, value).Password;
        }

        public static bool CompareSecureString(SecureString value, String value2)
        {
            return ConvertFromSecureString(value) == value2;
        }
    }
}
