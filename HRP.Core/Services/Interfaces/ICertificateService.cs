namespace HRP.Core.Services
{
    using HRP.Core.Model;

    public interface ICertificateService
    {
        string GetCertificateFilePath();

        string GetPasswordAccessTheCertificate();

        CommandResult GenerateSelfSignedCertificate(string password);

        bool IsGeneratedCertificate();

        bool IsExistedCertificate();

        bool IsCorrectPasswordAccessTheCertificate(string password);

        void CachePasswordAccessTheCertificate(string password);

        CommandResult ChangePasswordAccessTheCertificate(string oldPassword, string password);
    }
}