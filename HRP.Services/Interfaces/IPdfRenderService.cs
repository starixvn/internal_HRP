namespace HRP.Services
{
    public interface IPdfRenderService
    {
        byte[] RenderContent(string cssFilePath, string htmlText);

        byte[] RenderContentWithPasswordProtected(string cssFilePath, string htmlText, string userPassword, string ownerPassword);
    }
}