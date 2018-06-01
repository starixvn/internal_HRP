namespace HRP.Services
{
    using System;

    public interface IRijndaelService
    {
        Tuple<string, string> GenerateKey();

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);
    }
}