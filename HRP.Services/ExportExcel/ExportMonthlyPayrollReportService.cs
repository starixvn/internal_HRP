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
        private const short MonthlyPayrollAdditionColIndex = 13;

        public byte[] Export(string templatePath, MonthlyPayrollReport report, string password = null)
        {
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report");
                    WriteDataSheetBySelectedCurrency(template, worksheet, report);

                    template = templatePackage.Workbook.Worksheets[1];
                    worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report (1)");
                    WriteDataSheetByOriginalCurrency(template, worksheet, report);

                    template = templatePackage.Workbook.Worksheets[1];
                    worksheet = package.Workbook.Worksheets.Add("Monthly Payroll Report (2)");
                    WriteDataSheetByOriginalInputCurrency(template, worksheet, report);

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        package.Encryption.Password = password;
                    }

                    return package.GetAsByteArray();
                }
            }
        }

        private static void WriteDataSheetBySelectedCurrency(ExcelWorksheet template, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            WriteHeader(template, worksheet, report.SelectedCurrency, report);
            const short BodyRowIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            int row = BodyRowIndex;
            var serialNo = 1;
            foreach (EmployeeBasicInfo employee in report.Employees)
            {
                var colIndex = 1;
                MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCols].Copy(worksheet.Cells[row, 1, row, TotalCols]);
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
                worksheet.Cells[row, colIndex++].Value = monthlySalary.DirectBanking1AccountName;
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.EbankingAmount, monthlySalary.EbankingCurrencyId, report.SelectedCurrency?.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.DirectBanking1Amount, monthlySalary.DirectBanking1CurrencyId, report.SelectedCurrency?.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.DirectBanking2Amount, monthlySalary.DirectBanking2CurrencyId, report.SelectedCurrency?.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.CashAmount, monthlySalary.CashCurrencyId, report.SelectedCurrency?.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.Salary, monthlySalary.CurrencyId, report.SelectedCurrency?.CurrencyId);

                colIndex++;
                FillAllowanceData(worksheet, row, colIndex, monthlyBasicAllowance, monthlyAllowance);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
                serialNo++;
            }

            const int addTwoRow = 2;
            WriteCurrencyExchangeTable(row + 1, template, worksheet, report.SelectedCurrency, report);
            WriteTotalByCurrencyTable(row + addTwoRow, template, worksheet, report.SelectedCurrency, report);

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(BodyRowIndex, 1);
        }

        private static void WriteDataSheetByOriginalCurrency(ExcelWorksheet template, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            WriteHeader(template, worksheet, null, report);
            const short BodyRowIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            int row = BodyRowIndex;
            var serialNo = 1;
            foreach (EmployeeBasicInfo employee in report.Employees)
            {
                var colIndex = 1;
                MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCols].Copy(worksheet.Cells[row, 1, row, TotalCols]);
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
                worksheet.Cells[row, colIndex++].Value = monthlySalary.DirectBanking1AccountName;
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.EbankingAmount, monthlySalary.EbankingCurrencyId, monthlyBasicSalary.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.DirectBanking1Amount, monthlySalary.DirectBanking1CurrencyId, monthlyBasicSalary.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.DirectBanking2Amount, monthlySalary.DirectBanking2CurrencyId, monthlyBasicSalary.CurrencyId);
                worksheet.Cells[row, colIndex++].Value
                    = CalculationService.ConvertAmount(report.CurrencyExchangeRate, monthlySalary.CashAmount, monthlySalary.CashCurrencyId, monthlyBasicSalary.CurrencyId);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicSalary.Salary);

                colIndex++;
                FillAllowanceData(worksheet, row, colIndex, monthlyBasicAllowance, monthlyAllowance);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
                serialNo++;
            }

            WriteTotalByOriginalCurrency(row + 1, template, worksheet, report);

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(BodyRowIndex, 1);
        }

        private static void WriteDataSheetByOriginalInputCurrency(ExcelWorksheet template, ExcelWorksheet worksheet, MonthlyPayrollReport report)
        {
            WriteHeader(template, worksheet, null, report);
            const short BodyRowIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            int row = BodyRowIndex;
            var serialNo = 1;
            foreach (EmployeeBasicInfo employee in report.Employees)
            {
                var colIndex = 1;
                MonthlyBasicSalaryModel monthlyBasicSalary = report.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlySalaryModel monthlySalary = report.MonthlySalary.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance = report.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);
                MonthlyAllowanceModel monthlyAllowance = report.MonthlyAllowance.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCols].Copy(worksheet.Cells[row, 1, row, TotalCols]);
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
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.DirectBanking1AccountName);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.EbankingAmount);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.DirectBanking1Amount);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.DirectBanking2Amount);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlySalary.CashAmount);
                worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicSalary.Salary);

                colIndex++;
                FillAllowanceData(worksheet, row, colIndex, monthlyBasicAllowance, monthlyAllowance);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
                serialNo++;
            }

            WriteTotalByOriginalInputCurrency(row + 1, template, worksheet, report);

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.View.FreezePanes(BodyRowIndex, 1);
        }

        private static void WriteHeader(ExcelWorksheet template, ExcelWorksheet worksheet, Currency selectedCurrency, MonthlyPayrollReport report)
        {
            const short CompanyRowIndex = 3;
            const short MonthRowIndex = 4;
            const short EndHeaderRowIndex = 8;
            const short CurrencyRowIndex = 7;
            const short CurrencyColIndex = 17;

            template.Cells[1, 1, EndHeaderRowIndex, TotalCols].Copy(worksheet.Cells[1, 1, EndHeaderRowIndex, TotalCols]);
            worksheet.Cells[CompanyRowIndex, 1].Value = string.Concat(ViewLang.Company, ": ", string.IsNullOrWhiteSpace(report.Company.Name) ? report.Company.Code : report.Company.Name);
            worksheet.Cells[MonthRowIndex, 1].Value = string.Concat(ViewLang.PayrollMonth, ": ", report.Month.ToString("MM-yyyy"));
            worksheet.Cells[CurrencyRowIndex, CurrencyColIndex].Value = selectedCurrency != null ? selectedCurrency.CurrencyCode : string.Empty;
        }

        private static void FillAllowanceData(
            ExcelWorksheet worksheet,
            int row, int colIndex,
            MonthlyPayrollBasicAllowanceModel monthlyBasicAllowance,
            MonthlyAllowanceModel monthlyAllowance)
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

            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.AdditionAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DeductionAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.RoomingChargesAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyBasicAllowance.FitnessFirstAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.TotalAmount);
            worksheet.Cells[row, colIndex++].Value = ParseToDecimal(monthlyAllowance.DSTNetSalaryAmount);
            worksheet.Cells[row, colIndex].Value = ParseToDecimal(monthlyAllowance.TotalNetAmount);
        }

        private static void WriteCurrencyExchangeTable(
            int startRow,
            ExcelWorksheet template,
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
            template.Cells[HeaderRowIndex, 1, HeaderRowIndex + 1, TotalCol].Copy(worksheet.Cells[startRow, 1, startRow + 1, TotalCol]);
            if (USDCurrencyId != selectedCurrencyId)
            {
                worksheet.Cells[startRow + 1, TotalCol].Value = selectedCurrency?.CurrencyCode;
            }

            // Write data
            var rowWithStyle = template.Cells[BodyRowIndex, 1, BodyRowIndex, TotalCol];
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
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            Currency selectedCurrency,
            MonthlyPayrollReport report)
        {
            const short SubsistenceColIndex = 24;
            const short HeaderRowIndex = 18;
            const short BodyRowIndex = 19;
            const short StartColIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            // Write header
            template.Cells[HeaderRowIndex, StartColIndex, HeaderRowIndex, TotalCols].Copy(worksheet.Cells[startRow, StartColIndex, startRow, TotalCols]);

            // Write data
            var rowWithStyle = template.Cells[BodyRowIndex, StartColIndex, BodyRowIndex, TotalCols];
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
                colIndex++;

                if (currency.CurrencyId == selectedCurrency?.CurrencyId)
                {
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x =>
                    CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.EbankingAmount, x.EbankingCurrencyId, selectedCurrency?.CurrencyId));
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x =>
                        CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.DirectBanking1Amount, x.DirectBanking1CurrencyId, selectedCurrency?.CurrencyId));
                    worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Sum(x =>
                        CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.DirectBanking2Amount, x.DirectBanking2CurrencyId, selectedCurrency?.CurrencyId));
                    worksheet.Cells[row, colIndex].Value = report.MonthlySalary.Sum(x =>
                        CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.CashAmount, x.CashCurrencyId, selectedCurrency?.CurrencyId));
                }

                colIndex = SubsistenceColIndex;
                IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowances = report.MonthlyBasicAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlyAllowanceModel> allowances = report.MonthlyAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                FillTotalAllowance(worksheet, basicAllowances, allowances, row, colIndex);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
            }
        }

        private static void WriteTotalByOriginalCurrency(
            int startRow,
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            MonthlyPayrollReport report)
        {
            const short SubsistenceColIndex = 24;
            const short HeaderRowIndex = 18;
            const short BodyRowIndex = 19;
            const short StartColIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            // Write header
            template.Cells[HeaderRowIndex, StartColIndex, HeaderRowIndex, TotalCols].Copy(worksheet.Cells[startRow, StartColIndex, startRow, TotalCols]);

            // Write data
            var rowWithStyle = template.Cells[BodyRowIndex, StartColIndex, BodyRowIndex, TotalCols];
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
                colIndex++;

                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x =>
                    CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.EbankingAmount, x.EbankingCurrencyId, currency.CurrencyId));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x =>
                    CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.DirectBanking1Amount, x.DirectBanking1CurrencyId, currency.CurrencyId));
                worksheet.Cells[row, colIndex++].Value = salaryList.Sum(x =>
                    CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.DirectBanking2Amount, x.DirectBanking2CurrencyId, currency.CurrencyId));
                worksheet.Cells[row, colIndex].Value = salaryList.Sum(x =>
                    CalculationService.ConvertAmount(report.CurrencyExchangeRate, x.CashAmount, x.CashCurrencyId, currency.CurrencyId));

                colIndex = SubsistenceColIndex;
                IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowances = report.MonthlyBasicAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlyAllowanceModel> allowances = report.MonthlyAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                FillTotalAllowance(worksheet, basicAllowances, allowances, row, colIndex);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
            }
        }

        private static void WriteTotalByOriginalInputCurrency(
            int startRow,
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            MonthlyPayrollReport report)
        {
            const short SubsistenceColIndex = 24;
            const short HeaderRowIndex = 18;
            const short BodyRowIndex = 19;
            const short StartColIndex = 9;
            const short ActualBasicSalaryAmountColIndex = 10;

            // Write header
            template.Cells[HeaderRowIndex, StartColIndex, HeaderRowIndex, TotalCols].Copy(worksheet.Cells[startRow, StartColIndex, startRow, TotalCols]);

            // Write data
            var rowWithStyle = template.Cells[BodyRowIndex, StartColIndex, BodyRowIndex, TotalCols];
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
                colIndex++;

                worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Where(x => x.EbankingCurrencyId == currency.CurrencyId).Sum(x => ParseToDecimal(x.EbankingAmount));
                worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Where(x => x.DirectBanking1CurrencyId == currency.CurrencyId).Sum(x => ParseToDecimal(x.DirectBanking1Amount));
                worksheet.Cells[row, colIndex++].Value = report.MonthlySalary.Where(x => x.DirectBanking2CurrencyId == currency.CurrencyId).Sum(x => ParseToDecimal(x.DirectBanking2Amount));
                worksheet.Cells[row, colIndex].Value = report.MonthlySalary.Where(x => x.CashCurrencyId == currency.CurrencyId).Sum(x => ParseToDecimal(x.CashAmount));

                colIndex = SubsistenceColIndex;
                IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowances = report.MonthlyBasicAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                IEnumerable<MonthlyAllowanceModel> allowances = report.MonthlyAllowance.Where(x => x.CurrencyId == currency.CurrencyId);
                FillTotalAllowance(worksheet, basicAllowances, allowances, row, colIndex);

                CreateConditionalFormating(worksheet, row, ActualBasicSalaryAmountColIndex, MonthlyPayrollAdditionColIndex - 1, Constants.NumberFormatTwoDecimals);

                row++;
            }
        }

        private static void FillTotalAllowance(
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowances,
            IEnumerable<MonthlyAllowanceModel> allowances,
            int rowIndex, int colIndex)
        {
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.SubsistenceAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HomePassageAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HousingAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.MobilePhoneAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HPIAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.DutyMealAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.SecondmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.OthersAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.TotalAmount));

            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.SubsistenceAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HomePassageAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HousingAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.MobilePhoneAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.HPIAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.DutyMealAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.SecondmentAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.OthersAfterAdjustmentAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.AfterAdjustmentAmount));

            worksheet.Cells[rowIndex, colIndex++].Value = allowances.Sum(x => ParseToDecimal(x.AdditionAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = allowances.Sum(x => ParseToDecimal(x.DeductionAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.RoomingChargesAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = basicAllowances.Sum(x => ParseToDecimal(x.FitnessFirstAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = allowances.Sum(x => ParseToDecimal(x.TotalAmount));
            worksheet.Cells[rowIndex, colIndex++].Value = allowances.Sum(x => ParseToDecimal(x.DSTNetSalaryAmount));
            worksheet.Cells[rowIndex, colIndex].Value = allowances.Sum(x => ParseToDecimal(x.TotalNetAmount));
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