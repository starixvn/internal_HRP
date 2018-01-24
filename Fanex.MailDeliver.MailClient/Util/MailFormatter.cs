namespace Fanex.MailDeliver.MailClient.Util
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class MailFormatter
    {
        private static BinaryFormatter _formatter = new BinaryFormatter();

        public static MemoryStream Serialize(object obj)
        {
            var stream = new MemoryStream();
            _formatter.Serialize(stream, obj);
            return stream;
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            return (T)_formatter.Deserialize(stream);
        }
    }
}