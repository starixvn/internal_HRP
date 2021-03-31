namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Criteria.Report;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Criteria.User;
    using HRP.Models.ServiceModels;
    using Models.Currency;
    using Models.MonthlyPayroll;
    using Models.Report;
    using NSubstitute;
    using Repositories;
    using Xunit;

    public class ReportServiceTests
    {
        private ICryptoService cryptoService;
        private IReportRepository reportRepository;
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task GetDirectBankingBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();
            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(new DirectBankingReport());

            // Act
            var actual = await reportService.GetDirectBankingBankInAsync(0, DateTime.Now, PaymentType.Allowances, "");

            Assert.Null(actual.DirectBankingBankIns);
            Assert.Null(actual.CashBankIns);
            Assert.Null(actual.PaymentTransactions);
        }

        [Fact]
        public async Task GetEbankingBankInAsync_InValidParams_ReturnEmpty()
        {
            // Arrange
            var reportService = GetReportService();
            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(new DirectBankingReport());

            // Act
            var actual = await reportService.GetEbankingBankInAsync(0, DateTime.Now, "");

            Assert.Null(actual.DirectBankingBankIns);
            Assert.Null(actual.CashBankIns);
            Assert.Null(actual.PaymentTransactions);
        }

        [Fact]
        public async Task GetDirectBankingBankInAsync_ValidParams_ReturnListBankIns()
        {
            // Arrange
            var reportService = GetReportService();

            float salaryDescrypted = 1;
            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                FakeBankInObject(),
                FakeBankInObject()
            };

            var bankInReport = new DirectBankingReport
            {
                DirectBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                Month = reportedMonth,
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                }
            };

            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(bankInReport);

            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetDirectBankingBankInAsync(1, DateTime.Now, PaymentType.BasicSalary, "1-1");

            Assert.All(actual.DirectBankingBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
                Assert.NotNull(x.IDNo);
                Assert.NotNull(x.BeneficiaryName);
                Assert.NotNull(x.Nickname);
                Assert.NotNull(x.PaymentTransaction);
                Assert.NotNull(x.BankName);
                Assert.NotNull(x.BankBranch);
                Assert.NotNull(x.BankAddress);
                Assert.NotNull(x.BankSwiftCode);
                Assert.NotNull(x.AccountNumber);
                Assert.NotNull(x.CurrencyCode);
            });

            Assert.All(actual.CashBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
                Assert.NotNull(x.IDNo);
                Assert.NotNull(x.BeneficiaryName);
                Assert.NotNull(x.Nickname);
                Assert.NotNull(x.PaymentTransaction);
                Assert.NotNull(x.BankName);
                Assert.NotNull(x.BankBranch);
                Assert.NotNull(x.BankAddress);
                Assert.NotNull(x.BankSwiftCode);
                Assert.NotNull(x.AccountNumber);
                Assert.NotNull(x.CurrencyCode);
            });

            Assert.Equal(companyName, actual.CompanyName);
            Assert.Equal(reportedMonth, actual.Month);
            Assert.Equal(bankInReport.PaymentTransactions, actual.PaymentTransactions);
        }

        [Fact]
        public async Task GetEbankingBankInAsync_ValidParams_ReturnListBankIns()
        {
            // Arrange
            var reportService = GetReportService();

            float salaryDescrypted = 1;
            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                FakeBankInObject(),
                FakeBankInObject()
            };

            var bankInReport = new DirectBankingReport
            {
                DirectBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                Month = reportedMonth,
                PaymentTransactions = new List<ReferenceResourceModel>
                {
                    new ReferenceResourceModel { ResourceId = 1, Value = "e-banking" },
                    new ReferenceResourceModel { ResourceId = 2, Value = "cash" }
                }
            };

            reportRepository.GetDataForBankInReportAsync(Arg.Any<ICriteria>()).Returns(bankInReport);

            cryptoService.Decrypt(Arg.Any<string>()).Returns("1");

            // Act
            var actual = await reportService.GetEbankingBankInAsync(1, DateTime.Now, "1-1");

            Assert.All(actual.DirectBankingBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
                Assert.NotNull(x.IDNo);
                Assert.NotNull(x.BeneficiaryName);
                Assert.NotNull(x.Nickname);
                Assert.NotNull(x.PaymentTransaction);
                Assert.NotNull(x.BankName);
                Assert.NotNull(x.BankBranch);
                Assert.NotNull(x.BankAddress);
                Assert.NotNull(x.BankSwiftCode);
                Assert.NotNull(x.AccountNumber);
                Assert.NotNull(x.CurrencyCode);
            });

            Assert.All(actual.CashBankIns, x =>
            {
                Assert.Equal(salaryDescrypted, x.AmountDecrypted);
                Assert.NotNull(x.IDNo);
                Assert.NotNull(x.BeneficiaryName);
                Assert.NotNull(x.Nickname);
                Assert.NotNull(x.PaymentTransaction);
                Assert.NotNull(x.BankName);
                Assert.NotNull(x.BankBranch);
                Assert.NotNull(x.BankAddress);
                Assert.NotNull(x.BankSwiftCode);
                Assert.NotNull(x.AccountNumber);
                Assert.NotNull(x.CurrencyCode);
            });

            Assert.Equal(companyName, actual.CompanyName);
            Assert.Equal(reportedMonth, actual.Month);
            Assert.Equal(bankInReport.PaymentTransactions, actual.PaymentTransactions);
        }

        [Fact]
        public async Task GetDataForMonthlyPayrollReportAsync_WhenCalled_ReturnMonthlyPayrollReport()
        {
            // Arrange
            var report = new MonthlyPayrollReport
            {
                MonthlyBasicSalary = new List<MonthlyBasicSalaryModel> { new MonthlyBasicSalaryModel { EmployeeId = 3 } },
                MonthlySalary = new List<MonthlySalaryModel> { new MonthlySalaryModel { EmployeeId = 3 } },
                MonthlyBasicAllowance = new List<MonthlyPayrollBasicAllowanceModel> { new MonthlyPayrollBasicAllowanceModel { EmployeeId = 3, CurrencyCode = Constants.VNDCurrencyCode } },
                MonthlyAllowance = new List<MonthlyAllowanceModel> { new MonthlyAllowanceModel { EmployeeId = 3, DSTNetSalaryCurrencyCode = "PHP" } },
                CurrencyExchangeRate = new List<CurrencyExchangeRateModel>()
            };
            var month = DateTime.Today;
            var companyId = 21;
            var reportService = GetReportService();
            reportRepository.GetDataForMonthlyPayrollReportAsync(Arg.Is<GetDataForMonthlyPayrollReportCriteria>(x => x.CompanyId == companyId && x.Month == month))
                            .Returns(report);

            // Act
            var actual = await reportService.GetDataForMonthlyPayrollReportAsync(month, companyId, 1);

            // Assert
            Assert.IsType<MonthlyPayrollReport>(actual);
            Assert.Equal(report, actual);
        }

        public static IEnumerable<object[]> GetEmployeeDatas()
        {
            yield return new object[]
            {
                new DynamicReportModel
                {
                    Salary = "1000",
                    CurrencyId = 1,
                    AutoCalculateOn = PayrollAutoCalculateOn.Ebanking,
                    EbankingCurrencyId = 1,
                    AllowanceCurrencyId = 1,
                    DutyMealCurrencyId = 1,
                    DutyMealAmount = "1000",
                    AllowanceDirectBanking1CurrencyId = 1,
                    AllowanceAutoCalculateOn = PayrollAutoCalculateOn.DirectBanking1
                }
            };

            yield return new object[]
            {
                new DynamicReportModel
                {
                    Salary = "1000",
                    CurrencyId = 1,
                    AutoCalculateOn = PayrollAutoCalculateOn.DirectBanking1,
                    DirectBanking1CurrencyId = 1,
                    AllowanceCurrencyId = 1,
                    DutyMealCurrencyId = 1,
                    DutyMealAmount = "1000",
                    AllowanceDirectBanking1CurrencyId = 1,
                    AllowanceAutoCalculateOn = PayrollAutoCalculateOn.DirectBanking1
                }
            };

            yield return new object[]
            {
                 new DynamicReportModel
                {
                    Salary = "1000",
                    CurrencyId = 1,
                    AutoCalculateOn = PayrollAutoCalculateOn.DirectBanking2,
                    DirectBanking2CurrencyId = 1,
                    AllowanceCurrencyId = 1,
                    DutyMealCurrencyId = 1,
                    DutyMealAmount = "1000",
                    AllowanceDirectBanking2CurrencyId = 1,
                    AllowanceAutoCalculateOn = PayrollAutoCalculateOn.DirectBanking2
                }
            };

            yield return new object[]
            {
                new DynamicReportModel
                {
                    Salary = "1000",
                    CurrencyId = 1,
                    AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                    CashCurrencyId = 1,
                    AllowanceCurrencyId = 1,
                    DutyMealCurrencyId = 1,
                    DutyMealAmount = "1000",
                    AllowanceCashCurrencyId = 1,
                    AllowanceAutoCalculateOn = PayrollAutoCalculateOn.Cash
                }
            };

            yield return new object[]
            {
                new DynamicReportModel
                {
                    Salary = "1000",
                    CurrencyId = 1,
                    AutoCalculateOn = PayrollAutoCalculateOn.Default,
                    CashCurrencyId = 1,
                    AllowanceCurrencyId = 1,
                    DutyMealCurrencyId = 1,
                    DutyMealAmount = "1000",
                    AllowanceCashCurrencyId = 1,
                    AllowanceAutoCalculateOn = PayrollAutoCalculateOn.Default
                }
            };
        }

        [Theory, MemberData(nameof(GetEmployeeDatas))]
        public async Task GetEmployeeDataForDynamicReportAsync_WhenCalled_ReturnEmployeeDatas(DynamicReportModel model)
        {
            var reportService = GetReportService();
            var datas = new List<DynamicReportModel>
            {
                model
            };
            baseService.GetDataAsync<DynamicReportModel>(Arg.Any<GetEmployeeDataForDynamicReportCriteria>()).Returns(datas);
            baseService.GetDataAsync<CurrencyExchangeRateModel>(Arg.Any<GetLatestExchangeRatesCriteria>())
                .Returns(new List<CurrencyExchangeRateModel> { new CurrencyExchangeRateModel { FromCurrencyId = 1, ToCurrencyId = 1, ExchangeRate = 1 } });
            cryptoService.Decrypt("1000").Returns("1000");
            cryptoService.Decrypt(string.Empty).Returns(string.Empty);
            cryptoService.Decrypt(null).Returns(string.Empty);

            var actual = await reportService.GetEmployeeDataForDynamicReport(1, "1,2");

            Assert.Equal(datas.Count, actual.Count());
        }

        [Fact]
        public async Task GetUserFilePasswordAsync_WhenCalled_ReturnFilePassword()
        {
            var userId = 1;
            var expected = "456";
            var reportService = GetReportService();
            baseService.GetDataEntityAsync<string>(Arg.Any<GetUserFilePasswordCriteria>()).Returns("123");
            cryptoService.Decrypt("123").Returns(expected);
            // Act
            var actual = await reportService.GetUserFilePasswordAsync(userId);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task EditUserFilePasswordAsync_WhenCalled_ExpectMethodsCalled()
        {
            var userId = 1;
            var password = "123";
            var reportService = GetReportService();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditUserFilePasswordCriteria>()).Returns(new CommandResult());
            cryptoService.Encrypt(password).Returns("456");
            // Act
            await reportService.EditUserFilePasswordAsync(userId, password);

            // Assert
            await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<EditUserFilePasswordCriteria>());
            cryptoService.Received(1).Encrypt(password);
        }

        #region Helpers

        private static BankIn FakeBankInObject()
        {
            return new BankIn
            {
                IDNo = "a",
                BeneficiaryName = "hunter",
                Nickname = "hunter.tran",
                PaymentTransaction = "1",
                BankName = "Techcombank",
                BankBranch = "HCM",
                BankAddress = "1",
                BankSwiftCode = "QWERTY",
                AccountNumber = "1",
                Amount = "1",
                CurrencyCode = "1"
            };
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private static ISessionService FakeSessionService()
        {
            return Substitute.For<ISessionService>();
        }

        private static IReportRepository FakeReportRepository()
        {
            return Substitute.For<IReportRepository>();
        }

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private ReportService GetReportService()
        {
            cryptoService = FakeCryptoService();
            reportRepository = FakeReportRepository();
            sessionService = FakeSessionService();
            baseService = FakeBaseService();

            return new ReportService(cryptoService, sessionService, baseService, reportRepository);
        }

        #endregion Helpers
    }
}