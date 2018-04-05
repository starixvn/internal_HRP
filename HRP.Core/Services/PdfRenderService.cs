namespace HRP.Core.Services
{
    using System.IO;
    using System.Text;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;
    using Microsoft.AspNetCore.Hosting;

    public class PdfRenderService : IPdfRenderService
    {
        private const int A4Width = 595;
        private const int A4Height = 842;
        private readonly IHostingEnvironment hostingEnvironment;

        public PdfRenderService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public byte[] RenderContent(string cssFilePath, string htmlText)
        {
            using (var stream = new MemoryStream())
            {
                using (var pdfDocument = new Document(new Rectangle(A4Width, A4Height), 5, 5, 40, 40))
                {
                    pdfDocument.SetPageSize(PageSize.A4);

                    var pdfWriter = PdfWriter.GetInstance(pdfDocument, stream);
                    var worker = XMLWorkerHelper.GetInstance();

                    pdfDocument.Open();

                    using (var htmlTextReader = new MemoryStream(Encoding.Unicode.GetBytes(htmlText)))
                    {
                        string webRootPath = hostingEnvironment.WebRootPath;
                        var cssFile = Path.Combine(webRootPath, cssFilePath);

                        using (var cssStream = new FileStream(cssFile, FileMode.Open, FileAccess.Read))
                        {
                            worker.ParseXHtml(pdfWriter, pdfDocument, htmlTextReader, cssStream, Encoding.Unicode);
                        }
                    }

                    pdfDocument.Close();
                }

                return stream.ToArray();
            }
        }

        public byte[] RenderContentWithPasswordProtected(string cssFilePath, string htmlText, string userPassword, string ownerPassword)
        {
            byte[] content = RenderContent(cssFilePath, htmlText);

            using (var input = new MemoryStream(content))
            {
                using (var output = new MemoryStream())
                {
                    var reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, userPassword, ownerPassword, PdfWriter.ALLOW_SCREENREADERS);

                    return output.ToArray();
                }
            }
        }
    }
}