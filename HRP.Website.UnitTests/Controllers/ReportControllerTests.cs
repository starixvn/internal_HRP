namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Model;
    using Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Models;
    using Newtonsoft.Json;
    using NSubstitute;
    using Services.Interfaces;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class ReportControllerTests
    {
        private ISessionService sessionService;
        private IBaseService baseService;
        private IReportService reportService;
        private IHostingEnvironment hostingEnvironment;
        private IExportExcelService exportExcelService;
        private ICommonService commonService;

        [Fact]
        public async Task GetPaymentTransactions_EBanking_ReturnEbanking()
        {
            //Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionEbanking,
                    ResourceId = 1,
                    Value = "1"
                }
            };

            var expected = new List<MultiSelectViewModel>();
            expected.AddRange(refResources.Select(x => new MultiSelectViewModel
            {
                Value = $"{(int)BankTypes.Ebanking}-{x.ResourceId}",
                Text = x.Value
            }));

            commonService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(refResources);

            //Act
            var actual = await reportController.GetPaymentTransactions(BankTypes.Ebanking);

            //Assert
            var json = Assert.IsType<JsonResult>(actual);
            Assert.Equal(
                JsonConvert.SerializeObject(json.Value),
                JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task GetPaymentTransactions_DirectBanking_ReturnDirectBanking()
        {
            //Arrange
            var reportController = GetReportController();

            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    IsActive = true,
                    RefName = ReferenceResource.PaymentTransactionEbanking,
                    ResourceId = 1,
                    Value = "1"
                }
            };

            var expected = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel
                {
                    Value = ((int)BankTypes.DirectBanking).ToString(),
                    Text = BankTypes.DirectBanking.GetDisplayName(),
                    ParentId = "0",
                    Items = refResources.Select(x => new MultiSelectViewModel
                    {
                        Value = $"{(int)BankTypes.DirectBanking}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = ((int)BankTypes.DirectBanking).ToString()
                    }).ToList()
                },
                new MultiSelectViewModel
                {
                    Value = "3",
                    Text = "Cash",
                    ParentId = "0",
                    Items = refResources.Select(x => new MultiSelectViewModel
                    {
                        Value = $"{3}-{x.ResourceId}",
                        Text = x.Value,
                        ParentId = "3"
                    }).ToList()
                }
            };

            commonService
                .FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>())
                .Returns(refResources);

            //Act
            var actual = await reportController.GetPaymentTransactions(BankTypes.DirectBanking);

            //Assert
            var json = Assert.IsType<JsonResult>(actual);
            Assert.Equal(
                JsonConvert.SerializeObject(json.Value),
                JsonConvert.SerializeObject(expected));
        }

        [Fact]
        public async Task DirectBanking_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.DirectBanking();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Ebanking_WhenCalled_ReturnExpectedView()
        {
            //Arrange
            var reportController = GetReportController();

            //Act
            var actual = await reportController.Ebanking();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<BankInReportViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task GetDirectBankingReport_InValidParams_ReturnEmpty()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now
            };

            var reportController = GetReportController();

            //Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetDirectBankingReport_ValidParams_ReturnNull()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 1,
                SelectedTime = DateTime.Now
            };

            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                new BankIn { Amount = "1" },
                new BankIn { Amount = "1" },
            };

            var bankInReport = new BankInReport
            {
                EBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth
            };

            var reportController = GetReportController();
            reportService.GetBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<PaymentType>(), Arg.Any<string>()).Returns(bankInReport);

            //Act
            var actual = await reportController.GetDirectBankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_InValidParams_ReturnEmpty()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 0,
                SelectedTime = DateTime.Now
            };

            var reportController = GetReportController();

            //Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public async Task GetEbankingReport_ValidParams_ReturnNull()
        {
            //Arrage
            var bankInReportViewModel = new BankInReportViewModel
            {
                CompanyId = 1,
                SelectedTime = DateTime.Now
            };

            var companyName = "Dashword";
            var reportedMonth = DateTime.Today;

            var bankInData = new List<BankIn>
            {
                new BankIn { Amount = "1" },
                new BankIn { Amount = "1" },
            };

            var bankInReport = new BankInReport
            {
                EBankingBankIns = bankInData,
                CashBankIns = bankInData,
                CompanyName = companyName,
                ReportedMonth = reportedMonth
            };

            var reportController = GetReportController();
            reportService.GetBankInAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<PaymentType>(), Arg.Any<string>()).Returns(bankInReport);

            //Act
            var actual = await reportController.GetEbankingReport(bankInReportViewModel);

            //Assert
            Assert.IsType<FileContentResult>(actual);
        }

        [Fact]
        public async Task GetMonthlyPayrollReport_WhenCalled_ReturnFileContentResult()
        {
            // Arrange
            byte[] bytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==");
            var report = new MonthlyPayrollReport();
            var month = DateTime.Today;
            var companyId = 22;
            var webRootPath = "abc";
            var fileName = @"ExcelTemplates\MonthlyPayrollReport.xlsx";
            var templatePath = Path.Combine(webRootPath, fileName);
            var reportController = GetReportController();
            hostingEnvironment.WebRootPath.Returns(webRootPath);
            reportService.GetDataForMonthlyPayrollReportAsync(Arg.Is<DateTime>(x => x == month), Arg.Is<int>(x => x == companyId)).Returns(report);
            exportExcelService.ExportMonthlyPayrollReport(Arg.Is<string>(x => x == templatePath), Arg.Is<MonthlyPayrollReport>(x => x == report)).Returns(bytes);

            // Act
            var actual = await reportController.GetMonthlyPayrollReport(month, companyId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(actual);
            Assert.Equal(bytes, fileResult.FileContents);
            Assert.Equal(MimeType.XLSX, fileResult.ContentType);
            Assert.Equal($"MonthlyPayrollReport_{month.ToString("MM-yyyy")}_{companyId}.xlsx", fileResult.FileDownloadName);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private IReportService FakeReportService()
        {
            return Substitute.For<IReportService>();
        }

        private IHostingEnvironment FakeHostingEnviroment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private IExportExcelService FakeExportExcelService()
        {
            return Substitute.For<IExportExcelService>();
        }

        private ICommonService FakeCommonService()
        {
            return Substitute.For<ICommonService>();
        }

        private ReportController GetReportController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            reportService = FakeReportService();
            exportExcelService = FakeExportExcelService();
            hostingEnvironment = FakeHostingEnviroment();
            commonService = FakeCommonService();

            return new ReportController(
                baseService,
                sessionService,
                hostingEnvironment,
                exportExcelService,
                reportService,
                commonService);
        }

        #endregion Helpers
    }
}