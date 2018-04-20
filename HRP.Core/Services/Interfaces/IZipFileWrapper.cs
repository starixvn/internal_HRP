namespace HRP.Core.Services
{
    public interface IZipFileWrapper
    {
        byte[] CopyFileToByteArray(string sourceFileName, string entryName);
    }
}