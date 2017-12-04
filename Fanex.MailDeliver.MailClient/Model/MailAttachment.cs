namespace Fanex.MailDeliver.Model
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public enum FileExtentions
    {
        MS_EXCEL,
        MS_DOC,
        IMG_JPG,
        IMG_GIF,
        IMG_PNG,
        PDF,
        TEXT
    }

    [Serializable]
    [DataContract]
    public class MailAttachment
    {
        private string _fileName = "noname";
        private FileExtentions __contentType;
        private System.IO.MemoryStream __content;

        public MailAttachment()
        {
            ////TODO
        }

        public MailAttachment(string fileName, FileExtentions contentType)
        {
            _fileName = fileName;
            __contentType = contentType;
        }

        public MailAttachment(System.IO.MemoryStream content, FileExtentions contentType)
        {
            __content = content;
            __contentType = contentType;
        }

        [DataMember]
        public int AttachmentID { get; set; }

        [DataMember]
        public long MailID { get; set; }

        [DataMember]
        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }

        [DataMember]
        public FileExtentions ContentType
        {
            get
            {
                return __contentType;
            }

            set
            {
                __contentType = value;
            }
        }

        [DataMember]
        public System.IO.MemoryStream Content
        {
            get
            {
                return __content;
            }

            set
            {
                __content = value;
            }
        }
    }
}