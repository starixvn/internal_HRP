namespace Fanex.MailDeliver.MailClient.API
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Fanex.MailDeliver.MailClient.API.WCF;
    using Fanex.MailDeliver.MailClient.Util;
    using Fanex.MailDeliver.Model;

    public class MailDeliver : IMailDeliver, IDisposable
    {
        private MailServiceClient _mailClient;
        private readonly MailRecord _mailRecord;
        private readonly int _appId;

        public MailDeliver(int appId, string mailAddress)
        {
            _appId = appId;
            var endpoint = new EndpointAddress(mailAddress);

            // Setting binding
            var binding = GetBinding(endpoint);

            // Create mail client service
            _mailClient = new MailServiceClient(binding, endpoint);

            // Create Mail Record for send Mail
            _mailRecord = new MailRecord();
        }

        public Binding GetBinding(EndpointAddress endpoint)
        {
            if (endpoint.Uri.Scheme.IndexOf("http", StringComparison.InvariantCulture) != -1)
            {
                var httpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                {
                    SendTimeout = new TimeSpan(0, 0, 5),
                    OpenTimeout = new TimeSpan(0, 0, 5),
                    MaxBufferSize = 2097152, // max 2M
                    MaxReceivedMessageSize = 2097152,
                    MaxBufferPoolSize = 2097152 * 4 // max 8M
                };
                httpBinding.ReaderQuotas.MaxArrayLength = 2097152;
                httpBinding.ReaderQuotas.MaxDepth = 32;
                httpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
                httpBinding.ReaderQuotas.MaxStringContentLength = 2097152;
                return httpBinding;
            }
            else
            {
                // Setting net.tcp binding
                var tcpBinding = new NetTcpBinding(SecurityMode.None, true);
                tcpBinding.ReliableSession.Enabled = true;
                tcpBinding.ReliableSession.Ordered = true;
                tcpBinding.SendTimeout = new TimeSpan(0, 0, 5);
                tcpBinding.OpenTimeout = new TimeSpan(0, 0, 5);
                tcpBinding.MaxBufferSize = 2097152; // max 2M
                tcpBinding.MaxBufferPoolSize = 2097152 * 4; // max 8M
                tcpBinding.MaxConnections = 10;
                tcpBinding.ReaderQuotas.MaxArrayLength = 2097152;
                tcpBinding.ReaderQuotas.MaxDepth = 32;
                tcpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
                tcpBinding.ReaderQuotas.MaxStringContentLength = 2097152;

                return tcpBinding;
            }
        }

        public void SendMail(
            MailRecord mail,
            bool canRetry = true)
        {
            if (mail != null)
            {
                if (mail.Config == null || mail.Config.AppID == 0)
                {
                    mail.Config = new MailConfig
                    {
                        AppID = _appId
                    };
                }

                AttachmentUtil.Format(ref mail);

                mail.CanRetry = canRetry;
                var stream = MailFormatter.Serialize(mail);
                var buffer = stream.ToArray();
                _mailClient.SendMail(buffer);
            }
        }

        public MailRecord GetMail(long mailID)
        {
            MailRecord record = null;
            if (mailID > 0)
            {
                byte[] buffer = _mailClient.GetMail(mailID);
                if (buffer != null)
                {
                    record = MailFormatter.Deserialize<MailRecord>(buffer);
                }
            }

            return record;
        }

        public IEnumerable<MailRecord> GetListMail(
            int appID,
            DateTime mailDate,
            int page,
            int counter)
        {
            var mailList = new List<MailRecord>();
            if(appID <= 0)
            {
                return mailList;
            }

            IEnumerable<byte[]> buffers = _mailClient.GetListMail(appID, mailDate, page, counter);
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var record = MailFormatter.Deserialize<MailRecord>(buffer);
                    if (record != null)
                    {
                        mailList.Add(record);
                    }
                }
            }

            return mailList;
        }

        public IEnumerable<MailRecord> GetListMailByDate(
            int appID,
            DateTime fromDate,
            DateTime toDate,
            ref int totalRows,
            int page,
            int counter,
            MailStatus status)
        {
            var mailList = new List<MailRecord>();
            if(appID < 0)
            {
                return mailList;
            }

            IEnumerable<byte[]> buffers = _mailClient.GetListMailByDate(appID, fromDate, toDate, ref totalRows, page, counter, (int)status);
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var record = MailFormatter.Deserialize<MailRecord>(buffer);
                    if (record != null)
                    {
                        mailList.Add(record);
                    }
                }
            }

            return mailList;
        }

        public MailConfig StoreConfig(MailConfig config)
        {
            if (config != null)
            {
                var stream = MailFormatter.Serialize(config);
                return MailFormatter.Deserialize<MailConfig>(_mailClient.StoreConfig(stream.ToArray()));
            }

            return null;
        }

        public MailConfig GetConfig(int appID)
        {
            MailConfig config = null;

            if (appID > 0)
            {
                var buffer = _mailClient.GetConfig(appID);
                if (buffer != null)
                {
                    config = MailFormatter.Deserialize<MailConfig>(buffer);
                }
            }

            return config;
        }

        #region Mail Setting

        public MailSetting StoreSetting(MailSetting config)
        {
            if (config != null)
            {
                var stream = MailFormatter.Serialize(config);
                return MailFormatter.Deserialize<MailSetting>(_mailClient.StoreSetting(stream.ToArray()));
            }

            return null;
        }

        public MailSetting GetSetting(int settingID)
        {
            MailSetting setting = null;

            if (settingID > 0)
            {
                var buffer = _mailClient.GetSetting(settingID);
                if (buffer != null)
                {
                    setting = MailFormatter.Deserialize<MailSetting>(buffer);
                }
            }

            return setting;
        }

        #endregion Mail Setting

        #region resend mail

        public void ReSendMail(long mailID)
        {
            if (mailID > 0)
            {
                _mailClient.ReSendMail(mailID);
            }
        }

        #endregion resend mail

        #region mail function

        public MailFunction StoreFunction(MailFunction func)
        {
            if (func != null)
            {
                var stream = MailFormatter.Serialize(func);
                return MailFormatter.Deserialize<MailFunction>(_mailClient.StoreFunction(stream.ToArray()));
            }

            return null;
        }

        public MailFunction GetFunction(int funcID)
        {
            MailFunction func = null;

            if (funcID > 0)
            {
                var buffer = _mailClient.GetFunction(funcID);
                if (buffer != null)
                {
                    func = MailFormatter.Deserialize<MailFunction>(buffer);
                }
            }

            return func;
        }

        #endregion mail function

        #region get list setting

        public IEnumerable<MailSetting> GetListSetting()
        {
            var settings = new List<MailSetting>();

            IEnumerable<byte[]> buffers = _mailClient.GetListSetting();
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var setting = MailFormatter.Deserialize<MailSetting>(buffer);
                    if (setting != null)
                    {
                        settings.Add(setting);
                    }
                }
            }

            return settings;
        }

        public IEnumerable<MailConfig> GetListConfig(string serverIp)
        {
            var configs = new List<MailConfig>();
            IEnumerable<byte[]> buffers = _mailClient.GetListConfig(serverIp);
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var config = MailFormatter.Deserialize<MailConfig>(buffer);
                    if (config != null)
                    {
                        configs.Add(config);
                    }
                }
            }

            return configs;
        }

        public IEnumerable<MailFunction> GetListFuntion()
        {
            var functions = new List<MailFunction>();
            IEnumerable<byte[]> buffers = _mailClient.GetListFuntion();
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var func = MailFormatter.Deserialize<MailFunction>(buffer);
                    if (func != null)
                    {
                        functions.Add(func);
                    }
                }
            }

            return functions;
        }

        #endregion get list setting

        public IEnumerable<MailError> GetListError(long mailID)
        {
            var errors = new List<MailError>();
            IEnumerable<byte[]> buffers = _mailClient.GetListError(mailID);
            if (buffers != null)
            {
                foreach (var buffer in buffers)
                {
                    var error = MailFormatter.Deserialize<MailError>(buffer);
                    if (error != null)
                    {
                        errors.Add(error);
                    }
                }
            }

            return errors;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Fluent Interface
        public IMailDeliver CreateMail(string mailTitle = "")
        {
            _mailRecord.Title = mailTitle;
            return this;
        }

        public IMailDeliver AddSubject(string mailSubject = "")
        {
            _mailRecord.Subject = mailSubject;
            return this;
        }

        public IMailDeliver To(string toMail = "")
        {
            _mailRecord.MailTo = toMail;
            return this;
        }

        public IMailDeliver AddSendMailApplication(int mailApplication = 0)
        {
            _mailRecord.Config.AppID = mailApplication;
            return this;
        }

        public IMailDeliver AddSendMailFunction(int mailFunction = 0)
        {
            _mailRecord.Function.FuncID = mailFunction;
            return this;
        }

        public IMailDeliver AddBody(string mailBody = "")
        {
            _mailRecord.Body = mailBody;
            return this;
        }

        public IMailDeliver ToCcMails(string mailCc = "")
        {
            _mailRecord.MailCc = mailCc;
            return this;
        }

        public IMailDeliver ToBccMails(string mailBc = "")
        {
            _mailRecord.MailBcc = mailBc;
            return this;
        }

        public IMailDeliver AddTitle(string mailTitle = "")
        {
            _mailRecord.Title = mailTitle;
            return this;
        }

        public IMailDeliver AddAttachments(List<MailAttachment> mailAttachments)
        {
            if (mailAttachments != null)
            {
                _mailRecord.Attachments = mailAttachments;
            }

            return this;
        }

        public IMailDeliver AddAlternativeViews(List<MailLinkedResource> mailAlternativeViews)
        {
            if (mailAlternativeViews != null)
            {
                _mailRecord.AlternateView = mailAlternativeViews;
            }

            return this;
        }

        public IMailDeliver SendMail(bool canRetry = true)
        {
            _mailRecord.CanRetry = canRetry;
            this.SendMail(_mailRecord, _mailRecord.CanRetry);
            return this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_mailClient != null && _mailClient.State == CommunicationState.Opened)
            {
                _mailClient.Close();
            }

            _mailClient = null;
        }
    }
}