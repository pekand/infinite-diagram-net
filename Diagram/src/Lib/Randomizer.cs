using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Diagram
{
    public class Randomizer
    {
        /*************************************************************************************************************************/
        // GENERATOR

        /// <summary>
        /// get random crypto secure string</summary>
        public static string GetRandomString(int length = 32)
        {
            byte[] tokenData = new byte[length+10];
            RandomNumberGenerator.Fill(tokenData);

            string token =
                Convert.ToBase64String(tokenData)
                .Replace("=", "")
                .Replace("/", "")
                .Replace("+", "")
                .Substring(0, length);

            return token;
        }

    }
}
