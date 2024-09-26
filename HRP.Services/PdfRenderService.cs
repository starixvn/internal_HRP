namespace HRP.Services
{
    using System.IO;
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Microsoft.AspNetCore.Hosting;
    using Spire.Pdf;

    public class PdfRenderService(
        IWebHostEnvironment hostingEnvironment,
        IConverter converter): IPdfRenderService
    {
        public byte[] RenderContent(string cssFilePath, string htmlText)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Unit = Unit.Millimeters, Top = 10, Bottom = 10, Left = 4, Right = 4 }
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlText,
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    Background = true,
                    LoadImages = true,
                    UserStyleSheet = Path.Combine(hostingEnvironment.WebRootPath, cssFilePath)
                },
                FooterSettings = { FontName = "Arial", FontSize = 7, Spacing = 1.5, Center = "[page]" }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return converter.Convert(pdf);
        }

        public byte[] RenderContentWithPasswordProtected(string cssFilePath, string htmlText, string userPassword, string ownerPassword)
        {
            byte[] content = RenderContent(cssFilePath, htmlText);

            using (var output = new MemoryStream())
            {
                var pdf = new PdfDocument();
                pdf.LoadFromBytes(content);
                pdf.Security.Encrypt(userPassword);
                pdf.SaveToStream(output);

                return output.ToArray();
            }
        }
    }
}