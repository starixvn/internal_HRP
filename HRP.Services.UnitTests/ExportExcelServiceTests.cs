namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Services;
    using HRP.Models.Report;
    using HRP.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class ExportExcelServiceTests
    {
        private ICryptoService cryptoService;

        [Fact]
        public void ExportDynamicReport_ValidInput_ReturnByteData()
        {
            var service = GetExportExcelService();
            var selectedIds = new List<int> { 1, 2, 3, 4 };
            var employeeDatas = new List<DynamicReportModel>
            {
                new DynamicReportModel
                {
                    IDNo = "DAR001",
                    Nickname = "Del",
                    FullName = "Kai",
                    DateOfBirth = DateTime.Today,
                    Gender = "Male"
                }
            };

            var actual = service.ExportDynamicReport(employeeDatas, selectedIds);

            Assert.True(actual.Length > 0);
        }

        private static ICryptoService FakeCryptoService()
        {
            return Substitute.For<ICryptoService>();
        }

        private IExportExcelService GetExportExcelService()
        {
            cryptoService = FakeCryptoService();
            var service = new ExportExcelService(cryptoService);

            return service;
        }
    }
}