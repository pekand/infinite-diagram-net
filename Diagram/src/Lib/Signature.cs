using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace Diagram
{
    class Signature
    {
        public static string CalculateHash(string data)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);

            using var hashProvider = SHA512.Create();

            var hashedInputBytes = hashProvider.ComputeHash(bytes);
            var hashedInputStringBuilder = new StringBuilder(128);
            foreach (var b in hashedInputBytes)
            {
                hashedInputStringBuilder.Append(b.ToString("X2"));
            }
            return hashedInputStringBuilder.ToString();
        }

        public static string GenerateSignatureSecret()
        {
            const int signatureLength = 1024;
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";


            string signature = "";
            var randomByte = new byte[signatureLength];
            RandomNumberGenerator.Fill(randomByte);

            for (int i=0; i < signatureLength; i++) {
                signature += validCharacters[randomByte[i] % validCharacters.Length];
            }

            return signature;
        }

        public static string GenerateIV()
        {
            using SymmetricAlgorithm crypt = Aes.Create();

            crypt.KeySize = 256;
            crypt.GenerateIV();
            return Convert.ToBase64String(crypt.IV);
        }

        public static string SignText(string signatureSecret, string data, string iv)
        {
            string hash = Signature.CalculateHash(data);

            return Signature.Encrypt(signatureSecret, hash, iv);
        }

        public static bool CheckSignature(string signatureSecret, string signature, string data, string iv)
        {
            string hashFromSignature = Signature.Decrypt(signatureSecret, signature, iv);
            string hash = Signature.CalculateHash(data);

            return (hashFromSignature != null && hash != null && hashFromSignature == hash);
        }

        public static string Encrypt(string password, string data, string iv)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                using SymmetricAlgorithm crypt = Aes.Create();
                using HashAlgorithm hash = SHA256.Create();
                using MemoryStream memoryStream = new();

                crypt.KeySize = 256;
                crypt.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                crypt.IV = Convert.FromBase64String(iv);

                using (CryptoStream cryptoStream = new(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                string base64Ciphertext = Convert.ToBase64String(memoryStream.ToArray());

                return base64Ciphertext;
            }
            catch
            {
                return null;
            }
        }

        public static string Decrypt(string password, string data, string iv)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data);

                using SymmetricAlgorithm crypt = Aes.Create();
                using HashAlgorithm hash = SHA256.Create();
                using MemoryStream memoryStream = new(bytes);

                crypt.KeySize = 256;
                crypt.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                crypt.IV = Convert.FromBase64String(iv);

                using CryptoStream cryptoStream = new(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read);

                var plainTextBytes = new byte[bytes.Length];
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount); ;
            } catch {
                return null;
            }
        }
    }
}
