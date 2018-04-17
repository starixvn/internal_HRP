namespace HRP.Services
{
    using HRP.Core.Model;

    public interface ICertService
    {
        CommandResult GenerateSelfSignedCertificate(string password);

        CommandResult ChangePasswordAccessTheCertificate(string password);
    }
}