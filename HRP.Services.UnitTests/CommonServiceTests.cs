namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;
    using NSubstitute;
    using Xunit;

    public class CommonServiceTests
    {
        private IBaseService baseService;

        #region GetReferenceResourcesByNames

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetReferenceResourcesByNames_ParamIsNotNull_ReturnAsExpectedReferenceResources()
        {
            // Arrange
            var expected = GetRefResourcesForTest();

            var refNames = new[]
            {
                "BankType",
                "BankAccountType"
            };

            var service = GetCommonService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(expected);

            // Act
            var actual = await service.GetReferenceResourcesByNamesAsync(refNames);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetReferenceResourcesByNames

        #region FilterReferenceResources

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNull_ReturnEmptyListReferenceResources()
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(null, null);

            // Assert
            Assert.Empty(actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FilterReferenceResources_RefNameIsInvalid_ReturnEmptyListReferenceResources(string refName)
        {
            // Arrange
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(Arg.Any<IEnumerable<ReferenceResourceModel>>(), refName);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void FilterReferenceResources_RefResourcesIsNotNullAndRefNameIsValid_ReturnExpectedReferenceResources()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var refName = "BankType";
            var service = GetCommonService();

            // Act
            var actual = service.FilterReferenceResources(refResources, refName);

            // Assert
            Assert.Single(actual);
            Assert.Equal(refResources.First(), actual.First());
        }

        #endregion FilterReferenceResources

        [Fact]
        public async Task GetCompanyByFunctionKeyAsync_WhenCalled_ReturnAsExpectedCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { CompanyId = 1 }
            };
            var functionKey = "FunctionKey";
            var criteria = new GetCompanyByFunctionKeyCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<Company>(Arg.Do<GetCompanyByFunctionKeyCriteria>(x => criteria = x)).Returns(companies);

            // Act
            var actual = await service.GetCompanyByFunctionKeyAsync(functionKey);

            // Assert
            Assert.Equal(companies, actual);
            Assert.Equal(functionKey, criteria.FunctionKey);
        }

        [Fact]
        public async Task GetDivisionByCompanyIdAsync_WhenCalled_ReturnAsExpectedDivisions()
        {
            // Arrange
            var companyId = 1;
            var divisions = new List<Division>
            {
                new Division { DivisionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var criteria = new GetDivisionByCompanyIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<Division>(Arg.Do<GetDivisionByCompanyIdCriteria>(x => criteria = x)).Returns(divisions);

            // Act
            var actual = await service.GetDivisionByCompanyIdAsync(companyId);

            // Assert
            Assert.Equal(divisions, actual);
            Assert.Equal(companyId, criteria.CompanyId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetDepartmentByDivisionIdAsync_WhenCalled_ReturnAsExpectedDepartments()
        {
            // Arrange
            var divisionId = 1;
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var criteria = new GetDepartmentByDivisionIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<Department>(Arg.Do<GetDepartmentByDivisionIdCriteria>(x => criteria = x)).Returns(departments);

            // Act
            var actual = await service.GetDepartmentByDivisionIdAsync(divisionId);

            // Assert
            Assert.Equal(departments, actual);
            Assert.Equal(divisionId, criteria.DivisionId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetCostCenterByDepartmentIdAsync_WhenCalled_ReturnAsExpectedCostCenters()
        {
            // Arrange
            var departmentId = 1;
            var costCenters = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var criteria = new GetCostCenterByDepartmentIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<CostCenter>(Arg.Do<GetCostCenterByDepartmentIdCriteria>(x => criteria = x)).Returns(costCenters);

            // Act
            var actual = await service.GetCostCenterByDepartmentIdAsync(departmentId);

            // Assert
            Assert.Equal(costCenters, actual);
            Assert.Equal(departmentId, criteria.DepartmentId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetSectionByCostCenterIdAsync_WhenCalled_ReturnAsExpectedSections()
        {
            // Arrange
            var costCenterId = 1;
            var sections = new List<Section>
            {
                new Section { SectionId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var criteria = new GetSectionByCostCenterIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<Section>(Arg.Do<GetSectionByCostCenterIdCriteria>(x => criteria = x)).Returns(sections);

            // Act
            var actual = await service.GetSectionByCostCenterIdAsync(costCenterId);

            // Assert
            Assert.Equal(sections, actual);
            Assert.Equal(costCenterId, criteria.CostCenterId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetGradeLevelByJobCategoryIdAsync_WhenCalled_ReturnAsExpectedGradeLevels()
        {
            // Arrange
            var jobCategoryId = 1;
            var gradeLevels = new List<GradeLevel>
            {
                new GradeLevel { GradeLevelId = 1, Name = "SM", Description = "Senior Management", IsActive = true }
            };
            var criteria = new GetGradeLevelByJobCategoryIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<GradeLevel>(Arg.Do<GetGradeLevelByJobCategoryIdCriteria>(x => criteria = x)).Returns(gradeLevels);

            // Act
            var actual = await service.GetGradeLevelByJobCategoryIdAsync(jobCategoryId);

            // Assert
            Assert.Equal(gradeLevels, actual);
            Assert.Equal(jobCategoryId, criteria.JobCategoryId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetJobGradeByGradeLevelIdAsync_WhenCalled_ReturnAsExpectedJobGrades()
        {
            // Arrange
            var gradeLevelId = 1;
            var jobGrades = new List<JobGrade>
            {
                new JobGrade { JobGradeId = 1, Name = "A1", Description = "A1", IsActive = true }
            };
            var criteria = new GetJobGradeByGradeLevelIdCriteria();
            var service = GetCommonService();
            baseService.GetDataAsync<JobGrade>(Arg.Do<GetJobGradeByGradeLevelIdCriteria>(x => criteria = x)).Returns(jobGrades);

            // Act
            var actual = await service.GetJobGradeByGradeLevelIdAsync(gradeLevelId);

            // Assert
            Assert.Equal(jobGrades, actual);
            Assert.Equal(gradeLevelId, criteria.GradeLevelId);
            Assert.True(criteria.IsActive);
        }

        [Fact]
        public async Task GetAllJobCategoryAsync_WhenCalled_ReturnAsExpectedJobCategories()
        {
            // Arrange
            var jobCategories = new List<JobCategory>
            {
                new JobCategory { JobCategoryId = 1 }
            };
            var service = GetCommonService();
            baseService.GetDataAsync<JobCategory>(Arg.Any<GetAllJobCategoryCriteria>()).Returns(jobCategories);

            // Act
            var actual = await service.GetAllJobCategoryAsync();

            // Assert
            Assert.Equal(jobCategories, actual);
        }

        [Fact]
        public void GetSelectList_WhenCalled_ReturnAsExpectedSelectList()
        {
            // Arrange
            var refResources = GetRefResourcesForTest();
            var service = GetCommonService();

            // Act
            var actual = service.GetSelectList(
                refResources,
                nameof(ReferenceResourceModel.ResourceId),
                nameof(ReferenceResourceModel.Value));

            // Assert
            Assert.Equal(refResources.Count, actual.Count());
        }

        #region Helpers

        private List<ReferenceResourceModel> GetRefResourcesForTest()
        {
            return new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel
                {
                    ResourceId = 1,
                    RefName = "BankType",
                    Value = "1",
                    Description = "DAR",
                    IsActive = true
                },
                new ReferenceResourceModel
                {
                    ResourceId = 2,
                    RefName = "LocalCompany",
                    Value = "1",
                    Description = "NEX",
                    IsActive = true
                }
            };
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private CommonService GetCommonService()
        {
            baseService = FakeBaseService();
            var sessionService = FakeSessionService();
            var service = new CommonService(baseService, sessionService);

            return service;
        }

        #endregion Helpers
    }
}