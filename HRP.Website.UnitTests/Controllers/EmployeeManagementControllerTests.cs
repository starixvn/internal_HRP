namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IEmployeeManagementService employeeManagementService;
        private IEmployeePayrollService payrollService;
        private Fixture fixture;

        public EmployeeManagementControllerTests()
        {
            fixture = InitFixture();
        }

        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            var employeeController = GetEmployeeManagementController();
            var listCompany = new ListCompanyDivisionDepartment
            {
                Companies = new List<Company>(),
                Divisions = new List<Division>(),
                Departments = new List<Department>()
            };

            //Act
            employeeManagementService.GetAllCompanyDivisonDepartment().Returns(listCompany);
            var actual = employeeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<EmployeeManagementViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void GetCompanyTree_WhenCalled_ReturnJsonResult()
        {
            var employeeController = GetEmployeeManagementController();
            var listCompany = new ListCompanyDivisionDepartment
            {
                Companies = new List<Company>(),
                Divisions = new List<Division>(),
                Departments = new List<Department>()
            };

            //Act
            employeeManagementService.GetAllCompanyDivisonDepartment().Returns(listCompany);
            var actual = employeeController.GetCompanyTree();

            //Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public void EmployeeList_WhenCall_ReturnPartialViewAndEmployeeList()
        {
            var employeeController = GetEmployeeManagementController();
            var listCompany = new ListCompanyDivisionDepartment
            {
                Companies = new List<Company>(),
                Divisions = new List<Division>(),
                Departments = new List<Department>()
            };
            employeeManagementService.GetAllCompanyDivisonDepartment().Returns(listCompany);
            var employeeStatus = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.EmploymentStatus });
            sessionService.UserId = 1;
            var viewModel = new EmployeeManagementViewModel
            {
                DepartmentIds = string.Empty,
                EmployeeStatusIds = "1,2,3,4,5",
                EmployeeStatus = new SelectList(employeeStatus, "ResourceId", "Value"),
                EmployeeId = 41
            };

            //Act
            var actual = employeeController.EmployeeList(viewModel);
            var viewResult = Assert.IsType<PartialViewResult>(actual);

            //Assert
            Assert.Equal("_ListEmployee", viewResult.ViewName);
            Assert.IsAssignableFrom<IEnumerable<EmployeeBasicInfo>>(viewResult.ViewData.Model);
        }

        #region UpdateEmployee

        [Fact]
        public void UpdateEmployee_WhenViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetEmployeeById(Arg.Any<int>()).Returns(x => null);

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
            var expected = fixture.Create<CreateEmployeeViewModel>();
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetEmployeeById(Arg.Any<int>()).Returns(expected);

            // Act
            var result = employeeController.UpdateEmployee(99);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task UpdatePersonalDetails_ModelStateIsInvalid_ReturnJsonResultWithErrorList()
        {
            // Arrange
            var model = fixture.Create<CreateEmployeeViewModel>();
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(new CommandResult());

            // Act
            var result = await employeeController.UpdatePersonalDetails(model);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            var wrapper = (IDictionary<string, object>)new RouteValueDictionary(viewResult.Value);

            Assert.False((bool)wrapper["isSuccessful"]);
            Assert.IsType<Dictionary<string, string>>(wrapper["errors"]);
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

            var model = fixture.Create<CreateEmployeeViewModel>();
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(new CommandResult { Code = 0 });
            employeeManagementService.UpdatePersonalDetailsAsync(Arg.Any<CreateEmployeeViewModel>()).Returns(expected);

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
        public void CreateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            // Arrange
            var expected = "_CreateEmployment";
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetDataToCreateEmployment().Returns(new CreateEmploymentViewModel());

            // Act
            var actual = employeeController.CreateEmployment();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public void CreateEmploymentGet_CheckViewModel_ReturnExpectedViewModel()
        {
            //Arrange
            var departmentIds = "1,2";
            var employeeStatusIds = "1,2";
            var employeeFilterId = 1;
            var expected = new CreateEmploymentViewModel();
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetDataToCreateEmployment().Returns(expected);

            //Act
            var actual = employeeController.CreateEmployment(departmentIds, employeeStatusIds, employeeFilterId);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(departmentIds, viewModel.Filter.DepartmentIds);
            Assert.Equal(employeeStatusIds, viewModel.Filter.EmployeeStatusIds);
            Assert.Equal(employeeFilterId, viewModel.Filter.EmployeeId);
        }

        [Fact]
        public async Task CreateEmploymentPost_ModelStateIsInvalid_ReturnExpectedErrors()
        {
            // Arrange
            var model = new CreateEmploymentViewModel();
            var employeeController = GetEmployeeManagementController();
            employeeController.ModelState.AddModelError("company", "required");

            // Act
            var actual = await employeeController.CreateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Contains(errors.Keys, x => x == "company");
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
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.CreateEmployeeAsync(Arg.Any<CreateEmploymentViewModel>()).Returns(expected);

            // Act
            var actual = await employeeController.CreateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        #endregion CreateEmployment

        #region UpdateEmployment

        [Fact]
        public void UpdateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            // Arrange
            var expected = "_UpdateEmployment";
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(new UpdateEmploymentViewModel());

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
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(x => null);

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
            var departmentIds = "1,2";
            var employeeStatusIds = "1,2";
            var employeeFilterId = 1;
            var expected = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId
            };
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(expected);

            // Act
            var actual = employeeController.UpdateEmployment(employeeId, departmentIds, employeeStatusIds, employeeFilterId);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<UpdateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(expected.EmployeeId, viewModel.EmployeeId);
            Assert.Equal(departmentIds, viewModel.Filter.DepartmentIds);
            Assert.Equal(employeeStatusIds, viewModel.Filter.EmployeeStatusIds);
            Assert.Equal(employeeFilterId, viewModel.Filter.EmployeeId);
        }

        [Fact]
        public async Task UpdateEmploymentPost_ModelStateIsInvalid_ReturnExpectedErrors()
        {
            // Arrange
            var model = new UpdateEmploymentViewModel();
            var employeeController = GetEmployeeManagementController();
            employeeController.ModelState.AddModelError("company", "required");

            // Act
            var actual = await employeeController.UpdateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Contains(errors.Keys, x => x == "company");
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
            var employeeController = GetEmployeeManagementController();
            employeeManagementService.UpdateEmploymentAsync(Arg.Any<UpdateEmploymentViewModel>()).Returns(expected);

            // Act
            var actual = await employeeController.UpdateEmployment(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        #endregion UpdateEmployment

        [Fact]
        public async Task CheckEmployeeCodeExisted_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var expected = true;
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<CheckEmpCodeExistedCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.CheckEmployeeCodeExisted(1, "DAR", "DAR-1234");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEqual(expected, (bool)jsonResult.Value);
        }

        [Fact]
        public async Task GetDivisionByCompanyId_WhenCalled_ReturnExpectedDivisions()
        {
            // Arrange
            var expected = new List<Division>
            {
                new Division { DivisionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<Division>(Arg.Any<GetDivisionByCompanyIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetDivisionByCompanyId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Division>)jsonResult.Value);
        }

        [Fact]
        public async Task GetDepartmentByDivisionId_WhenCalled_ReturnExpectedDepartments()
        {
            // Arrange
            var expected = new List<Department>
            {
                new Department { DepartmentId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<Department>(Arg.Any<GetDepartmentByDivisionIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetDepartmentByDivisionId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Department>)jsonResult.Value);
        }

        [Fact]
        public async Task GetSectionByCostCenterId_WhenCalled_ReturnExpectedSections()
        {
            // Arrange
            var expected = new List<Section>
            {
                new Section { SectionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<Section>(Arg.Any<GetSectionByCostCenterIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetSectionByCostCenterId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Section>)jsonResult.Value);
        }

        [Fact]
        public async Task GetCostCenterDepartmentId_WhenCalled_ReturnExpectedDivisions()
        {
            // Arrange
            var expected = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<CostCenter>(Arg.Any<GetCostCenterDepartmentIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetCostCenterDepartmentId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<CostCenter>)jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestion_WhenCalled_ReturnExpectedEmployees()
        {
            // Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, FullName = "Tim", Nickname = "tim.trinh" }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByIDNoCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetEmployeeSuggestion("dar");

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<EmployeeSuggestionViewModel>)jsonResult.Value);
        }

        [Fact]
        public async Task GetGradeLevelByJobCategoryId_WhenCalled_ReturnExpectedGradeLevels()
        {
            // Arrange
            var expected = new List<GradeLevel>
            {
                new GradeLevel { GradeLevelId = 1, Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<GradeLevel>(Arg.Any<GetGradeLevelByJobCategoryIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetGradeLevelByJobCategoryId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<GradeLevel>)jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByName_WhenCalled_ReturnJsonResult()
        {
            // Arrange
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByNameCriteria>()).Returns(new List<EmployeeSuggestionViewModel>());

            // Actual
            var actual = await employeeController.GetEmployeeSuggestionByName("oliver");

            // Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public async Task GetJobGradeByGradeLevelId_WhenCalled_ReturnExpectedJobGrades()
        {
            // Arrange
            var expected = new List<JobGrade>
            {
                new JobGrade { JobGradeId = 1, Name = "A1", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<JobGrade>(Arg.Any<GetJobGradeByGradeLevelIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetJobGradeByGradeLevelId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<JobGrade>)jsonResult.Value);
        }

        [Fact]
        public async Task GetCityByCountryId_WhenCalled_ReturnExpectedCities()
        {
            // Arrange
            var expected = new List<City>
            {
                new City { CityId = 1, Code = "VNM", Name = "Viet nam", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<City>(Arg.Any<GetCityByCountryIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetCityByCountryId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<City>)jsonResult.Value);
        }

        [Fact]
        public async Task GetOfficeByCityId_WhenCalled_ReturnExpectedCities()
        {
            // Arrange
            var expected = new List<Office>
            {
                new Office { OfficeId = 1, Code = "SMS", Name = "SMS", IsActive = true }
            };
            var employeeController = GetEmployeeManagementController();
            baseService.GetDataAsync<Office>(Arg.Any<GetOfficeByCityIdCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetOfficeByCityId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Office>)jsonResult.Value);
        }


        [Fact]
        public void Payroll_InvalidEmployeeId_RedirectToErrorPage()
        {
            var employeeId = 0;
            var employeeManagementController = GetEmployeeManagementController();
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(x => null);

            //Act
            var actual = employeeManagementController.Payroll(employeeId);

            //Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("PageNotFound", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        [Fact]
        public void Payroll_ValidEmployeeId_ReturnView()
        {
            var employeeId = 1;
            var employeeManagementController = GetEmployeeManagementController();
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(new EmployeePayrollViewModel { EmployeeId = employeeId });

            var actual = employeeManagementController.Payroll(employeeId);

            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EmployeePayrollViewModel>(viewResult.Model);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void Payroll_ValidModel_ReturnRedirect()
        {
            var employeeId = 1;
            var model = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId },
                Allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId },
                Deduction = new EmployeePayrollDeduction { EmployeeId = employeeId }
            };

            var employeeManagementController = GetEmployeeManagementController();
            payrollService.When(x => x.UpdatePayroll(model, sessionService.UserId));

            var actual = employeeManagementController.SavePayroll(model);
            payrollService.Received().UpdatePayroll(model, sessionService.UserId);

            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Payroll_InValidModel_ReturnView()
        {
            var employeeId = 1;
            var model = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId },
                Allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId },
                Deduction = new EmployeePayrollDeduction { EmployeeId = employeeId }
            };

            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementController.ModelState.AddModelError("", "Error");

            var actual = employeeManagementController.SavePayroll(model);

            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<EmployeePayrollViewModel>(viewResult.Model);
            Assert.NotNull(viewResult.Model);
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

        private static IEmployeeManagementService FakeEmployeeManagementService()
        {
            var service = Substitute.For<IEmployeeManagementService>();

            return service;
        }

        private static IEmployeePayrollService FakeEmployeePayrollServicee()
        {
            var service = Substitute.For<IEmployeePayrollService>();

            return service;
        }

        private EmployeeManagementController GetEmployeeManagementController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            employeeManagementService = FakeEmployeeManagementService();
            payrollService = FakeEmployeePayrollServicee();

            var controller = new EmployeeManagementController(baseService, employeeManagementService, payrollService, sessionService);

            return controller;
        }

        private static Fixture InitFixture()
        {
            var fixture = new Fixture();
            fixture.Register<IFormFile>(() => null);
            fixture.Register<SelectList>(() => null);

            return fixture;
        }

        #endregion Helpers
    }
}