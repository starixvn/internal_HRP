namespace HRP.Core.Services.Interfaces
{
    public interface IAccountMailService
    {
         void SendMailForRecoveryPassword(string templateEmailPath);
    }
}