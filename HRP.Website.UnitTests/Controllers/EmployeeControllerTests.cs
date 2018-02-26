namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Models.EmployeeManagement.Payroll;
    using NSubstitute;
    using Services.Interfaces;
    using Xunit;

    public class EmployeeControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IEmployeeService employeeService;
        private IEmployeePayrollService payrollService;

        [Fact]
        public async Task Index_WhenCalled_ReturnAsExpectedViewResult()
        {
            //Arrange
            var employees = new List<EmployeeBasicInfo>
            {
                new EmployeeBasicInfo
                {
                    EmployeeId = 1,
                    EmployeeCode = "111",
                    Nickname = "tim.trinh",
                    FullName = "trinh dang thang",
                    CompanyCode = "DAR",
                    DivisionCode = "SM",
                    DepartmentCode = "SM",
                    JobTitle = "Management",
                    EmployeeStatus = "Confirmed",
                    EditBankPolicy = Policy.Read,
                    EditPayRollPolicy = Policy.Read,
                    EditProfilePolicy = Policy.Read
                }
            };
            var viewModel = new EmployeeManagementViewModel
            {
                DepartmentIds = "1,2",
                EmploymentStatusIds = "1,2"
            };
            var employeeController = GetEmployeeController();
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            employeeService.GetDataFilterForIndexPage(Arg.Any<string>(), Arg.Any<string>()).Returns(viewModel);
            employeeService.GetEmployeesAsync(Arg.Any<EmployeeManagementViewModel>()).Returns(employees);

            //Act
            var actual = await employeeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsAssignableFrom<EmployeeManagementViewModel>(viewResult.ViewData.Model);
            Assert.True(model.IsHostCreateEmployee);
            Assert.Equal(viewModel.DepartmentIds, model.DepartmentIds);
            Assert.Equal(viewModel.EmploymentStatusIds, model.EmploymentStatusIds);
            Assert.Equal(viewModel.HierarchyCompanies, model.HierarchyCompanies);
            Assert.Equal(viewModel.EmploymentStatuses, model.EmploymentStatuses);
            Assert.Equal(viewModel.HierarchyCompanyJson, model.HierarchyCompanyJson);
            Assert.Equal(viewModel.EmploymentStatusJson, model.EmploymentStatusJson);
            Assert.Equal(employees, model.Employees);
        }

        #region CreateEmployee

        [Fact]
        public async Task CreateEmployee_CheckViewResult_ReturnAsExpected()
        {
            // Arrange
            var viewName = "CreateEmployee";
            var employeeController = GetEmployeeController();
            employeeService.GetDataToCreateEmployeeAsync().Returns(new CreateEmployeeViewModel());

            // Act
            var actual = await employeeController.CreateEmployee();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<CreateEmployeeViewModel>(viewResult.Model);
            Assert.Equal(viewName, viewResult.ViewName);
        }

        [Fact]
        public void CreatePersonalDetails_InvalidProfilePicture_ReturnExpectedErrors()
        {
            // Arrange
            var model = new CreateEmployeeViewModel();
            var commandResult = new CommandResult();
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(commandResult);

            // Act
            var actual = employeeController.CreatePersonalDetails(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp => { Assert.Equal(nameof(model.ProfilePicture), kvp.Key); Assert.Equal(commandResult.Message, kvp.Value); }
            );
        }

        [Fact]
        public void CreatePersonalDetails_ValidProfilePictureAndModelStateIsInvalid_ReturnExpectedErrors()
        {
            // Arrange
            var model = new CreateEmployeeViewModel();
            var commandResult = new CommandResult { Code = 0 };
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(commandResult);
            employeeController.ModelState.AddModelError("nickname", "required");

            // Act
            var actual = employeeController.CreatePersonalDetails(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("nickname", kvp.Key); Assert.Equal("required", kvp.Value); }
            );
        }

        [Fact]
        public void CreatePersonalDetails_ValidProfilePictureAndModelStateIsValid_ReturnTrue()
        {
            // Arrange
            var model = new CreateEmployeeViewModel();
            var commandResult = new CommandResult { Code = 0 };
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(commandResult);

            // Act
            var actual = employeeController.CreatePersonalDetails(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        #endregion CreateEmployee

        #region UpdateEmployee

        [Fact]
        public void UpdateEmployee_WhenViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var employeeController = GetEmployeeController();
            employeeService.GetPersonalDetails(Arg.Any<int>()).Returns(x => null);

            // Act
            var result = employeeController.UpdateEmployee(0);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public void UpdateEmployee_WhenViewModelHasValue_ReturnViewResultWithCreateEmployeeViewModel()
        {
            // Arrange
            var expected = new CreateEmployeeViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetPersonalDetails(Arg.Any<int>()).Returns(expected);

            // Act
            var result = employeeController.UpdateEmployee(99);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task UpdatePersonalDetails_InvalidProfilePicture_ReturnJsonResultWithErrorList()
        {
            // Arrange
            var model = new CreateEmployeeViewModel();
            var commandResult = new CommandResult();
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(commandResult);

            // Act
            var result = await employeeController.UpdatePersonalDetails(model);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            var wrapper = (IDictionary<string, object>)new RouteValueDictionary(viewResult.Value);
            Assert.False((bool)wrapper["isSuccessful"]);
            var errors = (Dictionary<string, string>)(wrapper["errors"]);
            Assert.Collection(
                errors,
                kvp => { Assert.Equal(nameof(model.ProfilePicture), kvp.Key); Assert.Equal(commandResult.Message, kvp.Value); }
            );
        }

        [Fact]
        public async Task UpdatePersonalDetails_ValidProfilePictureAndModelStateIsInvalid_ReturnJsonResultWithErrorList()
        {
            // Arrange
            var model = new CreateEmployeeViewModel();
            var commandResult = new CommandResult { Code = 0 };
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(commandResult);
            employeeController.ModelState.AddModelError("nickname", "required");

            // Act
            var result = await employeeController.UpdatePersonalDetails(model);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            var wrapper = (IDictionary<string, object>)new RouteValueDictionary(viewResult.Value);
            Assert.False((bool)wrapper["isSuccessful"]);
            var errors = (Dictionary<string, string>)(wrapper["errors"]);
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("nickname", kvp.Key); Assert.Equal("required", kvp.Value); }
            );
        }

        [Fact]
        public async Task UpdatePersonalDetails_ModelStateIsValid_ReturnJsonResultWithMessage()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0,
                Message = "Successful."
            };
            var model = new CreateEmployeeViewModel();
            var employeeController = GetEmployeeController();
            employeeService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(new CommandResult { Code = 0 });
            employeeService.UpdatePersonalDetailsAsync(Arg.Any<CreateEmployeeViewModel>()).Returns(expected);

            // Act
            var result = await employeeController.UpdatePersonalDetails(model);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            var wrapper = (IDictionary<string, object>)new RouteValueDictionary(viewResult.Value);

            Assert.True((bool)wrapper["isSuccessful"]);
            Assert.IsType<string>(wrapper["message"]);
        }

        #endregion UpdateEmployee

        #region CreateEmployment

        [Fact]
        public void CreateEmploymentGet_CheckViewResult_ReturnAsExpected()
        {
            //Arrange
            var viewName = "_CreateEmployment";
            var expected = new CreateEmploymentViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetDataToCreateEmployment().Returns(new CreateEmploymentViewModel());

            //Act
            var actual = employeeController.CreateEmployment();

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.IsAssignableFrom<CreateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(viewName, viewResult.ViewName);
        }

        [Fact]
        public async Task CreateEmploymentPost_ModelStateIsInvalid_ReturnExpectedErrors()
        {
            // Arrange
            var model = new CreateEmploymentViewModel();
            var employeeController = GetEmployeeController();
            employeeController.ModelState.AddModelError("company", "required");

            // Act
            var actual = await employeeController.CreateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("company", kvp.Key); Assert.Equal("required", kvp.Value); }
            );
        }

        [Fact]
        public async Task CreateEmploymentPost_ModelStateIsValid_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new CreateEmploymentViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var employeeController = GetEmployeeController();
            employeeService.CreateEmployeeAsync(Arg.Any<CreateEmploymentViewModel>()).Returns(expected);

            // Act
            var actual = await employeeController.CreateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #endregion CreateEmployment

        #region UpdateEmployment

        [Fact]
        public void UpdateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            // Arrange
            var expected = "_UpdateEmployment";
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(new UpdateEmploymentViewModel());

            // Act
            var actual = employeeController.UpdateEmployment(1);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public void UpdateEmployment_ViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(x => null);

            // Act
            var result = employeeController.UpdateEmployment(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public void UpdateEmploymentGet_ViewModelHasValue_ReturnExpectedViewModel()
        {
            // Arrange
            var employeeId = 1;
            var expected = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId
            };
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(expected);

            // Act
            var actual = employeeController.UpdateEmployment(employeeId);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.IsAssignableFrom<UpdateEmploymentViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task UpdateEmploymentPost_ModelStateIsInvalid_ReturnExpectedErrors()
        {
            // Arrange
            var model = new UpdateEmploymentViewModel();
            var employeeController = GetEmployeeController();
            employeeController.ModelState.AddModelError("company", "required");

            // Act
            var actual = await employeeController.UpdateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("company", kvp.Key); Assert.Equal("required", kvp.Value); }
            );
        }

        [Fact]
        public async Task UpdateEmploymentPost_ModelStateIsValid_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new UpdateEmploymentViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var employeeController = GetEmployeeController();
            employeeService.UpdateEmploymentAsync(Arg.Any<UpdateEmploymentViewModel>()).Returns(expected);

            // Act
            var actual = await employeeController.UpdateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #endregion UpdateEmployment

        [Fact]
        public async Task CheckEmployeeCodeExisted_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var expected = true;
            var employeeController = GetEmployeeController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<CheckEmpCodeExistedCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.CheckEmployeeCodeExisted(1, "DAR", "DAR-1234");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEqual(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetDivisionByCompanyId_WhenCalled_ReturnExpectedDivisions()
        {
            // Arrange
            var expected = new List<Division>
            {
                new Division { DivisionId = 1, CompanyId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<Division>(Arg.Any<GetDivisionByCompanyIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetDivisionByCompanyId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetDepartmentByDivisionId_WhenCalled_ReturnExpectedDepartments()
        {
            // Arrange
            var expected = new List<Department>
            {
                new Department { DepartmentId = 1, DivisionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<Department>(Arg.Any<GetDepartmentByDivisionIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetDepartmentByDivisionId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetSectionByCostCenterId_WhenCalled_ReturnExpectedSections()
        {
            // Arrange
            var expected = new List<Section>
            {
                new Section { SectionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<Section>(Arg.Any<GetSectionByCostCenterIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetSectionByCostCenterId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetCostCenterDepartmentId_WhenCalled_ReturnExpectedDivisions()
        {
            // Arrange
            var expected = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<CostCenter>(Arg.Any<GetCostCenterDepartmentIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetCostCenterDepartmentId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestion_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, IDNo = "DAR111", FullName = "Tim", Nickname = "tim.trinh" }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByIDNoCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetEmployeeSuggestion("dar");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetGradeLevelByJobCategoryId_WhenCalled_ReturnExpectedGradeLevels()
        {
            // Arrange
            var expected = new List<GradeLevel>
            {
                new GradeLevel { GradeLevelId = 1, Name = "SM", Description = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<GradeLevel>(Arg.Any<GetGradeLevelByJobCategoryIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetGradeLevelByJobCategoryId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByName_WhenCalled_ReturnJsonResult()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>();
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByNameCriteria>()).Returns(expected);

            // Actual
            var actual = await employeeController.GetEmployeeSuggestionByName("oliver");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetJobGradeByGradeLevelId_WhenCalled_ReturnExpectedJobGrades()
        {
            // Arrange
            var expected = new List<JobGrade>
            {
                new JobGrade { JobGradeId = 1, Name = "A1", Description = "A1", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<JobGrade>(Arg.Any<GetJobGradeByGradeLevelIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetJobGradeByGradeLevelId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetCityByCountryId_WhenCalled_ReturnExpectedCities()
        {
            // Arrange
            var expected = new List<City>
            {
                new City { CityId = 1, Code = "VNM", Name = "Viet nam", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<City>(Arg.Any<GetCityByCountryIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetCityByCountryId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetOfficeByCityId_WhenCalled_ReturnExpectedCities()
        {
            // Arrange
            var expected = new List<Office>
            {
                new Office { OfficeId = 1, Code = "SMS", Name = "SMS", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<Office>(Arg.Any<GetOfficeByCityIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetOfficeByCityId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public void Payroll_InvalidEmployeeId_RedirectToErrorPage()
        {
            // Arrange
            var employeeId = 0;
            var employeeController = GetEmployeeController();
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(x => null);

            // Act
            var actual = employeeController.Payroll(employeeId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("PageNotFound", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        [Fact]
        public void Payroll_ValidEmployeeId_ReturnView()
        {
            // Arrange
            var employeeId = 1;
            var employeeController = GetEmployeeController();
            var viewModel = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                FullName = "Full Name",
                Nickname = "Nick Name",
                Code = "DAR",
                CompanyId = 1,
                BasicSalary = new EmployeePayrollBasicSalaryModel
                {
                    EmployeeId = employeeId,
                    EbankingAccountId = 1,
                    EbankingAccountName = "E Account",
                    DirectBanking1AccountId = 1,
                    DirectBanking1BankTypeId = 1,
                    DirectBanking1AccountName = "Account 1",
                    DirectBanking2AccountId = 2,
                    DirectBanking2BankTypeId = 2,
                    DirectBanking2AccountName = "Account 2",
                }
            };
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(viewModel);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);

            // Act
            var actual = employeeController.Payroll(employeeId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EmployeePayrollViewModel>(viewResult.Model);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void EditPayroll_ValidModel_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeId = 1;
            var model = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId },
                Allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId },
                Deduction = new EmployeePayrollDeduction { EmployeeId = employeeId }
            };
            var expected = new CommandResult { Code = 0 };
            var employeeController = GetEmployeeController();
            employeeController.Url = Substitute.For<IUrlHelper>();
            employeeController.Url.Action().ReturnsForAnyArgs("/Employee/Index");
            payrollService.UpdatePayroll(model, sessionService.UserId).Returns(expected);

            // Act
            var actual = employeeController.EditPayroll(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected.IsSuccessful, jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(expected.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
            Assert.Equal("/Employee/Index", jsonResult.Value.GetType().GetProperty("redirectTo").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditPayroll_InValidModel_ReturnErrors()
        {
            // Arrange
            var model = new EmployeePayrollViewModel();
            var employeeController = GetEmployeeController();
            employeeController.ModelState.AddModelError("employee", "error");

            // Act
            var actual = employeeController.EditPayroll(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static IEmployeeService FakeEmployeeService()
        {
            var service = Substitute.For<IEmployeeService>();

            return service;
        }

        private static IEmployeePayrollService FakeEmployeePayrollServicee()
        {
            var service = Substitute.For<IEmployeePayrollService>();

            return service;
        }

        private EmployeeController GetEmployeeController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            employeeService = FakeEmployeeService();
            payrollService = FakeEmployeePayrollServicee();

            var controller = new EmployeeController(baseService, employeeService, payrollService, sessionService);

            return controller;
        }

        #endregion Helpers
    }
}