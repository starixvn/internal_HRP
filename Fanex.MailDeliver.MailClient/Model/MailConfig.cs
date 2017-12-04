namespace Fanex.MailDeliver.Model
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class MailConfig
    {
        [DataMember]
        public int AppID { get; set; }

        [DataMember]
        public string AppName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public MailSetting Setting { get; set; }

        [DataMember]
        public int SettingID { get; set; }

        [DataMember]
        public string MailFrom { get; set; }

        [DataMember]
        public bool IsEncrypt { get; set; }
    }
}