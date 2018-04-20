namespace HRP.Core.Services
{
    using System.IO;
    using System.IO.Compression;

    public class ZipFileWrapper : IZipFileWrapper
    {
        public byte[] CopyFileToByteArray(string sourceFileName, string entryName)
        {
            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(sourceFileName, entryName, CompressionLevel.Fastest);
                }

                return ms.ToArray();
            }
        }
    }
}