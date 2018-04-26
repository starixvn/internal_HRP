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
                var currentRow = RenderReportRowsForBankIns(ebankingGroupedByBank, worksheet, startRow);

                var cashGroupedByBank = data.CashBankIns.GroupBy(x => x.BankName);
                currentRow = RenderReportRowsForBankIns(cashGroupedByBank, worksheet, currentRow);

                // Format for list bank ins
                worksheet.Cells[$"B9:I{currentRow - 1}"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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
                    ////int count = 0;
                    var bankInRows = grouping.ToList();
                    foreach (var bankIn in bankInRows)
                    {
                        worksheet.Cells[rowIndex, 2].Value = rowIndex - 9;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.Nickname;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.PaymentTransaction;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.CurrencyCode;

                        rowIndex++;
                    }

                    ////// Create sumary row for a bank
                    ////var summaryRow = worksheet.Cells[rowIndex, 2, rowIndex, 7];
                    ////summaryRow.Merge = true;

                    ////// TODO: move to resource when user confirmed
                    ////summaryRow.Value = string.Format("Total ({0})", grouping.Key);

                    ////worksheet.Cells[rowIndex, 8].FormulaR1C1 =
                    ////    $"=SUM(R{(rowIndex - grouping.Count())}C8:R{rowIndex - 1}C8)";

                    ////rowIndex++;
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
            var selectedGroups = ColumnsConfirguration.Columns.Where(x => selectedIds.Any(s => s == (int) x.Key )).SelectMany(x => x.Value);
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