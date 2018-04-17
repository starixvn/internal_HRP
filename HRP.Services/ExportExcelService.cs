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

        public byte[] ExportBankInReports(DateTime selectedTime, FileInfo fileInfo, BankInReport data)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();

                // Set company name
                worksheet.Cells["C2"].Value = data.CompanyName;

                // Set month
                worksheet.Cells["C3"].Value = selectedTime.ToString("MMMM yyyy");

                var startRow = 9;

                // EBanking
                var ebankingGroupedByBank = data.EBankingBankIns.GroupBy(x => x.BankName);
                startRow = RenderReportRowsForBankIns(ebankingGroupedByBank, worksheet, startRow);

                var directBanking1GroupedByBank = data.DirectBanking1BankIns.GroupBy(x => x.BankName);
                startRow = RenderReportRowsForBankIns(directBanking1GroupedByBank, worksheet, startRow);

                var directBanking2GroupedByBank = data.DirectBanking2BankIns.GroupBy(x => x.BankName);
                startRow = RenderReportRowsForBankIns(directBanking2GroupedByBank, worksheet, startRow);

                var cashGroupedByBank = data.CashBankIns.GroupBy(x => x.BankName);
                RenderReportRowsForBankIns(cashGroupedByBank, worksheet, startRow);

                worksheet.Cells.AutoFitColumns();

                return xlPackage.GetAsByteArray();
            }
        }

        private static int RenderReportRowsForBankIns(
            IEnumerable<IGrouping<string, BankIn>> bankInsGroupedByBank,
            ExcelWorksheet worksheet,
            int startRow)
        {
            int rowIndex = startRow;

            foreach (IGrouping<string, BankIn> bankIns in bankInsGroupedByBank)
            {
                // group by currency

                var bankInsGroupByCurrency = bankIns.GroupBy(x => x.CurrencyCode);
                foreach (IGrouping<string, BankIn> grouping in bankInsGroupByCurrency)
                {
                    worksheet.Cells[rowIndex, 2].LoadFromCollectionFiltered(grouping.ToList());

                    // Create sumary row for a bank
                    rowIndex = rowIndex + grouping.Count();
                    var summaryRow = worksheet.Cells[rowIndex, 2, rowIndex, 5];
                    summaryRow.Merge = true;

                    // TODO: move to resource when user confirmed
                    summaryRow.Value = string.Format("Total ({0})", grouping.Key);

                    worksheet.Cells[rowIndex, 6].FormulaR1C1 =
                        $"=SUM(R{(rowIndex - grouping.Count())}C6:R{rowIndex - 1}C6)";

                    rowIndex++;
                }
            }

            return rowIndex;
        }

        public Stream ExportDynamicReport(IEnumerable<DynamicReportModel> records, GenerateReportViewModel viewModel)
        {
            var selectedGroups = viewModel.ColumnGroupIds.Select(x => ColumnsDictionary.Columns[x]);
            var headers = selectedGroups.SelectMany(x => x.Columns);

            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");

                // Write header
                var col = 1;
                foreach (var header in headers)
                {
                    worksheet.Cells[1, col].Value = header.HeaderName;
                    col++;
                }

                // Write Data
                var row = 2;
                foreach (var record in records)
                {
                    col = 1;
                    foreach (var header in headers)
                    {
                        var propertyInfo = record.GetType().GetProperty(header.PropertyName);

                        worksheet.Cells[row, col].Value = propertyInfo.GetValue(record);
                        if (propertyInfo.PropertyType == typeof(DateTime))
                        {
                            worksheet.Cells[row, col].Style.Numberformat.Format = "MM/dd/yyyy";
                        }

                        col++;
                    }

                    row++;
                }

                var memory = new MemoryStream();
                package.SaveAs(memory);
                memory.Position = 0;

                return memory;
            }
        }
    }
}