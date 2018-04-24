namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Core.Extensions;
    using Core.Extensions.Excel;
    using HRP.Core.Services;
    using Interfaces;
    using Models.Report;
    using NPOI.XSSF.UserModel;
    using OfficeOpenXml;

    [ExcludeFromCodeCoverage]
    public class ExportExcelService : IExportExcelService
    {
        private readonly ICryptoService cryptoService;

        public ExportExcelService(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

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

        public byte[] ExportBankInReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();

                // Set company name
                worksheet.Cells["D2"].Value = data.CompanyName;

                // Set month
                worksheet.Cells["D3"].Value = selectedTime.ToString("MMMM yyyy");

                // Set payment type
                worksheet.Cells["D4"].Value = typeOfPayment.GetDisplayName();

                // Set selected payment transaction
                worksheet.Cells["D5"].Value = string.Join(",", data.SelectedPaymentTransactions);

                var startRow = 10;

                // EBanking
                var ebankingGroupedByBank = data.EBankingBankIns.GroupBy(x => x.BankName);
                startRow = RenderReportRowsForBankIns(ebankingGroupedByBank, worksheet, startRow);

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
                    int count = 0;
                    var bankInRows = grouping.ToList();
                    foreach (var bankIn in bankInRows)
                    {
                        worksheet.Cells[rowIndex, 2].Value = ++count;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.Nickname;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.PaymentTransaction;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.CurrencyCode;

                        rowIndex++;
                    }

                    // Create sumary row for a bank
                    var summaryRow = worksheet.Cells[rowIndex, 2, rowIndex, 7];
                    summaryRow.Merge = true;

                    // TODO: move to resource when user confirmed
                    summaryRow.Value = string.Format("Total ({0})", grouping.Key);

                    worksheet.Cells[rowIndex, 8].FormulaR1C1 =
                        $"=SUM(R{(rowIndex - grouping.Count())}C8:R{rowIndex - 1}C8)";

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

        public Stream ExportSalaryAllowanceReport(string templatePath, IEnumerable<DynamicReportModel> records)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Salary & Allowance Report");

                // Write header
                templateWorksheet.Cells[1, 1, 8, 46].Copy(worksheet.Cells[1, 1, 8, 46]);

                // Write Data
                var row = 9;
                foreach (var record in records)
                {
                    templateWorksheet.Cells[9, 1, 9, 46].Copy(worksheet.Cells[row, 1, row, 46]);

                    worksheet.Cells[row, 1].Value = record.FullName;
                    worksheet.Cells[row, 2].Value = record.Nickname;
                    row++;
                }
                var memory = new MemoryStream();
                package.SaveAs(memory);
                memory.Position = 0;

                return memory;
            }
        }

        public byte[] ExportDynamicReport(IEnumerable<DynamicReportEmployeeModel> records, IEnumerable<DynamicReportColumnModel> columns)
        {         
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");

                // Write header
                var col = 1;
                foreach (var column in columns)
                {
                    worksheet.Cells[1, col].Value = column.HeaderName;
                    col++;
                }

                // Write Data
                var row = 2;
                foreach (var record in records)
                {
                    col = 1;
                    foreach (var column in columns)
                    {
                        var propertyInfo = record.GetType().GetProperty(column.ColumnName);

                        if (column.IsEncrypted)
                        {
                            var decryptedValue = cryptoService.Decrypt(propertyInfo.GetValue(record).ToString());
                            worksheet.Cells[row, col].Value = decryptedValue;
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = propertyInfo.GetValue(record);
                        }

                        col++;
                    }

                    row++;
                }

                return package.GetAsByteArray();
            }
        }
    }
}