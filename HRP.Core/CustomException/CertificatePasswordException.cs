namespace HRP.Core.CustomException
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CertificatePasswordException : Exception
    {
        public CertificatePasswordException()
        {
        }

        public CertificatePasswordException(string message)
            : base(message)
        {
        }

        public CertificatePasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CertificatePasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}