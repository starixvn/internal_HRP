namespace HRP.Core.Services
{
    using System;
    using Core.Services.Interfaces;
    using HRP.Core.Model;
    using Microsoft.Extensions.Options;

    public class AccountMailService : BaseMailService, IAccountMailService
    {
        private const string _EmailTemplateRecoveryPassword = "";
        private readonly IBaseMailService emailService;
        public AccountMailService(IOptionsSnapshot<MailConfig> configuration) : base(configuration)
        {
            emailService = new BaseMailService(configuration);
        }

        public void SendMailForRecoveryPassword(string username)
        {
            var email = GetMailForRecoveryPassword(username);
            SendEmail(email);
        }

        private MailModel GetMailForRecoveryPassword(string username)
        {
            MailModel email = new MailModel();
            return email;
        }

    }
}