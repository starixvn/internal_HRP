namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using HRP.Core.Constants;
    using HRP.Models.Currency;
    using HRP.Models.Employee;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Report;
    using HRP.Models.ServiceModels;
    using HRP.Resources;
    using OfficeOpenXml;

    public interface IExportMonthlyPayrollReportService
    {
        byte[] Export(string templatePath, MonthlyPayrollReport report, string password = null);
    }

    public class ExportMonthlyPayrollReportService : IExportMonthlyPayrollReportService
    {
        private const short TotalCols = 48;

        public byte[] Export(string templatePath, MonthlyPayrollReport report, string password = null)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report");
                    GenerateHeader(templateWorksheet, worksheet, report.SelectedCurrency, report);
                    const short BodyRowIndex = 9;
                    const short ActualBasicSalaryAmountColIndex = 10;
                    const short MonthlyPayrollDeductionColIndex = 14;
                    const short LastTotalNetBasicSalaryColIndex = 20;

                    int row = BodyRowIndex;
                    var serialNo = 1;
                    foreach (EmployeeBasicInfo employee in report.Employees)
                    {
                        var colIndex = 1;
                        MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                        MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                        templateWorksheet.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCols].Copy(worksheet.Cells[row, 1, row, TotalCols]);
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

                        colIndex++;
                        colIndex = FillAllowanceData(worksheet, row, colIndex, monthlyBasicAllowance);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.AdditionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DeductionAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.RoomingChargesAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.FitnessFirstAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.TotalAmount);
                        worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DSTNetSalaryAmount);
                        worksheet.Cells[row, colIndex].Value = ParseToDecimal(monthlyAllowance.TotalNetAmount);

                        CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollDeductionColIndex - 1, Constants.NumberFormatTwoDecimals);
                        CreateConditionalFormating(worksheet, row, MonthlyPayrollDeductionColIndex + 1, toCol: MonthlyPayrollDeductionColIndex + 1, decimalFormat: Constants.NumberFormatTwoDecimals);
                        CreateConditionalFormating(worksheet, row, LastTotalNetBasicSalaryColIndex + 1, TotalCols, Constants.NumberFormatTwoDecimals);

                        row++;
                        serialNo++;
                    }

                    const int addTwoRow = 2;
                    WriteCurrencyExchangeTable(row + 1, templateWorksheet, worksheet, report.SelectedCurrency, report);
                    WriteTotalByCurrencyTable(row + addTwoRow, templateWorksheet, worksheet, report.SelectedCurrency, report);

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    worksheet.View.FreezePanes(BodyRowIndex, 1);

                    if (!string.IsNullOrEmpty(password))
                    {
                        package.Encryption.Password = password;
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        private static void GenerateHeader(ExcelWorksheet templateWorksheet, ExcelWorksheet worksheet, Currency selectedCurrency, MonthlyPayrollReport report)
        {
            const short CompanyRowIndex = 3;
            const short MonthRowIndex = 4;
            const short EndHeaderRowIndex = 8;
            const short CurrencyRowIndex = 7;
            const short CurrencyColIndex = 16;

            // Write header
            templateWorksheet.Cells[1, 1, EndHeaderRowIndex, TotalCols].Copy(worksheet.Cells[1, 1, EndHeaderRowIndex, TotalCols]);
            // Write Data
            worksheet.Cells[CompanyRowIndex, 1].Value = string.Concat(ViewLang.Company, ": ", string.IsNullOrWhiteSpace(report.Company.Name) ? report.Company.Code : report.Company.Name);
            worksheet.Cells[MonthRowIndex, 1].Value = string.Concat(ViewLang.PayrollMonth, ": ", report.Month.ToString("MM-yyyy"));
            worksheet.Cells[CurrencyRowIndex, CurrencyColIndex].Value = selectedCurrency?.CurrencyCode;
        }

        private static int FillAllowanceData(ExcelWorksheet worksheet, int row, int colIndex, MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance)
        {
            worksheet.Cells[row, colIndex++].Value = monthlyBasicAllowance.CurrencyCode;
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

        private static void WriteCurrencyExchangeTable(
            int startRow,
            ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            Currency selectedCurrency,
            MonthlyPayrollReport report)
        {
            const short addTwoRow = 2;
            const short USDColIndex = 3;
            const short TotalCol = 4;
            const short HeaderRowIndex = 17;
            const short BodyRowIndex = 19;
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var selectedCurrencyId = selectedCurrency?.CurrencyId;

            // Write header
            templateWorksheet.Cells[HeaderRowIndex, 1, HeaderRowIndex + 1, TotalCol].Copy(worksheet.Cells[startRow, 1, startRow + 1, TotalCol]);
            if (USDCurrencyId != selectedCurrencyId)
            {
                worksheet.Cells[startRow + 1, TotalCol].Value = selectedCurrency?.CurrencyCode;
            }

            // Write data
            var rowWithStyle = templateWorksheet.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCol];
            var row = startRow + addTwoRow;
            foreach (Currency currency in report.CurrencyList)
            {
                rowWithStyle.Copy(worksheet.Cells[row, 1, row, TotalCol]);
                CurrencyExchangeRateModel exchangeRateToUSD = GetCurrencyExchangeRate(report.CurrencyExchangeRate, currency.CurrencyId, USDCurrencyId);
                worksheet.Cells[row, 1].Value = currency.CurrencyCode;
                worksheet.Cells[row, USDColIndex].Value = exchangeRateToUSD?.ExchangeRate;
                if (USDCurrencyId != selectedCurrencyId)
                {
                    CurrencyExchangeRateModel exchangeRateToMYR = GetCurrencyExchangeRate(report.CurrencyExchangeRate, currency.CurrencyId, selectedCurrencyId);
                    worksheet.Cells[row, TotalCol].Value = exchangeRateToMYR?.ExchangeRate;
                }

                CreateConditionalFormating(worksheet, row, USDColIndex, TotalCol, Constants.NumberFormatFiveDecimals);
                row++;
            }
        }

        private static void WriteTotalByCurrencyTable(
            int startRow,
            ExcelWorksheet templateWorksheet,
            ExcelWorksheet worksheet,
            Currency selectedCurrency,
            MonthlyPayrollReport report)
        {
            const short USDColIndex = 21;
            const short SubsistenceColIndex = 24;
            const short HeaderRowIndex = 18;
            const short BodyRowIndex = 19;
            const short StartColIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;
            const short MonthlyPayrollDeductionColIndex = 14;
            const short LastTotalNetBasicSalaryColIndex = 20;

            // Write header
            templateWorksheet.Cells[HeaderRowIndex, StartColIndex, HeaderRowIndex, TotalCols].Copy(worksheet.Cells[startRow, StartColIndex, startRow, TotalCols]);

            // Write data
            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var rowWithStyle = templateWorksheet.Cells[BodyRowIndex, StartColIndex, BodyRowIndex, TotalCols];
            var row = startRow + 1;
            foreach (Currency currency in report.CurrencyList)
            {
                var colIndex = StartColIndex;
                rowWithStyle.Copy(worksheet.Cells[row, colIndex, row, TotalCols]);

                IEnumerable<MonthlyBasicSalaryModel> basicSalaryList = report.MonthlyBasicSalary.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlySalaryModel> salaryList = report.MonthlySalary.Where(x => x.CurrencyId == currency.CurrencyId);
                worksheet.Cells[row, colIndex++].Value = currency.CurrencyCode;
                worksheet.Cells[row, colIndex++].Value = basicSalaryList.Sum(x => ParseToDecimal(x.Salary));
                colIndex++;
                worksheet.Cells[row, colIndex++].Value = basicSalaryList.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.AdditionAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.DeductionAmount));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x => ParseToDecimal(x.Salary));

                if (currency.CurrencyId == selectedCurrency?.CurrencyId)
                {
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.EbankingAmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.DirectBanking1AmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.DirectBanking2AmountInMYR);
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x => x.CashAmountInMYR);
                    worksheet.Cells[row, colIndex].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInMYR);
                }

                if (currency.CurrencyId == USDCurrencyId)
                {
                    worksheet.Cells[row, USDColIndex].Value = report.MonthlySalary.Sum(x => x.SalaryAmountInUSD);
                }

                colIndex = SubsistenceColIndex;
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

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollDeductionColIndex - 1, Constants.NumberFormatTwoDecimals);
                CreateConditionalFormating(worksheet, row, MonthlyPayrollDeductionColIndex + 1, toCol: MonthlyPayrollDeductionColIndex + 1, decimalFormat: Constants.NumberFormatTwoDecimals);
                CreateConditionalFormating(worksheet, row, LastTotalNetBasicSalaryColIndex + 1, TotalCols, Constants.NumberFormatTwoDecimals);

                row++;
            }
        }

        private static CurrencyExchangeRateModel GetCurrencyExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int? fromCurrencyId,
            int? toCurrencyId)
        {
            return currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);
        }

        private static void CreateConditionalFormating(ExcelWorksheet worksheet, int fromRow, int fromCol, int toCol, string decimalFormat)
        {
            var cond1 = worksheet.ConditionalFormatting.AddExpression(worksheet.Cells[fromRow, fromCol, fromRow, toCol]);
            cond1.Style.NumberFormat.Format = Constants.NumberFormatNoDecimal;
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
    }
}