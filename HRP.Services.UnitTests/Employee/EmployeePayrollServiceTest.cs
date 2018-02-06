namespace HRP.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using Models.EmployeeManagement.Payroll;
    using NSubstitute;
    using Xunit;

    public class EmployeePayrollServiceTest
    {
        private IBaseService baseService;
        private IDynamicRepository repository;
        private ICryptoService cryptoService;

        [Fact]
        public void GetEmployeePayroll_ValidData_ReturnModel()
        {
            var employeeId = 1;
            var userId = 1;

            InitializeServices();
            repository.FetchMultiple(Arg.Any<GetEmployeePayrollCriteria>(), Arg.Any<Type[]>())
                .Returns(new[] 
                {
                    new List<object>
                    {
                        new EmployeePayrollViewModel
                        {
                            EmployeeId = employeeId,
                            FullName = "Full Name",
                            Nickname = "Nick Name",
                            Code = "DAR",
                            CompanyId = 1
                        }
                    },
                    new List<object>(),
                    new List<object>(),
                    new List<object>()
                });
            baseService.GetData<CurrencyExchangeRateModel>(Arg.Any<GetLatestExchangeRatesCriteria>()).Returns(new List<CurrencyExchangeRateModel>());
            baseService.GetData<CurrencyCodeModel>(Arg.Any<GetActiveCurrencyCodesCriteria>()).Returns(new List<CurrencyCodeModel>());
            baseService.GetData<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(new List<ReferenceResourceModel>());
            cryptoService.Decrypt(Arg.Any<string>()).Returns(string.Empty);

            var service = new EmployeePayrollService(baseService, repository, cryptoService);
            var actual = service.GetEmployeePayroll(employeeId, userId);

            Assert.NotNull(actual);
            Assert.NotNull(actual.BasicSalary);
            Assert.NotNull(actual.Allowance);
            Assert.NotNull(actual.Deduction);
        }

        [Fact]
        public void UpdatePayroll_ValidData_UpdateSuccessfully()
        {
            var employeeId = 1;
            var userId = 1;

            InitializeServices();
            baseService.ExecuteNonQuery(Arg.Any<UpdateEmployeePayrollCriteria>()).Returns(new CommandResult { Code = 0 });
            cryptoService.Encrypt(Arg.Any<string>()).Returns(string.Empty);

            var service = new EmployeePayrollService(baseService, repository, cryptoService);
            var model = new EmployeePayrollViewModel {
                EmployeeId = employeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId },
                Allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId },
                Deduction = new EmployeePayrollDeduction { EmployeeId =  employeeId }
            };

            var actual = service.UpdatePayroll(model, userId);

            baseService.Received().ExecuteNonQuery(Arg.Any<UpdateEmployeePayrollCriteria>());
            cryptoService.Received().Encrypt(Arg.Any<string>());
            Assert.True(actual.IsSuccessful);
        }

        private void InitializeServices()
        {
            baseService = Substitute.For<IBaseService>();
            repository = Substitute.For<IDynamicRepository>();
            cryptoService = Substitute.For<ICryptoService>();
        }
    }
}