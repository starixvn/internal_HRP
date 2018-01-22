namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using NSubstitute;
    using Xunit;

    public class EmployeePayrollServiceTest
    {
        private IBaseService baseService;
        private IDynamicRepository repository;
        private ICryptoService cryptoService;

        //[Fact]
        public void GetEmployeePayroll_ReturnModel()
        {
            var employeeId = 1;
            var userId = 1;

            InitializeServices();
            repository.FetchMultiple(Arg.Any<GetEmployeePayrollCriteria>(), Arg.Any<Type[]>())
                .Returns(new List<object>[] {});
            baseService.GetData<CurrencyExchangeRateModel>(Arg.Any<GetLatestExchangeRatesCriteria>()).Returns(new List<CurrencyExchangeRateModel>());
            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>()).Returns(new List<CurrencyCodeModel>());
            baseService.GetData<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(new List<ReferenceResourceModel>());
            cryptoService.Decrypt(Arg.Any<string>()).Returns(string.Empty);
        }

        private void InitializeServices()
        {
            baseService = Substitute.For<IBaseService>();
            repository = Substitute.For<IDynamicRepository>();
            cryptoService = Substitute.For<ICryptoService>();
        }
    }
}