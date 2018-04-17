namespace HRP.Core.Services
{
    using System.Security.Cryptography.X509Certificates;

    public interface ICertificateService
    {
        X509Certificate2 GenerateSelfSignedCertificate(
            string subjectName,
            string issuerName,
            int keyStrength = 2048);
    }
}