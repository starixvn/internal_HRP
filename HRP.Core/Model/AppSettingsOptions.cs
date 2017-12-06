namespace HRP.Core.Model
{
    public class AppSettingsOptions
    {
        public int SessionTimeout { set; get; }

        public bool EnableEncrypt { get; set; }

        public string CertificateFilePath { get; set; }

        public string PasswordToSignTheCertificateFile { get; set; }

        public string BaseHostUrl { get; set; }
    }
}