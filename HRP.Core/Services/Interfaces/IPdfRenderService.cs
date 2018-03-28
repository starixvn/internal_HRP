namespace HRP.Core.Services
{
    public interface IPdfRenderService
    {
        byte[] RenderContent(string cssFilePath, string htmlText);
    }
}