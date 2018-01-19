namespace HRP.Core.Services
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);

        string Decrypt(string cipherText);
    }
}