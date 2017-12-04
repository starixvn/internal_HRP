namespace Fanex.MailDeliver.Model
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class MailSetting
    {
        [DataMember]
        public int SettingID { get; set; }

        [DataMember]
        public string SettingName { get; set; }

        [DataMember]
        public string Server { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool IsAuth { get; set; }
    }
}