namespace HRP.Core.Services
{
    using System;
    using Fanex.MailDeliver.MailClient.API;
    using Fanex.MailDeliver.Model;
    using HRP.Core.Model;
    using Microsoft.Extensions.Options;

    public class HRPMailDeliver : IDisposable
    {
        protected IOptionsSnapshot<MailConfigs> configuration;
        private MailDeliver mailDeliver;

        public HRPMailDeliver(IOptionsSnapshot<MailConfigs> configuration)
        {
            this.configuration = configuration;
        }

        protected virtual void SendMail(MailRecord mailRecord)
        {
            mailDeliver.SendMail(mailRecord);
        }

        protected virtual MailDeliver GetMailDeliver()
        {
            mailDeliver = new MailDeliver(configuration.Value.AppId, configuration.Value.MailAddress);
            return mailDeliver;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            mailDeliver.Dispose();
        }
    }
}
