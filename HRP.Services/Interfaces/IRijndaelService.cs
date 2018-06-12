namespace HRP.Services
{
    using System;

    public interface IRijndaelService
    {
        Tuple<string, string> GenerateKey();

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);

        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        void DecryptObject(object obj, string rijndaelKey, string rijndaelIV);

        void EncryptObject(object obj, string rijndaelKey, string rijndaelIV);

        void CleanPlainValue(object obj);
    }
}