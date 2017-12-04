namespace HRP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using HRP.Core.Extensions;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using NPOI.XSSF.UserModel;
    using NPOI.SS.UserModel;

    public class ExportExcelController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public ExportExcelController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"ExcelTemplates\\TestTemplate.xls";
            var writer = new ExcelTemplateWriter(Path.Combine(sWebRootFolder, sFileName), "Employee", "Employee");
            writer.CopyRows(0, 3);
            writer.SyncColumnsWidth(5);
            writer.CreateFreezePane(0, 3);

            return File(await writer.Render(Path.Combine(sWebRootFolder, "employee.xls")), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employee.xls");
        }

        public async Task<IActionResult> Export()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Age");

                row = excelSheet.CreateRow(1);
                row.CreateCell(0).SetCellValue(1);
                row.CreateCell(1).SetCellValue("Kane Williamson");
                row.CreateCell(2).SetCellValue(29);

                row = excelSheet.CreateRow(2);
                row.CreateCell(0).SetCellValue(2);
                row.CreateCell(1).SetCellValue("Martin Guptil");
                row.CreateCell(2).SetCellValue(33);

                row = excelSheet.CreateRow(3);
                row.CreateCell(0).SetCellValue(3);
                row.CreateCell(1).SetCellValue("Colin Munro");
                row.CreateCell(2).SetCellValue(23);

                workbook.Write(fs);
            }

            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
    }
}