namespace HRP.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Hosting;

    public class CertService : ICertService
    {
        private const string SubjectName = "CN=HRP";
        private const string PfxFileName = "HRP.pfx";
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICertificateService certificateService;

        public CertService(
            IHostingEnvironment hostingEnvironment,
            ICertificateService certificateService)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.certificateService = certificateService;
        }

        public CommandResult GenerateSelfSignedCertificate(string password)
        {
            var commandResult = new CommandResult
            {
                Code = 0
            };

            try
            {
                X509Certificate2 cert = certificateService.GenerateSelfSignedCertificate(SubjectName, SubjectName);
                byte[] certData = cert.Export(X509ContentType.Pfx, password);
                string fileName = Path.Combine(hostingEnvironment.WebRootPath, "Cert", PfxFileName);
                File.WriteAllBytes(fileName, certData);
            }
            catch (Exception ex)
            {
                commandResult.Code = 1;
                commandResult.Message = ex.Message;
            }

            return commandResult;
        }

        public CommandResult ChangePasswordAccessTheCertificate(string password)
        {
            var commandResult = new CommandResult
            {
                Code = 0
            };

            try
            {
                string fileName = Path.Combine(hostingEnvironment.WebRootPath, "Cert", PfxFileName);
                var cert = new X509Certificate2(fileName, "123456", X509KeyStorageFlags.Exportable);
                File.WriteAllBytes(fileName, cert.Export(X509ContentType.Pfx, "1234aa"));
            }
            catch (Exception ex)
            {
                commandResult.Code = 1;
                commandResult.Message = ex.Message;
            }

            return commandResult;
        }
    }
}