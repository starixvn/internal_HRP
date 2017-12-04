namespace HRP.Core.Services
{
    using Model;

    public interface IBaseMailService
    {
         void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}