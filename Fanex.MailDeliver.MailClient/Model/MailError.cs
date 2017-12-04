namespace Fanex.MailDeliver.Model
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public class MailError
    {
        [DataMember]
        public long ErrorID { get; set; }

        [DataMember]
        public long MailID { get; set; }

        [DataMember]
        public int AppID { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string SettingName { get; set; }

        [DataMember]
        public DateTime ErrorTime { get; set; }
    }
}