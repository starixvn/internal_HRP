namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Core.Extensions.Excel;
    using Interfaces;
    using Models.Report;
    using NPOI.XSSF.UserModel;
    using OfficeOpenXml;

    [ExcludeFromCodeCoverage]
    public class ExportExcelService : IExportExcelService
    {
        public Stream GetPayDetails(Stream fileStream)
        {
            var workbook = new XSSFWorkbook(fileStream);
            var templateSheet = workbook.GetSheetAt(0);
            var sheet = workbook.CreateSheet("Pay Details");

            for (var i = 0; i < 9; i++)
            {
                sheet.CopyRow(templateSheet, i, i, 0, int.MaxValue - 1);
            }

            var memory = new MemoryStream();
            sheet.Workbook.RemoveSheetAt(0);
            sheet.CreateFreezePane(4, 9);
            sheet.Workbook.Write(memory);
            memory.Position = 0;

            return memory;
        }

        public byte[] ExportBankInReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            IEnumerable<BankIn> data)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();

                // Set company name
                worksheet.Cells["C2"].Value = data.First().CompanyName;

                // Set month
                worksheet.Cells["C3"].Value = selectedTime.ToString("MMMM yyyy");

                // Set values for each banks
                var bankInDataGrouped = data.GroupBy(x => x.BankName);
                var startRow = 10;
                foreach (IGrouping<string, BankIn> bankIns in bankInDataGrouped)
                {
                    worksheet.Cells[startRow, 2].LoadFromCollectionFiltered(bankIns.ToList());

                    // Create sumary row for a bank
                    startRow = startRow + bankIns.Count();
                    var summaryRow = worksheet.Cells[startRow, 2, startRow, 5];
                    summaryRow.Merge = true;
                    summaryRow.Value = "Total (RM) - " + bankIns.Key;

                    worksheet.Cells[startRow, 6].FormulaR1C1 =
                        $"=SUM(R{(startRow - bankIns.Count() - 1)}C6:R{startRow - 1}C6)";

                    startRow++;
                }

                worksheet.Cells.AutoFitColumns();

                return xlPackage.GetAsByteArray();
            }
        }
    }
}