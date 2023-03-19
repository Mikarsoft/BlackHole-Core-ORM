using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.CoreSupport
{
    internal static class HashGeneratorSHA1
    {
        public static string GenerateSHA1(this string text)
        {
            var sh = SHA1.Create();
            var hash = new StringBuilder();
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] b = sh.ComputeHash(bytes);

            foreach (byte a in b)
            {
                var h = a.ToString("x2");
                hash.Append(h);
            }

            return hash.ToString();
        }
    }
}
