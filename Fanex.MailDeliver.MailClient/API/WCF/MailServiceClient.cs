namespace Fanex.MailDeliver.MailClient.API.WCF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerStepThroughAttribute]
    public partial class MailServiceClient : System.ServiceModel.ClientBase<IMailService>, IMailService
    {
        public MailServiceClient()
        {
        }

        public MailServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public MailServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public MailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public MailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public void SendMail(byte[] buffer)
        {
            Channel.SendMail(buffer);
        }

        public byte[] GetMail(long mailID)
        {
            return Channel.GetMail(mailID);
        }

        public IEnumerable<byte[]> GetListMail(int appID, DateTime mailDate, int page, int counter)
        {
            return Channel.GetListMail(appID, mailDate, page, counter);
        }

        public IEnumerable<byte[]> GetListMailByDate(int appID, DateTime fromDate, DateTime toDate, ref int totalRows, int page, int counter, int status)
        {
            return Channel.GetListMailByDate(appID, fromDate, toDate, ref totalRows, page, counter, status);
        }

        public byte[] StoreConfig(byte[] buffer)
        {
            return Channel.StoreConfig(buffer);
        }

        public byte[] GetConfig(int appID)
        {
            return Channel.GetConfig(appID);
        }

        public byte[] StoreSetting(byte[] buffer)
        {
            return Channel.StoreSetting(buffer);
        }

        public byte[] GetSetting(int settingID)
        {
            return Channel.GetSetting(settingID);
        }

        public void ReSendMail(long mailID)
        {
            Channel.ReSendMail(mailID);
        }

        public byte[] StoreFunction(byte[] buffer)
        {
            return Channel.StoreFunction(buffer);
        }

        public byte[] GetFunction(int funcID)
        {
            return Channel.GetFunction(funcID);
        }

        public IEnumerable<byte[]> GetListSetting()
        {
            return Channel.GetListSetting();
        }

        public IEnumerable<byte[]> GetListConfig(string serverIp)
        {
            return Channel.GetListConfig(serverIp);
        }

        public IEnumerable<byte[]> GetListFuntion()
        {
            return Channel.GetListFuntion();
        }

        public IEnumerable<byte[]> GetListError(long mailID)
        {
            return Channel.GetListError(mailID);
        }
    }
}