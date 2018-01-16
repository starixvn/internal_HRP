namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IEmployeeManagementService employeeManagementService;
        private EmployeeManagementController employeeManagementController;

        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            employeeManagementController = GetEmployeeManagementController();

            //Act
            var actual = employeeManagementController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        #region UpdateEmployment

        [Fact]
        public async Task UpdateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            //Arrange
            var expected = "_UpdateEmployment";
            employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetEmploymentToUpdate(Arg.Any<int>()).Returns(new UpdateEmploymentViewModel());

            //Act
            var actual = await employeeManagementController.UpdateEmployment(1);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public async Task UpdateEmploymentGet_CheckViewModel_ReturnExpectedViewModel()
        {
            //Arrange
            var employeeId = 1;
            var expected = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId
            };
            employeeManagementController = GetEmployeeManagementController();
            employeeManagementService.GetEmploymentToUpdate(Arg.Any<int>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.UpdateEmployment(employeeId);

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
        public async Task GetSectionByDepartmentId_WhenCalled_ReturnExpectedSections()
        {
            //Arrange
            var expected = new List<Section>
            {
                new Section { SectionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<Section>(Arg.Any<GetSectionByDepartmentIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetSectionByDepartmentId(1);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (IEnumerable<Section>)jsonResult.Value);
        }

        [Fact]
        public async Task GetCostCenterBySectionId_WhenCalled_ReturnExpectedDivisions()
        {
            //Arrange
            var expected = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataAsync<CostCenter>(Arg.Any<GetCostCenterBySectionIdCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.GetCostCenterBySectionId(1);

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
            baseService.GetDataAsync<EmployeeSuggestionViewModel>(Arg.Any<GetEmployeeSuggestionCriteria>()).Returns(expected);

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
            var controller = new EmployeeManagementController(baseService, employeeManagementService, null);

            return controller;
        }

        #endregion Helpers
    }
}