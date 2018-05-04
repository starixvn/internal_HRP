namespace HRP.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [ExcludeFromCodeCoverage]
    public class ObjectStream : IObjectStream
    {
        public Stream CreateInstance(string filePath)
        {
            return new FileStream(filePath, FileMode.Open);
        }
    }
}