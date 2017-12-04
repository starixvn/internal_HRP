namespace Fanex.MailDeliver.Model
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class MailFunction
    {
        [DataMember]
        public int FuncID { get; set; }

        [DataMember]
        public int AppID { get; set; }

        [DataMember]
        public MailConfig Config { get; set; }

        [DataMember]
        public string FuncName { get; set; }

        [DataMember]
        public MailSetting Setting { get; set; }

        [DataMember]
        public int SettingID { get; set; }
    }
}