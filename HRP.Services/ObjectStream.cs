namespace HRP.Services
{
    using System.IO;

    public class ObjectStream : IObjectStream
    {
        public Stream CreateInstance(string filePath)
        {
            return new FileStream(filePath, FileMode.Open);
        }
    }
}