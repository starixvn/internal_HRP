namespace HRP.Services
{
    using HRP.Models;

    public interface IRijndaelService
    {
        (RijndaelKeyValuePair, RijndaelKeyValuePair) GenerateRijndaelKeyValuePair();

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);

        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        void DecryptObject(object obj, string rijndaelKey, string rijndaelIV);

        void EncryptObject(object obj, string rijndaelKey, string rijndaelIV);

        void CleanPlainValue(object obj);
    }
}