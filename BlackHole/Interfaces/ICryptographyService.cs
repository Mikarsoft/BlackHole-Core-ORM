
namespace BlackHole.Interfaces
{
    /// <summary>
    /// Cryptography Service is a work in progress
    /// The Encrypted Text that it produces cannot be inserted into
    /// the database because it contains some special characters 
    /// and it may cause trouble.
    /// In the next version it will be completed and it will 
    /// produce Hexadecimal text.
    /// </summary>
    public interface ICryptographyService
    {
        /// <summary>
        /// Generates a SHA1 Hash of the inserted text
        /// </summary>
        /// <param name="text">input text</param>
        /// <returns>SHA1 Hash</returns>
        string PassToHash(string text);

        /// <summary>
        /// Using the static stored 'EncryptionKeyVar'
        /// to encrypt the inserted text
        /// </summary>
        /// <param name="clearText">input text</param>
        /// <returns>Encrypted Text</returns>
        string Encrypt(string clearText);

        /// <summary>
        /// Using the static stored 'EncryptionKeyVar'
        /// to decrypt the inserted encrypted text 
        /// </summary>
        /// <param name="cipherText">encrypted text</param>
        /// <returns>Decrypted Text</returns>
        string Decrypt(string cipherText);

        /// <summary>
        /// Using a very simple Encryption
        /// to encrypt the inserted text
        /// </summary>
        /// <param name="text">input text</param>
        /// <returns>Simple Encrypted Text</returns>
        string BasicEncryption(string text);

        /// <summary>
        /// Using a very simple Decryption 
        /// to decrypt a simple encrypted text
        /// </summary>
        /// <param name="text">simple encrypted text</param>
        /// <returns>Decrypted Text</returns>
        string BasicDecryption(string text);
    }
}
