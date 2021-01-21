namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Enumerations;
    using Core.Extensions;
    using HRP.Core.Constants;
    using HRP.Core.Services;
    using Models.Report;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;
    using Resources;

    public class ExportExcelService : IExportExcelService
    {
        private const int FirstRedRgb = 189;
        private const int FirstGreenRgb = 215;
        private const int FirstBlueRgb = 238;
        private const int SecondRedRgb = 228;
        private const int SecondGreenRgb = 223;
        private const int SecondBlueRgb = 236;
        private readonly ICryptoService cryptoService;

        public ExportExcelService(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        public byte[] ExportDirectBankingReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data, string password = null)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                var worksheet = xlPackage.Workbook.Worksheets.First();

                WriteBankingReportHeader(selectedTime, typeOfPayment, data, worksheet);

                const int startRow = 10;

                if (data.IsExportable)
                {
                    // EBanking
                    var ebankingGroupedByBank = data.EBankingBankIns.GroupBy(x => x.BankName);
                    var currentRow = RenderReportRowsForDirectBankingBankIns(ebankingGroupedByBank, worksheet, startRow);

                    var cashGroupedByBank = data.CashBankIns.GroupBy(x => x.BankName);
                    currentRow = RenderReportRowsForDirectBankingBankIns(cashGroupedByBank, worksheet, currentRow);

                    // Format for list bank ins
                    worksheet.Cells[$"B9:J{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // summary for banking
                    currentRow = currentRow + 1;
                    var eBankingCurrencies = data.EBankingBankIns.GroupBy(x => x.CurrencyCode);

                    currentRow = RenderSummaryRowForBankIns(
                        worksheet,
                        currentRow,
                        Message.GrandTotalBankIn,
                        eBankingCurrencies);

                    // summary for cash
                    currentRow = currentRow + 1;
                    var cashCurrencies = data.CashBankIns.GroupBy(x => x.CurrencyCode);
                    RenderSummaryRowForBankIns(
                        worksheet,
                        currentRow,
                        Message.GrandTotalCash,
                        cashCurrencies);
                }
                else
                {
                    // There is no data
                    WriteNoDataMessage(worksheet, $"B{startRow}:J{startRow}");
                    worksheet.Cells[$"B9:J{startRow}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                worksheet.Cells.AutoFitColumns();

                if (!string.IsNullOrEmpty(password))
                {
                    xlPackage.Encryption.Password = password;
                }

                return xlPackage.GetAsByteArray();
            }
        }

        public byte[] ExportEbankingReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            PaymentType typeOfPayment,
            BankInReport data,
            string password = null)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(fileInfo, true))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();

                WriteBankingReportHeader(selectedTime, typeOfPayment, data, worksheet);

                const int startRow = 10;

                if (data.IsExportable)
                {
                    // EBanking
                    var ebankingGroupedByBank = data.EBankingBankIns.GroupBy(x => x.BankName);
                    var currentRow = RenderReportRowsForEbankingBankIns(ebankingGroupedByBank, worksheet, startRow);

                    // Format for list bank ins
                    worksheet.Cells[$"B9:O{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    RenderFlatternedRows(ebankingGroupedByBank, worksheet, currentRow, startRow);
                }
                else
                {
                    // There is no data
                    WriteNoDataMessage(worksheet, $"B{startRow}:O{startRow}");
                    worksheet.Cells[$"B9:O{startRow}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                worksheet.Cells.AutoFitColumns();

                if (!string.IsNullOrEmpty(password))
                {
                    xlPackage.Encryption.Password = password;
                }

                return xlPackage.GetAsByteArray();
            }
        }

        private static void WriteNoDataMessage(ExcelWorksheet worksheet, string range)
        {
            var noDataCells = worksheet.Cells[range];
            noDataCells.Merge = true;
            noDataCells.Value = Message.ThereIsNoData;
            worksheet.Cells[range.Split(':')[0]].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static void WriteBankingReportHeader(
            DateTime selectedTime,
            PaymentType typeOfPayment,
            BankInReport data,
            ExcelWorksheet worksheet)
        {
            // Set company name
            worksheet.Cells["D2"].Value = data.CompanyName;

            // Set month
            worksheet.Cells["D3"].Value = selectedTime.ToString("MM-yyyy");

            // Set payment type
            worksheet.Cells["D4"].Value = typeOfPayment != 0 ? typeOfPayment.GetDisplayName() : PaymentType.BasicSalary.GetDisplayName();

            // Set selected payment transaction
            worksheet.Cells["D5"].Value = string.Join(",", data.SelectedPaymentTransactions);
        }

        private static void RenderFlatternedRows(
            IEnumerable<IGrouping<string, BankIn>> bankInsGroupedByBank,
            ExcelWorksheet worksheet,
            int currentRow,
            int startRow)
        {
            const int currentCol = 2;
            const int addRow = 3;
            const int forthCol = 4;
            const int endCol = 5;

            currentRow++;
            worksheet.Cells[currentRow, currentCol, currentRow + addRow, endCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, currentCol, currentRow + addRow, endCol].Style.Fill.BackgroundColor.SetColor(1, FirstRedRgb, FirstGreenRgb, FirstBlueRgb);
            worksheet.Cells[currentRow++, currentCol].Value = ViewLang.Company;
            worksheet.Cells[currentRow++, currentCol].Value = ViewLang.Bank;
            worksheet.Cells[currentRow++, currentCol].Value = ViewLang.EuroAccountNumber;
            worksheet.Cells[currentRow++, currentCol].Value = ViewLang.Cost;

            currentRow++;
            worksheet.Cells[currentRow, currentCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, currentCol].Style.Fill.BackgroundColor.SetColor(1, SecondRedRgb, SecondGreenRgb, SecondBlueRgb);
            worksheet.Cells[currentRow++, currentCol].Value = ViewLang.PaymentPurpose;

            currentRow++;
            var bankInIndex = -1;

            foreach (IGrouping<string, BankIn> grouping in bankInsGroupedByBank)
            {
                var bankInRows = grouping.ToList();

                foreach (BankIn bankIn in bankInRows)
                {
                    bankInIndex++;

                    //Staff No.
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.StaffNo, bankIn.IDNo);

                    //Beneficiary
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.Beneficiary, bankIn.BeneficiaryName);

                    //Account No.
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.AccountNo, bankIn.AccountNumber);

                    //Bank Name
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.BankName, bankIn.BankName);

                    //Branch
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.BankBranch, bankIn.BankBranch);

                    //Address
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.Address, bankIn.BankAddress);

                    //SWIFT Codes
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.SwiftCode, bankIn.BankSwiftCode);

                    //Currency
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.Currency, bankIn.CurrencyCode);

                    //Amount
                    WriteFlatternedSection(worksheet, ref currentRow, ViewLang.Amount, bankIn.AmountDecrypted);

                    //Value Date
                    currentRow++;
                    worksheet.Cells[currentRow, currentCol, currentRow, addRow].Merge = true;
                    worksheet.Cells[currentRow, currentCol, currentRow, addRow].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, currentCol, currentRow, addRow].Style.Fill.BackgroundColor.SetColor(1, SecondRedRgb, SecondGreenRgb, SecondBlueRgb);
                    worksheet.Cells[currentRow, currentCol, currentRow, addRow].Value = ViewLang.ValueDate;
                    worksheet.Cells[currentRow, forthCol].Formula = $"=IF(ISBLANK($L${bankInIndex + startRow}),\"\",$L${bankInIndex + startRow})";
                    worksheet.Cells[currentRow, forthCol].Style.Numberformat.Format = "yyyy-mm-dd";

                    currentRow++;
                }
            }
        }

        private static void WriteFlatternedSection(
            ExcelWorksheet worksheet,
            ref int currentRow,
            string title,
            object data)
        {
            const int currentCol = 2;
            const int endCol = 3;
            const int lastCol = 4;

            currentRow++;
            worksheet.Cells[currentRow, currentCol, currentRow, endCol].Merge = true;
            worksheet.Cells[currentRow, currentCol, currentRow, endCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, currentCol, currentRow, endCol].Style.Fill.BackgroundColor.SetColor(1, SecondRedRgb, SecondGreenRgb, SecondBlueRgb);
            worksheet.Cells[currentRow, currentCol, currentRow, endCol].Value = title;
            worksheet.Cells[currentRow, lastCol].Value = data;

            if (data is float)
            {
                // writing amount of money
                worksheet.Cells[currentRow, lastCol].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;
            }
        }

        private static int RenderReportRowsForEbankingBankIns(
            IEnumerable<IGrouping<string, BankIn>> bankInsGroupedByBank,
            ExcelWorksheet worksheet,
            int startRow)
        {
            int rowIndex = startRow;
            foreach (IGrouping<string, BankIn> bankIns in bankInsGroupedByBank)
            {
                IEnumerable<IGrouping<string, BankIn>> bankInsGroupByCurrency = bankIns.GroupBy(x => x.CurrencyCode);
                foreach (IGrouping<string, BankIn> grouping in bankInsGroupByCurrency)
                {
                    var bankInRows = grouping.ToList();
                    foreach (BankIn bankIn in bankInRows)
                    {
                        var colIndex = 2;
                        const int subRowIndex = 9;

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - subRowIndex;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.IDNo;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankBranch;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankAddress;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankSwiftCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.CurrencyCode;

                        worksheet.Cells[rowIndex, colIndex].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;

                        rowIndex++;
                    }
                }
            }

            return rowIndex;
        }

        private static int RenderSummaryRowForBankIns(
            ExcelWorksheet worksheet,
            int currentRow,
            string summaryText,
            IEnumerable<IGrouping<string, BankIn>> bankInCurrencies)
        {
            var beginRow = currentRow;
            foreach (IGrouping<string, BankIn> currency in bankInCurrencies)
            {
                worksheet.Cells[$"I{currentRow}"].Value = currency.Key;
                worksheet.Cells[$"J{currentRow}"].Value = currency.Sum(x => x.AmountDecrypted);
                worksheet.Cells[$"J{currentRow}"].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;

                currentRow++;
            }

            // the bankInCurrencies have no items
            if (beginRow == currentRow)
            {
                currentRow++;
            }

            worksheet.Cells[$"B{beginRow}:H{currentRow - 1}"].Merge = true;
            worksheet.Cells[$"B{beginRow}"].Value = summaryText;
            worksheet.Cells[$"B{beginRow}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells[$"B{beginRow}:J{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

            return currentRow - 1;
        }

        private static int RenderReportRowsForDirectBankingBankIns(
            IEnumerable<IGrouping<string, BankIn>> bankInsGroupedByBank,
            ExcelWorksheet worksheet,
            int startRow)
        {
            int rowIndex = startRow;
            foreach (IGrouping<string, BankIn> bankIns in bankInsGroupedByBank)
            {
                IEnumerable<IGrouping<string, BankIn>> bankInsGroupByCurrency = bankIns.GroupBy(x => x.CurrencyCode);
                foreach (IGrouping<string, BankIn> grouping in bankInsGroupByCurrency)
                {
                    var bankInRows = grouping.ToList();
                    foreach (var bankIn in bankInRows)
                    {
                        var colIndex = 2;
                        const int subRowIndex = 9;

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - subRowIndex;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.IDNo;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.PaymentTransaction;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, colIndex++].Value = bankIn.CurrencyCode;

                        worksheet.Cells[rowIndex, colIndex].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = Constants.NumberFormatNoDecimal;

                        rowIndex++;
                    }
                }
            }

            return rowIndex;
        }

        public byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds, string password = null)
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");
                var columnConfig = ColumnsConfiguration.Columns;
                var selectedGroups = selectedIds.SelectMany(x => columnConfig[x]);
                IEnumerable<MappingColumn> columns = selectedGroups.SelectMany(x => x.Columns);

                // Write header
                worksheet.Cells[1, 1].Value = "ID No";
                var col = 2;
                foreach (MappingColumn column in columns)
                {
                    worksheet.Cells[1, col].Value = column.HeaderName;
                    col++;
                }

                WriteDataDynamicReport(worksheet, records, columns);

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                if (!string.IsNullOrEmpty(password))
                {
                    package.Encryption.Password = password;
                }

                return package.GetAsByteArray();
            }
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high

        private void WriteDataDynamicReport(ExcelWorksheet worksheet, IEnumerable<DynamicReportModel> records, IEnumerable<MappingColumn> columns)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            var row = 2;
            foreach (DynamicReportModel record in records)
            {
                worksheet.Cells[row, 1].Value = record.IDNo;
                var col = 2;

                foreach (MappingColumn column in columns)
                {
                    PropertyInfo propertyInfo = record.GetType().GetProperty(column.PropertyName);
                    var propertyValue = propertyInfo.GetValue(record);
                    ExcelRange cell = worksheet.Cells[row, col];

                    if (column.IsAmount && propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
                    {
                        cell.Value = decimal.Parse(propertyValue.ToString());
                    }
                    else
                    {
                        cell.Value = column.IsEncrypted ? cryptoService.Decrypt(propertyValue?.ToString()) : propertyValue;
                    }

                    if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        cell.Style.Numberformat.Format = "dd/MM/yyyy";
                    }

                    col++;
                }

                row++;
            }
        }
    }
}