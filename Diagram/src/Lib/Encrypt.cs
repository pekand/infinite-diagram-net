using System;
using System.Text;
using System.Security.Cryptography;
using System.Security;
using System.IO;

#nullable disable

namespace Diagram
{

    /// <summary>
    /// repository for encryption related functions</summary>
    public class Encrypt //UID5102459625
    {
        /*************************************************************************************************************************/
        // HASHES

        /// <summary>
        /// get sha hash from inputString</summary>
        public static string CalculateSHA512Hash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA512.Create()) {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
                byte[] hash = algorithm.ComputeHash(inputBytes);

                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// get md5 hash from inputString</summary>
        public static string CalculateMD5Hash(string inputString)
        {
            // step 1, calculate MD5 hash from input
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
                byte[] hash = md5.ComputeHash(inputBytes);
            
                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string GetMd5Hash(byte[] buffer)
        {
            using (MD5 md5Hasher = MD5.Create())
            {

                byte[] data = md5Hasher.ComputeHash(buffer);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
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


            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(Encrypt.CalculateSHA512Hash(password), salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// decrypt cipherText with sharedSecret password using salt</summary>
        public static string DecryptStringAES(string cipherText, string sharedSecret, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret)) throw new ArgumentNullException("sharedSecret");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(CalculateSHA512Hash(sharedSecret), salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes)) {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) 
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        /// <summary>
        /// helper function for DecryptStringAES</summary>
        private static byte[] ReadByteArray(Stream s)
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
