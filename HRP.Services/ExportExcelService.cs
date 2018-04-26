namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Core.Extensions;
    using HRP.Core.Services;
    using Interfaces;
    using Models.Report;
    using OfficeOpenXml.Style;
    using OfficeOpenXml;
    using Resources;

    [ExcludeFromCodeCoverage]
    public class ExportExcelService : IExportExcelService
    {
        private readonly ICryptoService cryptoService;

        public ExportExcelService(ICryptoService cryptoService)
        {
            this.cryptoService = cryptoService;
        }

        public byte[] ExportDirectBankingReports(DateTime selectedTime, FileInfo fileInfo, PaymentType typeOfPayment, BankInReport data)
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

                worksheet.Cells.AutoFitColumns();

                return xlPackage.GetAsByteArray();
            }
        }

        public byte[] ExportEbankingReports(
            DateTime selectedTime,
            FileInfo fileInfo,
            PaymentType typeOfPayment,
            BankInReport data)
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
                var currentRow = RenderReportRowsForEbankingBankIns(ebankingGroupedByBank, worksheet, startRow);

                // Format for list bank ins
                worksheet.Cells[$"B9:O{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                RenderFlatternedRows(ebankingGroupedByBank, worksheet, currentRow, startRow);

                worksheet.Cells.AutoFitColumns();

                return xlPackage.GetAsByteArray();
            }
        }

        private static void RenderFlatternedRows(
            IEnumerable<IGrouping<string, BankIn>> bankInsGroupedByBank,
            ExcelWorksheet worksheet,
            int currentRow,
            int startRow)
        {
            currentRow++;
            worksheet.Cells[currentRow, 2, currentRow + 3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 2, currentRow + 3, 5].Style.Fill.BackgroundColor.SetColor(1, 189, 215, 238);
            worksheet.Cells[currentRow++, 2].Value = "Company";
            worksheet.Cells[currentRow++, 2].Value = "Bank";
            worksheet.Cells[currentRow++, 2].Value = "EUR Account No.";
            worksheet.Cells[currentRow++, 2].Value = "COST";

            currentRow++;
            worksheet.Cells[currentRow, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 2].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);
            worksheet.Cells[currentRow++, 2].Value = "Payment Purpose";

            currentRow++;

            foreach (IGrouping<string, BankIn> grouping in bankInsGroupedByBank)
            {
                var bankInRows = grouping.ToList();
                var bankInIndex = -1;
                foreach (var bankIn in bankInRows)
                {
                    bankInIndex++;

                    //Staff No.
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Staff No.";
                    worksheet.Cells[currentRow, 4].Value = bankIn.EmployeeId;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Beneficiary
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Beneficiary";
                    worksheet.Cells[currentRow, 4].Value = bankIn.BeneficiaryName;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Account No.
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Account No. ";
                    worksheet.Cells[currentRow, 4].Value = bankIn.AccountNumber;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Bank Name
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Bank Name";
                    worksheet.Cells[currentRow, 4].Value = bankIn.BankName;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Branch
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Branch";
                    worksheet.Cells[currentRow, 4].Value = bankIn.BankBranch;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Address
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Address";
                    worksheet.Cells[currentRow, 4].Value = bankIn.BankAddress;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //SWIFT Codes
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "SWIFT Codes";
                    worksheet.Cells[currentRow, 4].Value = bankIn.BankSwiftCode;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Currency
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Currency";
                    worksheet.Cells[currentRow, 4].Value = bankIn.CurrencyCode;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Amount
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Amount";
                    worksheet.Cells[currentRow, 4].Value = bankIn.AmountDecrypted;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    //Value Date
                    currentRow++;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = "Value Date";
                    worksheet.Cells[currentRow, 4].Formula = $"=IF(ISBLANK($L${bankInIndex + startRow}),\"\",$L${bankInIndex + startRow})";
                    worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);

                    currentRow++;
                }
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
                // group by currency

                var bankInsGroupByCurrency = bankIns.GroupBy(x => x.CurrencyCode);

                foreach (IGrouping<string, BankIn> grouping in bankInsGroupByCurrency)
                {
                    var bankInRows = grouping.ToList();
                    foreach (var bankIn in bankInRows)
                    {
                        worksheet.Cells[rowIndex, 2].Value = rowIndex - 9;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.EmployeeId;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.BankBranch;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.BankAddress;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.BankSwiftCode;
                        worksheet.Cells[rowIndex, 10].Value = bankIn.CurrencyCode;
                        worksheet.Cells[rowIndex, 11].Value = bankIn.AmountDecrypted;

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
            foreach (var currency in bankInCurrencies)
            {
                worksheet.Cells[$"I{currentRow}"].Value = currency.Key;
                worksheet.Cells[$"H{currentRow}"].Value = currency.Sum(x => x.AmountDecrypted);
                currentRow++;
            }

            if (beginRow == currentRow)
            {
                // the bankInCurrencies have no items
                currentRow++;
            }

            worksheet.Cells[$"B{beginRow}:G{currentRow - 1}"].Merge = true;
            worksheet.Cells[$"B{beginRow}"].Value = summaryText;
            worksheet.Cells[$"B{beginRow}"].Style.VerticalAlignment =
                ExcelVerticalAlignment.Center;

            worksheet.Cells[$"B{beginRow}:I{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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
                // group by currency

                var bankInsGroupByCurrency = bankIns.GroupBy(x => x.CurrencyCode);

                foreach (IGrouping<string, BankIn> grouping in bankInsGroupByCurrency)
                {
                    ////int count = 0;
                    var bankInRows = grouping.ToList();
                    foreach (var bankIn in bankInRows)
                    {
                        worksheet.Cells[rowIndex, 2].Value = rowIndex - 9;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.EmployeeId;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.Nickname;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.PaymentTransaction;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, 10].Value = bankIn.CurrencyCode;

                        rowIndex++;
                    }
                }
            }

            return rowIndex;
        }

        public byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report)
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
                var sNo = 1;
                foreach (var employee in report.Employees)
                {
                    var monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                    var monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                    var monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                    var monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                    templateWorksheet.Cells[9, 1, 9, 46].Copy(worksheet.Cells[row, 1, row, 46]);
                    worksheet.Cells[row, 2].Value = sNo;
                    worksheet.Cells[row, 3].Value = employee.DivisionCode;
                    worksheet.Cells[row, 4].Value = employee.DepartmentCode;
                    worksheet.Cells[row, 5].Value = employee.CostCenterCode;
                    worksheet.Cells[row, 6].Value = employee.SectionCode;
                    worksheet.Cells[row, 7].Value = employee.EmployeeCode;
                    worksheet.Cells[row, 8].Value = employee.FullName;
                    worksheet.Cells[row, 9].Value = employee.Nickname;

                    worksheet.Cells[row, 10].Value = monthlyBasicSalary.CurrencyCode;
                    worksheet.Cells[row, 11].Value = cryptoService.Decrypt(monthlyBasicSalary.Salary);
                    worksheet.Cells[row, 12].Value = cryptoService.Decrypt(monthlySalary.Salary);

                    worksheet.Cells[row, 23].Value = monthlyBasicAllowance.CurrencyCode;
                    worksheet.Cells[row, 24].Value = cryptoService.Decrypt(monthlyBasicAllowance.SubsistenceAmount);

                    worksheet.Cells[row, 42].Value = cryptoService.Decrypt(monthlyAllowance.TotalAmount);
                    worksheet.Cells[row, 45].Value = cryptoService.Decrypt(monthlyAllowance.DSTNetSalaryAmount);
                    worksheet.Cells[row, 46].Value = cryptoService.Decrypt(monthlyAllowance.TotalNetAmount);

                    row++;
                    sNo++;
                }

                return package.GetAsByteArray();
            }
        }

        public byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds)
        {
            var selectedGroups = selectedIds.SelectMany(x => ColumnsConfirguration.Columns[x]);
            var columns = selectedGroups.SelectMany(x => x.Columns);

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
                        var propertyInfo = record.GetType().GetProperty(column.PropertyName);

                        if (column.IsEncrypted)
                        {
                            var decryptedValue = cryptoService.Decrypt(propertyInfo.GetValue(record)?.ToString());
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