using System;
using System.Text;

namespace MonolithicNetCore.Common.Security
{
    public class SecurityCommon
    {
        /// <summary>
        /// Create hash password md5
        /// </summary>
        /// <param name="plainText">Plain text input</param>
        /// <returns></returns>
        public static string CreateHash(string plainText)
        {
            var x = System.Security.Cryptography.MD5.Create();
            var data = Encoding.ASCII.GetBytes(plainText);
            data = x.ComputeHash(data);
            return Convert.ToBase64String(data);
        }
    }
}