namespace HRP.Core.Services
{
    using System.IO;
    using System.Text;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.tool.xml;

    public class PdfRenderService : IPdfRenderService
    {
        private const int A4Width = 595;
        private const int A4Height = 842;

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
                        using (var cssStream = new FileStream(cssFilePath, FileMode.Open, FileAccess.Read))
                        {
                            worker.ParseXHtml(pdfWriter, pdfDocument, htmlTextReader, cssStream, Encoding.Unicode);
                        }
                    }

                    pdfDocument.Close();
                }

                return stream.ToArray();
            }
        }
    }
}