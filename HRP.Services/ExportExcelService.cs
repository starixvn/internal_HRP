namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using HRP.Core.Services;
    using Interfaces;
    using Models.Currency;
    using Models.Employee;
    using Models.MonthlyPayroll;
    using Models.Report;
    using OfficeOpenXml.Style;
    using OfficeOpenXml;
    using Resources;

    public class ExportExcelService : IExportExcelService
    {
        private const string NumberFormatWithThousandSeperatorAndNoDecimal = "#,##0";
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

                WriteBankingReportHeader(selectedTime, typeOfPayment, data, worksheet);

                var startRow = 10;

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

                WriteBankingReportHeader(selectedTime, typeOfPayment, data, worksheet);

                var startRow = 10;

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
            currentRow++;
            worksheet.Cells[currentRow, 2, currentRow + 3, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 2, currentRow + 3, 5].Style.Fill.BackgroundColor.SetColor(1, 189, 215, 238);
            worksheet.Cells[currentRow++, 2].Value = ViewLang.Company;
            worksheet.Cells[currentRow++, 2].Value = ViewLang.Bank;
            worksheet.Cells[currentRow++, 2].Value = ViewLang.EuroAccountNumber;
            worksheet.Cells[currentRow++, 2].Value = ViewLang.Cost;

            currentRow++;
            worksheet.Cells[currentRow, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 2].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);
            worksheet.Cells[currentRow++, 2].Value = ViewLang.PaymentPurpose;

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
                    worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);
                    worksheet.Cells[currentRow, 2, currentRow, 3].Value = ViewLang.ValueDate;
                    worksheet.Cells[currentRow, 4].Formula = $"=IF(ISBLANK($L${bankInIndex + startRow}),\"\",$L${bankInIndex + startRow})";
                    worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "yyyy-mm-dd";

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
            currentRow++;
            worksheet.Cells[currentRow, 2, currentRow, 3].Merge = true;
            worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 2, currentRow, 3].Style.Fill.BackgroundColor.SetColor(1, 228, 223, 236);
            worksheet.Cells[currentRow, 2, currentRow, 3].Value = title;
            worksheet.Cells[currentRow, 4].Value = data;

            if (data is float)
            {
                // writing amount of money
                worksheet.Cells[currentRow, 4].Style.Numberformat.Format = NumberFormatWithThousandSeperatorAndNoDecimal;
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
                        worksheet.Cells[rowIndex, 2].Value = rowIndex - 9;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.IDNo;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.BankBranch;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.BankAddress;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.BankSwiftCode;
                        worksheet.Cells[rowIndex, 10].Value = bankIn.CurrencyCode;

                        worksheet.Cells[rowIndex, 11].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = NumberFormatWithThousandSeperatorAndNoDecimal;

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
                worksheet.Cells[$"J{currentRow}"].Style.Numberformat.Format = NumberFormatWithThousandSeperatorAndNoDecimal;

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
                        worksheet.Cells[rowIndex, 2].Value = rowIndex - 9;
                        worksheet.Cells[rowIndex, 3].Value = bankIn.IDNo;
                        worksheet.Cells[rowIndex, 4].Value = bankIn.BeneficiaryName;
                        worksheet.Cells[rowIndex, 5].Value = bankIn.Nickname;
                        worksheet.Cells[rowIndex, 6].Value = bankIn.PaymentTransaction;
                        worksheet.Cells[rowIndex, 7].Value = bankIn.BankName;
                        worksheet.Cells[rowIndex, 8].Value = bankIn.AccountNumber;
                        worksheet.Cells[rowIndex, 9].Value = bankIn.CurrencyCode;

                        worksheet.Cells[rowIndex, 10].Value = bankIn.AmountDecrypted;
                        worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = NumberFormatWithThousandSeperatorAndNoDecimal;

                        rowIndex++;
                    }
                }
            }

            return rowIndex;
        }

        #region MonthlyPayrollReport

        public byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report");

                    // Write header
                    templateWorksheet.Cells[1, 1, 8, 48].Copy(worksheet.Cells[1, 1, 8, 48]);

                    // Write Data
                    worksheet.Cells[3, 1].Value = string.Concat(ViewLang.Company, ": ", string.IsNullOrWhiteSpace(report.Company.Name) ? report.Company.Code : report.Company.Name);
                    worksheet.Cells[4, 1].Value = string.Concat(ViewLang.PayrollMonth, ": ", report.Month.ToString("MM-yyyy"));
                    var row = 9;
                    var serialNo = 1;
                    foreach (EmployeeBasicInfo employee in report.Employees)
                    {
                        MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                        templateWorksheet.Cells[9, 1, 9, 48].Copy(worksheet.Cells[row, 1, row, 48]);
                        worksheet.Cells[row, 1].Value = serialNo;
                        worksheet.Cells[row, 2].Value = employee.DivisionCode;
                        worksheet.Cells[row, 3].Value = employee.DepartmentCode;
                        worksheet.Cells[row, 4].Value = employee.CostCenterCode;
                        worksheet.Cells[row, 5].Value = employee.SectionCode;
                        worksheet.Cells[row, 6].Value = employee.IDNo;
                        worksheet.Cells[row, 7].Value = employee.FullName;
                        worksheet.Cells[row, 8].Value = employee.Nickname;

                        worksheet.Cells[row, 9].Value = monthlyBasicSalary.CurrencyCode;
                        worksheet.Cells[row, 10].Value = ParseToDecimal(monthlyBasicSalary.Salary);
                        worksheet.Cells[row, 11].Value = monthlyBasicSalary.CurrencyCode;
                        worksheet.Cells[row, 12].Value = ParseToDecimal(monthlyBasicSalary.AfterAdjustmentAmount);

                        worksheet.Cells[row, 13].Value = ParseToDecimal(monthlySalary.AdditionAmount);
                        worksheet.Cells[row, 14].Value = ParseToDecimal(monthlySalary.DeductionAmount);
                        worksheet.Cells[row, 15].Value = ParseToDecimal(monthlySalary.Salary);
                        worksheet.Cells[row, 16].Value = monthlySalary.EbankingAmountInMYR;
                        worksheet.Cells[row, 17].Value = monthlySalary.DirectBanking1AmountInMYR;
                        worksheet.Cells[row, 18].Value = monthlySalary.DirectBanking2AmountInMYR;
                        worksheet.Cells[row, 19].Value = monthlySalary.CashAmountInMYR;
                        worksheet.Cells[row, 20].Value = monthlySalary.SalaryAmountInMYR;
                        worksheet.Cells[row, 21].Value = monthlySalary.SalaryAmountInUSD;

                        worksheet.Cells[row, 23].Value = monthlyBasicAllowance.CurrencyCode;
                        worksheet.Cells[row, 24].Value = ParseToDecimal(monthlyBasicAllowance.SubsistenceAmount);
                        worksheet.Cells[row, 25].Value = ParseToDecimal(monthlyBasicAllowance.HomePassageAmount);
                        worksheet.Cells[row, 26].Value = ParseToDecimal(monthlyBasicAllowance.HousingAmount);
                        worksheet.Cells[row, 27].Value = ParseToDecimal(monthlyBasicAllowance.MobilePhoneAmount);
                        worksheet.Cells[row, 28].Value = ParseToDecimal(monthlyBasicAllowance.HPIAmount);
                        worksheet.Cells[row, 29].Value = ParseToDecimal(monthlyBasicAllowance.DutyMealAmount);
                        worksheet.Cells[row, 30].Value = ParseToDecimal(monthlyBasicAllowance.SecondmentAmount);
                        worksheet.Cells[row, 31].Value = ParseToDecimal(monthlyBasicAllowance.OthersAmount);
                        worksheet.Cells[row, 32].Value = ParseToDecimal(monthlyBasicAllowance.TotalAmount);

                        worksheet.Cells[row, 33].Value = ParseToDecimal(monthlyBasicAllowance.SubsistenceAfterAdjustmentAmount);
                        worksheet.Cells[row, 34].Value = ParseToDecimal(monthlyBasicAllowance.HomePassageAfterAdjustmentAmount);
                        worksheet.Cells[row, 35].Value = ParseToDecimal(monthlyBasicAllowance.HousingAfterAdjustmentAmount);
                        worksheet.Cells[row, 36].Value = ParseToDecimal(monthlyBasicAllowance.MobilePhoneAfterAdjustmentAmount);
                        worksheet.Cells[row, 37].Value = ParseToDecimal(monthlyBasicAllowance.HPIAfterAdjustmentAmount);
                        worksheet.Cells[row, 38].Value = ParseToDecimal(monthlyBasicAllowance.DutyMealAfterAdjustmentAmount);
                        worksheet.Cells[row, 39].Value = ParseToDecimal(monthlyBasicAllowance.SecondmentAfterAdjustmentAmount);
                        worksheet.Cells[row, 40].Value = ParseToDecimal(monthlyBasicAllowance.OthersAfterAdjustmentAmount);
                        worksheet.Cells[row, 41].Value = ParseToDecimal(monthlyBasicAllowance.AfterAdjustmentAmount);

                        worksheet.Cells[row, 42].Value = ParseToDecimal(monthlyAllowance.AdditionAmount);
                        worksheet.Cells[row, 43].Value = ParseToDecimal(monthlyAllowance.DeductionAmount);
                        worksheet.Cells[row, 44].Value = ParseToDecimal(monthlyBasicAllowance.RoomingChargesAmount);
                        worksheet.Cells[row, 45].Value = ParseToDecimal(monthlyBasicAllowance.FitnessFirstAmount);
                        worksheet.Cells[row, 46].Value = ParseToDecimal(monthlyAllowance.TotalAmount);
                        worksheet.Cells[row, 47].Value = ParseToDecimal(monthlyAllowance.DSTNetSalaryAmount);
                        worksheet.Cells[row, 48].Value = ParseToDecimal(monthlyAllowance.TotalNetAmount);
                        row++;
                        serialNo++;
                    }

                    WriteCurrencyExchangeTable(row + 1, templateWorksheet, worksheet, report);
                    WriteTotalByCurrencyTable(row + 2, templateWorksheet, worksheet, report);

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    return package.GetAsByteArray();
                }
            }
        }

        private static void WriteCurrencyExchangeTable(int startRow, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            // Write header
            templateWorksheet.Cells[17, 1, 18, 4].Copy(worksheet.Cells[startRow, 1, startRow + 1, 4]);

            // Write data
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var MYRCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var rowWithStyle = templateWorksheet.Cells[19, 1, 19, 4];
            var row = startRow + 2;
            foreach (Currency currency in report.CurrencyList)
            {
                rowWithStyle.Copy(worksheet.Cells[row, 1, row, 4]);
                CurrencyExchangeRateModel exchangeRateToUSD = report.CurrencyExchangeRate.FirstOrDefault(x => x.FromCurrencyId == currency.CurrencyId && x.ToCurrencyId == USDCurrencyId);
                CurrencyExchangeRateModel exchangeRateToMYR = report.CurrencyExchangeRate.FirstOrDefault(x => x.FromCurrencyId == currency.CurrencyId && x.ToCurrencyId == MYRCurrencyId);

                worksheet.Cells[row, 1].Value = currency.CurrencyCode;
                worksheet.Cells[row, 3].Value = exchangeRateToUSD?.ExchangeRate;
                worksheet.Cells[row, 4].Value = exchangeRateToMYR?.ExchangeRate;
                row++;
            }
        }

        private static void WriteTotalByCurrencyTable(int startRow, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            // Write header
            templateWorksheet.Cells[18, 9, 18, 48].Copy(worksheet.Cells[startRow, 9, startRow, 48]);

            // Write data
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var MYRCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var rowWithStyle = templateWorksheet.Cells[19, 9, 19, 48];
            var row = startRow + 1;
            foreach (Currency currency in report.CurrencyList)
            {
                rowWithStyle.Copy(worksheet.Cells[row, 9, row, 48]);

                IEnumerable<MonthlyBasicSalaryModel> basicSalaryList = report.MonthlyBasicSalary.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlySalaryModel> salaryList = report.MonthlySalary.Where(x => x.CurrencyId == currency.CurrencyId);
                worksheet.Cells[row, 9].Value = currency.CurrencyCode;
                worksheet.Cells[row, 10].Value = basicSalaryList.Sum(x => ParseToDecimal(x.Salary));
                worksheet.Cells[row, 12].Value = basicSalaryList.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));
                worksheet.Cells[row, 13].Value = salaryList.Sum(x => ParseToDecimal(x.AdditionAmount));
                worksheet.Cells[row, 14].Value = salaryList.Sum(x => ParseToDecimal(x.DeductionAmount));
                worksheet.Cells[row, 15].Value = salaryList.Sum(x => ParseToDecimal(x.Salary));

                if (currency.CurrencyId == MYRCurrencyId)
                {
                    worksheet.Cells[row, 16].Value = report.MonthlySalary.Sum(x => x.EbankingAmountInMYR);
                    worksheet.Cells[row, 17].Value = report.MonthlySalary.Sum(x => x.DirectBanking1AmountInMYR);
                    worksheet.Cells[row, 18].Value = report.MonthlySalary.Sum(x => x.DirectBanking2AmountInMYR);
                    worksheet.Cells[row, 19].Value = report.MonthlySalary.Sum(x => x.CashAmountInMYR);
                    worksheet.Cells[row, 20].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInMYR);
                }

                if (currency.CurrencyId == USDCurrencyId)
                {
                    worksheet.Cells[row, 21].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInUSD);
                }

                IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowanceList = report.MonthlyBasicAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlyAllowanceModel> allowanceList = report.MonthlyAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                worksheet.Cells[row, 24].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SubsistenceAmount));
                worksheet.Cells[row, 25].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HomePassageAmount));
                worksheet.Cells[row, 26].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HousingAmount));
                worksheet.Cells[row, 27].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.MobilePhoneAmount));
                worksheet.Cells[row, 28].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HPIAmount));
                worksheet.Cells[row, 29].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.DutyMealAmount));
                worksheet.Cells[row, 30].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SecondmentAmount));
                worksheet.Cells[row, 31].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.OthersAmount));
                worksheet.Cells[row, 32].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.TotalAmount));

                worksheet.Cells[row, 33].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SubsistenceAfterAdjustmentAmount));
                worksheet.Cells[row, 34].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HomePassageAfterAdjustmentAmount));
                worksheet.Cells[row, 35].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HousingAfterAdjustmentAmount));
                worksheet.Cells[row, 36].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.MobilePhoneAfterAdjustmentAmount));
                worksheet.Cells[row, 37].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HPIAfterAdjustmentAmount));
                worksheet.Cells[row, 38].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.DutyMealAfterAdjustmentAmount));
                worksheet.Cells[row, 39].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SecondmentAfterAdjustmentAmount));
                worksheet.Cells[row, 40].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.OthersAfterAdjustmentAmount));
                worksheet.Cells[row, 41].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));

                worksheet.Cells[row, 42].Value = allowanceList.Sum(x => ParseToDecimal(x.AdditionAmount));
                worksheet.Cells[row, 43].Value = allowanceList.Sum(x => ParseToDecimal(x.DeductionAmount));
                worksheet.Cells[row, 44].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.RoomingChargesAmount));
                worksheet.Cells[row, 45].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.FitnessFirstAmount));
                worksheet.Cells[row, 46].Value = allowanceList.Sum(x => ParseToDecimal(x.TotalAmount));
                worksheet.Cells[row, 47].Value = allowanceList.Sum(x => ParseToDecimal(x.DSTNetSalaryAmount));
                worksheet.Cells[row, 48].Value = allowanceList.Sum(x => ParseToDecimal(x.TotalNetAmount));

                row++;
            }
        }

        private static decimal ParseToDecimal(string amount)
        {
            decimal.TryParse(amount, out decimal value);
            return value;
        }

        #endregion MonthlyPayrollReport

        public byte[] ExportDynamicReport(IEnumerable<DynamicReportModel> records, IEnumerable<int> selectedIds)
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");
                var columnConfig = ColumnsConfiguration.Columns;
                var selectedGroups = selectedIds.SelectMany(x => columnConfig[x]);
                var columns = selectedGroups.SelectMany(x => x.Columns);
                // Write header
                worksheet.Cells[1, 1].Value = "ID No";
                var col = 2;
                foreach (var column in columns)
                {
                    worksheet.Cells[1, col].Value = column.HeaderName;
                    col++;
                }

                // Write Data
                var row = 2;
                foreach (DynamicReportModel record in records)
                {
                    worksheet.Cells[row, 1].Value = record.IDNo;
                    col = 2;

                    foreach (var column in columns)
                    {
                        var propertyInfo = record.GetType().GetProperty(column.PropertyName);
                        var propertyValue = propertyInfo.GetValue(record);
                        var cell = worksheet.Cells[row, col];

                        cell.Value = column.IsEncrypted ? cryptoService.Decrypt(propertyValue?.ToString()) : propertyValue;

                        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                        {
                            cell.Style.Numberformat.Format = "dd/MM/yyyy";
                        }

                        col++;
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}