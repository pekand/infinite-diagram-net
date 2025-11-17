using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace Diagram
{
    class Hash
    {

        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        public static string GetFileHash(string pathToFile)
        {
            try
            {
                if (!Os.FileExists(pathToFile))
                {
                    return null;
                }

                using (FileStream stream = File.OpenRead(pathToFile))
                {
                    using (SHA256 Sha256 = SHA256.Create())
                    {
                        return Hash.BytesToString(Sha256.ComputeHash(stream));
                    }
                }

            } catch (Exception ex) {
                Program.log.Write("GetFileHash error: "+ex.Message);
            }

            return "";
        }
    }
}
