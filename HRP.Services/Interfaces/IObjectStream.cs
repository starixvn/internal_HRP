namespace HRP.Services
{
    using System.IO;

    public interface IObjectStream
    {
        Stream CreateInstance(string filePath);
    }
}