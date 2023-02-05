using BlackHole.Interfaces;
using BlackHole.Statics;
using System.Security.Cryptography;
using System.Text;

namespace BlackHole.Services
{
    internal class CryptographyService : ICryptographyService
    {
        string ICryptographyService.PassToHash(string text)
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

        string ICryptographyService.Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";

            if (EncryptionKeyVar.Keyword != string.Empty)
            {
                EncryptionKey = EncryptionKeyVar.Keyword;
            }

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] 
                { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        string ICryptographyService.Decrypt(string cipherText)
        {

            string EncryptionKey = "MAKV2SPBNI99212";

            if (EncryptionKeyVar.Keyword != string.Empty)
            {
                EncryptionKey = EncryptionKeyVar.Keyword;
            }

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] 
                { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        string ICryptographyService.BasicEncryption(string text)
        {
            byte xorConstant = 0x57;

            byte[] data = Encoding.UTF8.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ xorConstant);
            }
            string output = Convert.ToBase64String(data);
            return output;
        }

        string ICryptographyService.BasicDecryption(string text)
        {
            byte xorConstant = 0x57;

            byte[] data = Convert.FromBase64String(text);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ xorConstant);
            }
            string plainText = Encoding.UTF8.GetString(data);
            return plainText;
        }
    }
}

