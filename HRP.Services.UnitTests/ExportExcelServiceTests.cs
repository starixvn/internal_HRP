namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models.Report;
    using NSubstitute;
    using Xunit;

    public class ExportExcelServiceTests
    {
        [Fact]
        public void ExportDirectBankingReports_HasNotData_ReturnAsExpected()
        {
            // Arrange
            var data = new DirectBankingReport
            {
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                }
            };
            var fileInfo = new FileInfo(@"FileTest\\TemplateTest.xlsx");
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportDirectBankingReports(DateTime.Now, fileInfo, PaymentType.BasicSalary, data);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ExportDirectBankingReports_HasData_ReturnAsExpected()
        {
            // Arrange
            var data = new DirectBankingReport
            {
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                },
                DirectBankingBankIns = new List<BankIn> { new BankIn { IDNo = "DAR1234", Nickname = "tim.trinh", BankName = "e-banking" } },
                CashBankIns = new List<BankIn> { new BankIn { IDNo = "DAR1234", Nickname = "tim.trinh", BankName = "e-banking" } }
            };
            var fileInfo = new FileInfo(@"FileTest\\TemplateTest.xlsx");
            var password = "123456";
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportDirectBankingReports(DateTime.Now, fileInfo, PaymentType.BasicSalary, data, password);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ExportDirectBankingReports_EmptyCashBankIns_ReturnAsExpected()
        {
            // Arrange
            var data = new DirectBankingReport
            {
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                },
                DirectBankingBankIns = new List<BankIn> { new BankIn { IDNo = "DAR1234", Nickname = "tim.trinh", BankName = "e-banking" } },
                CashBankIns = new List<BankIn>()
            };

            var fileInfo = new FileInfo(@"FileTest\\TemplateTest.xlsx");
            var password = "123456";
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportDirectBankingReports(DateTime.Now, fileInfo, PaymentType.BasicSalary, data, password);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ExportEbankingReports_HasNotData_ReturnAsExpected()
        {
            // Arrange
            var data = new DirectBankingReport
            {
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                }
            };
            var fileInfo = new FileInfo(@"FileTest\\TemplateTest.xlsx");
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportEbankingReports(DateTime.Now, fileInfo, PaymentType.BasicSalary, data);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ExportEbankingReports_HasData_ReturnAsExpected()
        {
            // Arrange
            var data = new DirectBankingReport
            {
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                },
                DirectBankingBankIns = new List<BankIn> { new BankIn { IDNo = "DAR1234", Nickname = "tim.trinh", BankName = "e-banking" } },
                CashBankIns = new List<BankIn> { new BankIn { IDNo = "DAR1234", Nickname = "tim.trinh", BankName = "e-banking" } }
            };
            var fileInfo = new FileInfo(@"FileTest\\TemplateTest.xlsx");
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportEbankingReports(DateTime.Now, fileInfo, PaymentType.BasicSalary, data);

            // Assert
            Assert.NotEmpty(actual);
        }

        ////[Fact]
        ////public void ExportMonthlyPayrollReport_WhenCalled_ReturnAsExpected()
        ////{
        ////    // Arrange
        ////    var data = new MonthlyPayrollReport
        ////    {
        ////        Month = DateTime.Now,
        ////        Company = new Company { CompanyId = 1, Code = "DAR", Name = "DAR" },
        ////        Employees = new List<EmployeeBasicInfo> { new EmployeeBasicInfo { EmployeeId = 1, Nickname = "tim.trinh" } },
        ////        CurrencyList = new List<Currency>
        ////        {
        ////            new Currency { CurrencyId = 1, CurrencyCode = "USD" },
        ////            new Currency { CurrencyId = 2, CurrencyCode = "MYR" }
        ////        },
        ////        MonthlyBasicSalary = new List<MonthlyBasicSalaryModel> { new MonthlyBasicSalaryModel { EmployeeId = 1 } },
        ////        MonthlySalary = new List<MonthlySalaryModel> { new MonthlySalaryModel { EmployeeId = 1 } },
        ////        MonthlyAllowance = new List<MonthlyAllowanceModel> { new MonthlyAllowanceModel { EmployeeId = 1 } },
        ////        MonthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel> { new MonthlyPayrollBasicAllowanceModel { EmployeeId = 1 } },
        ////        CurrencyExchangeRate = new List<CurrencyExchangeRateModel>
        ////        {
        ////            new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 },
        ////            new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 2, ExchangeRate = 2 }
        ////        }
        ////    };
        ////    var service = GetExportExcelService();

        ////    // Act
        ////    var actual = service.ExportMonthlyPayrollReport(@"FileTest\\TemplateTest.xlsx", data);

        ////    // Assert
        ////    Assert.NotEmpty(actual);
        ////}

        [Fact]
        public void ExportDynamicReport_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var data = new List<DynamicReportModel> { new DynamicReportModel { IDNo = "DAR1234", CompanyStartDate = DateTime.Now } };
            var selectedIds = new List<int> { 1, 42 };
            var service = GetExportExcelService();

            // Act
            var actual = service.ExportDynamicReport(data, selectedIds);

            // Assert
            Assert.NotEmpty(actual);
        }

        #region Helpers

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static ExportExcelService GetExportExcelService()
        {
            var cryptoService = FakeCryptoService();
            var service = new ExportExcelService(cryptoService);

            return service;
        }

        #endregion Helpers
    }
}