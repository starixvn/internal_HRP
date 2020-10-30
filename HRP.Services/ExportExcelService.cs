namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using HRP.Core.Services;
    using Models.Currency;
    using Models.Employee;
    using Models.MonthlyPayroll;
    using Models.Report;
    using Models.ServiceModels;
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
        private const string NumberFormatNoDecimal = "#,##0";
        private const string NumberFormatTwoDecimals = "#,##0.##";
        private const string NumberFormatFiveDecimals = "#,##0.#####";
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
                worksheet.Cells[currentRow, lastCol].Style.Numberformat.Format = NumberFormatNoDecimal;
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
                        worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = NumberFormatNoDecimal;

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
                worksheet.Cells[$"J{currentRow}"].Style.Numberformat.Format = NumberFormatNoDecimal;

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
                        worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = NumberFormatNoDecimal;

                        rowIndex++;
                    }
                }
            }

            return rowIndex;
        }

        #region MonthlyPayrollReport

        public byte[] ExportMonthlyPayrollReport(string templatePath, MonthlyPayrollReport report, string password = null)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report");
                    GenerateHeader(templateWorksheet, worksheet, report);
                    const int tenthCol = 10;
                    const int endRow = 8;
                    const int endCol = 48;

                    var row = 9;
                    var serialNo = 1;
                    foreach (EmployeeBasicInfo employee in report.Employees)
                    {
                        var colIndex = 1;
                        MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                        templateWorksheet.Cells[endRow + 1, 1, endRow + 1, endCol].Copy(worksheet.Cells[row, 1, row, endCol]);
                        worksheet.Cells[row, colIndex++].Value = serialNo;
                        worksheet.Cells[row, colIndex++].Value = employee.DivisionCode;
                        worksheet.Cells[row, colIndex++].Value = employee.DepartmentCode;
                        worksheet.Cells[row, colIndex++].Value = employee.CostCenterCode;
                        worksheet.Cells[row, colIndex++].Value = employee.SectionCode;
                        worksheet.Cells[row, colIndex++].Value = employee.IDNo;
                        worksheet.Cells[row, colIndex++].Value = employee.FullName;
                        worksheet.Cells[row, colIndex++].Value = employee.Nickname;
                        worksheet.Cells[row, colIndex++].Value = monthlyBasicSalary.CurrencyCode;
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicSalary.Salary);
                        worksheet.Cells[row, colIndex++].Value = monthlyBasicSalary.CurrencyCode;
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicSalary.AfterAdjustmentAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.AdditionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.DeductionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.Salary);
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.EbankingAmountInMYR;
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.DirectBanking1AmountInMYR;
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.DirectBanking2AmountInMYR;
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.CashAmountInMYR;
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.SalaryAmountInMYR;
                        worksheet.Cells[row, colIndex++].Value = monthlySalary.SalaryAmountInUSD;
                        worksheet.Cells[row, colIndex++].Value = monthlyBasicAllowance.CurrencyCode;
                        colIndex = FillAllowanceData(worksheet, row, colIndex, monthlyBasicAllowance);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.AdditionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DeductionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.RoomingChargesAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.FitnessFirstAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.TotalAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DSTNetSalaryAmount);
                        worksheet.Cells[row, colIndex].Value = ParseToDecimal(monthlyAllowance.TotalNetAmount);

                        CreateConditionalFormating(worksheet, row, tenthCol, colIndex, NumberFormatTwoDecimals);

                        row++;
                        serialNo++;
                    }

                    const int addTwoRow = 2;
                    WriteCurrencyExchangeTable(row + 1, templateWorksheet, worksheet, report);
                    WriteTotalByCurrencyTable(row + addTwoRow, templateWorksheet, worksheet, report);

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    if (!string.IsNullOrEmpty(password))
                    {
                        package.Encryption.Password = password;
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        private static void GenerateHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            const int thirdRow = 3;
            const int forthRow = 4;
            const int endRow = 8;
            const int endCol = 48;

            // Write header
            templateWorksheet.Cells[1, 1, endRow, endCol].Copy(worksheet.Cells[1, 1, endRow, endCol]);
            // Write Data
            worksheet.Cells[thirdRow, 1].Value = string.Concat(ViewLang.Company, ": ", string.IsNullOrWhiteSpace(report.Company.Name) ? report.Company.Code : report.Company.Name);
            worksheet.Cells[forthRow, 1].Value = string.Concat(ViewLang.PayrollMonth, ": ", report.Month.ToString("MM-yyyy"));
        }

        private static int FillAllowanceData(ExcelWorksheet worksheet, int row, int colIndex, MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance)
        {
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.SubsistenceAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HomePassageAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HousingAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.MobilePhoneAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HPIAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.DutyMealAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.SecondmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.OthersAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.TotalAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.SubsistenceAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HomePassageAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HousingAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.MobilePhoneAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.HPIAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.DutyMealAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.SecondmentAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.OthersAfterAdjustmentAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.AfterAdjustmentAmount);
            return colIndex;
        }

        private static void WriteCurrencyExchangeTable(int startRow, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            const int addTwoRow = 2;
            const int thirdCol = 3;
            const int endCol = 4;
            const int seventeenthRow = 17;
            const int eighteenthRow = 18;
            const int nineteenthRow = 19;

            // Write header
            templateWorksheet.Cells[seventeenthRow, 1, eighteenthRow, endCol].Copy(worksheet.Cells[startRow, 1, startRow + 1, endCol]);

            // Write data
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var MYRCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var rowWithStyle = templateWorksheet.Cells[nineteenthRow, 1, nineteenthRow, endCol];
            var row = startRow + addTwoRow;
            foreach (Currency currency in report.CurrencyList)
            {
                rowWithStyle.Copy(worksheet.Cells[row, 1, row, endCol]);
                CurrencyExchangeRateModel exchangeRateToUSD = report.CurrencyExchangeRate.FirstOrDefault(x => x.FromCurrencyId == currency.CurrencyId && x.ToCurrencyId == USDCurrencyId);
                CurrencyExchangeRateModel exchangeRateToMYR = report.CurrencyExchangeRate.FirstOrDefault(x => x.FromCurrencyId == currency.CurrencyId && x.ToCurrencyId == MYRCurrencyId);

                worksheet.Cells[row, 1].Value = currency.CurrencyCode;
                worksheet.Cells[row, thirdCol].Value = exchangeRateToUSD?.ExchangeRate;
                worksheet.Cells[row, endCol].Value = exchangeRateToMYR?.ExchangeRate;

                CreateConditionalFormating(worksheet, row, thirdCol, endCol, NumberFormatFiveDecimals);

                row++;
            }
        }

        private static void WriteTotalByCurrencyTable(int startRow, ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            const int firstAddCol = 5;
            const int secondAddCol = 3;
            const int tenthCol = 10;
            const int eighteenthRow = 18;
            const int nineteenthRow = 19;
            const int lastCol = 48;
            var colIndex = 9;

            // Write header
            templateWorksheet.Cells[eighteenthRow, colIndex, eighteenthRow, lastCol].Copy(worksheet.Cells[startRow, colIndex, startRow, lastCol]);

            // Write data
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var MYRCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var rowWithStyle = templateWorksheet.Cells[nineteenthRow, colIndex, nineteenthRow, lastCol];
            var row = startRow + 1;
            foreach (Currency currency in report.CurrencyList)
            {
                rowWithStyle.Copy(worksheet.Cells[row, colIndex, row, lastCol]);

                IEnumerable<MonthlyBasicSalaryModel> basicSalaryList = report.MonthlyBasicSalary.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlySalaryModel> salaryList = report.MonthlySalary.Where(x => x.CurrencyId == currency.CurrencyId);
                worksheet.Cells[row, colIndex++].Value = currency.CurrencyCode;
                worksheet.Cells[row, colIndex++].Value = basicSalaryList.Sum(x => ParseToDecimal(x.Salary));
                worksheet.Cells[row, colIndex++].Value = basicSalaryList.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.AdditionAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.DeductionAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.Salary));

                if (currency.CurrencyId == MYRCurrencyId)
                {
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.EbankingAmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.DirectBanking1AmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.DirectBanking2AmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.CashAmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInMYR);
                }
                else
                {
                    colIndex += firstAddCol;
                }

                if (currency.CurrencyId == USDCurrencyId)
                {
                    worksheet.Cells[row, colIndex].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInUSD);
                }

                colIndex += secondAddCol;
                IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowanceList = report.MonthlyBasicAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlyAllowanceModel> allowanceList = report.MonthlyAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SubsistenceAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HomePassageAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HousingAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.MobilePhoneAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HPIAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.DutyMealAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SecondmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.OthersAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.TotalAmount));

                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SubsistenceAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HomePassageAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HousingAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.MobilePhoneAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.HPIAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.DutyMealAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.SecondmentAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.OthersAfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));

                worksheet.Cells[row, colIndex++].Value = allowanceList.Sum(x => ParseToDecimal(x.AdditionAmount));
                worksheet.Cells[row, colIndex++].Value = allowanceList.Sum(x => ParseToDecimal(x.DeductionAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.RoomingChargesAmount));
                worksheet.Cells[row, colIndex++].Value = basicAllowanceList.Sum(x => ParseToDecimal(x.FitnessFirstAmount));
                worksheet.Cells[row, colIndex++].Value = allowanceList.Sum(x => ParseToDecimal(x.TotalAmount));
                worksheet.Cells[row, colIndex++].Value = allowanceList.Sum(x => ParseToDecimal(x.DSTNetSalaryAmount));
                worksheet.Cells[row, colIndex].Value = allowanceList.Sum(x => ParseToDecimal(x.TotalNetAmount));

                CreateConditionalFormating(worksheet, row, tenthCol, colIndex, NumberFormatTwoDecimals);

                row++;
            }
        }

        private static void CreateConditionalFormating(ExcelWorksheet worksheet, int fromRow, int fromCol, int toCol, string decimalFormat)
        {
            var cond1 = worksheet.ConditionalFormatting.AddExpression(worksheet.Cells[fromRow, fromCol, fromRow, toCol]);
            cond1.Style.NumberFormat.Format = NumberFormatNoDecimal;
            cond1.Formula = string.Format("IF((MOD(ROUND({0}, 2), 1) = 0),1,0)", worksheet.Cells[fromRow, fromCol, fromRow, toCol].Address);

            var cond2 = worksheet.ConditionalFormatting.AddExpression(worksheet.Cells[fromRow, fromCol, fromRow, toCol]);
            cond2.Style.NumberFormat.Format = decimalFormat;
            cond2.Formula = string.Format("IF((MOD(ROUND({0}, 2), 1) <> 0),1,0)", worksheet.Cells[fromRow, fromCol, fromRow, toCol].Address);
        }

        private static decimal ParseToDecimal(string amount)
        {
            decimal.TryParse(amount, out decimal value);
            return value;
        }

        #endregion MonthlyPayrollReport

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