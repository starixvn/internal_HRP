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
        private EmployeeManagementController employeeManagementController;
        private Fixture fixture;

        public EmployeeManagementControllerTests()
        {
            fixture = InitFixture();
        }

        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            employeeManagementController = GetEmployeeManagementController();
            var listCompany = new ListCompanyDivisionDepartment
            {
                Companies = new List<Company>(),
                Divisions = new List<Division>(),
                Departments = new List<Department>()
            };

            //Act
            employeeManagementService.GetAllCompanyDivisonDepartment().Returns(listCompany);
            var actual = employeeManagementController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<EmployeeManagementViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void GetCompanyTree_WhenCalled_ReturnJsonResult()
        {
            employeeManagementController = GetEmployeeManagementController();
            var listCompany = new ListCompanyDivisionDepartment
            {
                Companies = new List<Company>(),
                Divisions = new List<Division>(),
                Departments = new List<Department>()
            };

            //Act
            employeeManagementService.GetAllCompanyDivisonDepartment().Returns(listCompany);
            var actual = employeeManagementController.GetCompanyTree();

            //Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public void EmployeeList_WhenCall_ReturnPartialViewAndEmployeeList()
        {
            employeeManagementController = GetEmployeeManagementController();
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
            var actual = employeeManagementController.EmployeeList(viewModel);
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
            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetEmployeeById(Arg.Any<int>()).Returns(x => null);

            // Act
            var result = employeeManagementController.UpdateEmployee(0);

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
            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetEmployeeById(Arg.Any<int>()).Returns(expected);

            // Act
            var result = employeeManagementController.UpdateEmployee(99);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task UpdatePersonalDetails_ModelStateIsInvalid_ReturnJsonResultWithErrorList()
        {
            // Arrange
            var model = fixture.Create<CreateEmployeeViewModel>();
            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(new CommandResult());

            // Act
            var result = await employeeManagementController.UpdatePersonalDetails(model);

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
            var expected = new CommandResult()
            {
                Code = 0,
                Message = "Successful."
            };

            var model = fixture.Create<CreateEmployeeViewModel>();
            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.CheckValidPicture(Arg.Any<IFormFile>()).Returns(new CommandResult { Code = 0 });
            employeeManagementService.UpdatePersonalDetailsAsync(Arg.Any<CreateEmployeeViewModel>()).Returns(expected);

            // Act
            var result = await employeeManagementController.UpdatePersonalDetails(model);

            // Assert
            var viewResult = Assert.IsType<JsonResult>(result);
            var wrapper = (IDictionary<string, object>)new RouteValueDictionary(viewResult.Value);

            Assert.True((bool)wrapper["isSuccessful"]);
            Assert.IsType<string>(wrapper["message"]);
        }

        #endregion UpdateEmployee

        #region UpdateEmployment

        [Fact]
        public void UpdateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            //Arrange
            var expected = "_UpdateEmployment";
            employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(new UpdateEmploymentViewModel());

            //Act
            var actual = employeeManagementController.UpdateEmployment(1);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public void UpdateEmployment_ViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(x => null);

            // Act
            var result = employeeManagementController.UpdateEmployment(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public void UpdateEmploymentGet_ViewModelHasValue_ReturnExpectedViewModel()
        {
            //Arrange
            var employeeId = 1;
            var expected = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId
            };
            employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetDataToUpdateEmployment(Arg.Any<int>()).Returns(expected);

            //Act
            var actual = employeeManagementController.UpdateEmployment(employeeId);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<UpdateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(expected.EmployeeId, viewModel.EmployeeId);
        }

        #endregion UpdateEmployment

        [Fact]
        public async Task CheckEmployeeCodeExisted_WhenCalled_ReturnAsExpected()
        {
            //Arrange
            var expected = true;
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<CheckEmpCodeExistedCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.CheckEmployeeCodeExisted(1, "DAR", "DAR-1234");

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEqual(expected, (bool)jsonResult.Value);
        }

        [Fact]
        public async Task GetDivisionByCompanyId_WhenCalled_ReturnExpectedDivisions()
        {
            //Arrange
            var expected = new List<Division>
            {
                new Division { DivisionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<Division>(Arg.Any<GetDivisionByCompanyIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetDivisionByCompanyId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Division>)jsonResult.Value);
        }

        [Fact]
        public async Task GetDepartmentByDivisionId_WhenCalled_ReturnExpectedDepartments()
        {
            //Arrange
            var expected = new List<Department>
            {
                new Department { DepartmentId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<Department>(Arg.Any<GetDepartmentByDivisionIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetDepartmentByDivisionId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Department>)jsonResult.Value);
        }

        [Fact]
        public async Task GetSectionByCostCenterId_WhenCalled_ReturnExpectedSections()
        {
            //Arrange
            var expected = new List<Section>
            {
                new Section { SectionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<Section>(Arg.Any<GetSectionByCostCenterIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetSectionByCostCenterId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Section>)jsonResult.Value);
        }

        [Fact]
        public async Task GetCostCenterDepartmentId_WhenCalled_ReturnExpectedDivisions()
        {
            //Arrange
            var expected = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<CostCenter>(Arg.Any<GetCostCenterDepartmentIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetCostCenterDepartmentId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<CostCenter>)jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestion_WhenCalled_ReturnExpectedEmployees()
        {
            //Arrange
            var expected = new List<EmployeeSuggestionViewModel>
            {
                new EmployeeSuggestionViewModel { EmployeeId = 1, FullName = "Tim", NickName = "tim.trinh" }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByIDNoCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetEmployeeSuggestion("dar");

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<EmployeeSuggestionViewModel>)jsonResult.Value);
        }

        [Fact]
        public async Task GetGradeLevelByJobCategoryId_WhenCalled_ReturnExpectedGradeLevels()
        {
            //Arrange
            var expected = new List<GradeLevel>
            {
                new GradeLevel { GradeLevelId = 1, Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<GradeLevel>(Arg.Any<GetGradeLevelByJobCategoryIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetGradeLevelByJobCategoryId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<GradeLevel>)jsonResult.Value);
        }

        [Fact]
        public async Task GetEmployeeSuggestionByName_WhenCalled_ReturnJsonResult()
        {
            //Arrange
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionByNameCriteria>()).Returns(new List<EmployeeSuggestionViewModel>());

            //Actual
            var actual = await employeeManagementController.GetEmployeeSuggestionByName("oliver");

            //Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Fact]
        public async Task GetJobGradeByGradeLevelId_WhenCalled_ReturnExpectedJobGrades()
        {
            //Arrange
            var expected = new List<JobGrade>
            {
                new JobGrade { JobGradeId = 1, Name = "A1", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<JobGrade>(Arg.Any<GetJobGradeByGradeLevelIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetJobGradeByGradeLevelId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<JobGrade>)jsonResult.Value);
        }

        [Fact]
        public async Task GetCityByCountryId_WhenCalled_ReturnExpectedCities()
        {
            //Arrange
            var expected = new List<City>
            {
                new City { CityId = 1, Code = "VNM", Name = "Viet nam", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<City>(Arg.Any<GetCityByCountryIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetCityByCountryId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<City>)jsonResult.Value);
        }

        [Fact]
        public async Task GetOfficeByCityId_WhenCalled_ReturnExpectedCities()
        {
            //Arrange
            var expected = new List<Office>
            {
                new Office { OfficeId = 1, Code = "SMS", Name = "SMS", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<Office>(Arg.Any<GetOfficeByCityIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetOfficeByCityId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Office>)jsonResult.Value);
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

        private IEmployeeManagementService FakeEmployeeManagementService()
        {
            var service = Substitute.For<IEmployeeManagementService>();

            return service;
        }

        private EmployeeManagementController GetEmployeeManagementController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            employeeManagementService = FakeEmployeeManagementService();
            var controller = new EmployeeManagementController(baseService, employeeManagementService, null, sessionService);

            return controller;
        }

        private Fixture InitFixture()
        {
            var fixture = new Fixture();
            fixture.Register<IFormFile>(() => null);
            fixture.Register<SelectList>(() => null);

            return fixture;
        }

        #endregion Helpers
    }
}