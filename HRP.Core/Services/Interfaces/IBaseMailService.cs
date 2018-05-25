namespace HRP.Core.Services
{
    using Model;

    public interface IBaseMailService
    {
        bool SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}