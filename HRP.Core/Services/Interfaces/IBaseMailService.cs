namespace HRP.Core.Services
{
    using System.Threading.Tasks;
    using Model;

    public interface IBaseMailService
    {
        Task<bool> SendEmailAsync(MailModel email);

        string ReadTextFile(string filePath);
    }
}