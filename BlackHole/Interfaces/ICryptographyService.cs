
namespace BlackHole.Interfaces
{
    public interface ICryptographyService
    {
        string PassToHash(string text);
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
        string BasicEncryption(string text);
        string BasicDecryption(string text);
    }
}
