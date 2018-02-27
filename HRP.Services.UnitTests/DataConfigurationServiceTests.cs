namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.City;
    using HRP.Criteria.CostCenter;
    using HRP.Criteria.Country;
    using HRP.Criteria.Currency;
    using HRP.Criteria.Department;
    using HRP.Criteria.GradeLevel;
    using HRP.Criteria.JobCategory;
    using HRP.Criteria.JobGrade;
    using HRP.Criteria.Office;
    using HRP.Criteria.RefResource;
    using HRP.Criteria.Section;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.DataConfiguration;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class DataConfigurationServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        [Fact]
        public void GetDataForIndexPage_WhenCalled_ReturnOrderedFieldList()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();

            // Act
            var actual = dataConfigurationService.GetDataForIndexPage() as IOrderedEnumerable<Field>;

            // Assert
            Assert.NotNull(actual);
        }

        #region CompanyBVI

        [Fact]
        public async Task GetAllCompanyBVIAsync_BaseServiceGetCompanyList_ReturnSimpleItemList()
        {
            // Arrange
            var companies = new List<Company> { MockCompany() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            // Act
            var actual = (await dataConfigurationService.GetAllCompanyBVIAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(companies.Count, actual.Count);
            Assert.Equal(companies[0].CompanyId, actual[0].ItemId);
            Assert.Equal(companies[0].Code, actual[0].ItemName);
            Assert.Equal(companies[0].Name, actual[0].Description);
            Assert.Equal(companies[0].IsActive, actual[0].IsActive);
        }

        [Fact]
        public async Task GetAllCompanyBVIAsync_BaseServiceGetEmptyCompanyList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var companies = new List<Company>();

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            // Act
            var actual = (await dataConfigurationService.GetAllCompanyBVIAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(companies.Count, actual.Count);
        }

        [Fact]
        public async Task CreateCompanyBVIAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new CreateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCompanyCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateCompanyBVIAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetCompanyBVIByIdAsync_BaseServiceGetCompanyInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var id = 11;
            var company = MockCompany();
            var criteria = new GetCompanyByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Company>(Arg.Do<GetCompanyByIdCriteria>(x => criteria = x)).Returns(company);

            // Act
            var actual = await dataConfigurationService.GetCompanyBVIByIdAsync(id);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(company.CompanyId, actual.ItemId);
            Assert.Equal(company.Code, actual.ItemName);
            Assert.Equal(company.Name, actual.Description);
            Assert.Equal(company.IsActive, actual.IsActive);
            Assert.Equal(id, criteria.CompanyId);
        }

        [Fact]
        public async Task GetCompanyBVIByIdAsync_BaseServiceGetNullCompany_ReturnNull()
        {
            // Arrange
            Company company = null;
            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<Company>(Arg.Any<GetCompanyByIdCriteria>()).Returns(company);

            // Act
            var actual = await dataConfigurationService.GetCompanyBVIByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCompanyBVIAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new UpdateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCompanyCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateCompanyBVIAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemId, criteria.CompanyId);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion CompanyBVI

        #region Division

        [Fact]
        public async Task GetAllDivisionAsync_BaseServiceGetDivisionList_ReturnDivisionList()
        {
            // Arrange
            var expected = new List<Division> { MockDivision() };

            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataAsync<Division>(Arg.Any<GetAllDivisionCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllDivisionAsync() as List<Division>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateDivisionAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var division = MockDivision();
            var criteria = new CreateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDivisionCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateDivisionAsync(division);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(division.Code, criteria.Code);
            Assert.Equal(division.Name, criteria.Name);
            Assert.Equal(division.CompanyId, criteria.CompanyId);
            Assert.Equal(division.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetDivisionByIdAsync_BaseServiceGetDivisionInstance_ReturnDivisionInstance()
        {
            // Arrange
            var id = 21;
            var criteria = new GetDivisionByIdCriteria();
            var division = MockDivision();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Division>(Arg.Do<GetDivisionByIdCriteria>(x => criteria = x)).Returns(division);

            // Act
            var actual = await dataConfigurationService.GetDivisionByIdAsync(id);

            // Assert
            Assert.IsType<Division>(actual);
            Assert.Equal(division.DivisionId, actual.DivisionId);
            Assert.Equal(division.Code, actual.Code);
            Assert.Equal(division.Name, actual.Name);
            Assert.Equal(division.IsActive, actual.IsActive);
            Assert.Equal(division.CompanyId, actual.CompanyId);
            Assert.Equal(division.CompanyCode, actual.CompanyCode);
            Assert.Equal(id, criteria.DivisionId);
        }

        [Fact]
        public async Task GetDivisionByIdAsync_BaseServiceGetNullDivision_ReturnNull()
        {
            // Arrange
            Division division = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Division>(Arg.Any<GetDivisionByIdCriteria>()).Returns(division);

            // Act
            var actual = await dataConfigurationService.GetDivisionByIdAsync(21);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateDivisionAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var division = MockDivision();
            var criteria = new UpdateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDivisionCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateDivisionAsync(division);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(division.DivisionId, criteria.DivisionId);
            Assert.Equal(division.Code, criteria.Code);
            Assert.Equal(division.Name, criteria.Name);
            Assert.Equal(division.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Division

        #region Department

        [Fact]
        public async Task GetAllDepartmentAsync_BaseServiceGetDepartmentList_ReturnDepartmentList()
        {
            // Arrange
            var expected = new List<Department> { MockDepartment() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Department>(Arg.Any<GetAllDepartmentCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllDepartmentAsync() as List<Department>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateDepartmentAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var criteria = new CreateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDepartmentCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateDepartmentAsync(department);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(department.Code, criteria.Code);
            Assert.Equal(department.Name, criteria.Name);
            Assert.Equal(department.DivisionId, criteria.DivisionId);
            Assert.Equal(department.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_BaseServiceGetDepartmentInstance_ReturnDepartmentInstance()
        {
            // Arrange
            var id = 21;
            var department = MockDepartment();
            var criteria = new GetDepartmentByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Department>(Arg.Do<GetDepartmentByIdCriteria>(x => criteria = x)).Returns(department);

            // Act
            var actual = await dataConfigurationService.GetDepartmentByIdAsync(id);

            // Assert
            Assert.IsType<Department>(actual);
            Assert.Equal(department.DepartmentId, actual.DepartmentId);
            Assert.Equal(department.Code, actual.Code);
            Assert.Equal(department.Name, actual.Name);
            Assert.Equal(department.IsActive, actual.IsActive);
            Assert.Equal(department.CompanyId, actual.CompanyId);
            Assert.Equal(department.CompanyCode, actual.CompanyCode);
            Assert.Equal(department.DivisionId, actual.DivisionId);
            Assert.Equal(department.DivisionCode, actual.DivisionCode);
            Assert.Equal(id, criteria.DepartmentId);
        }

        [Fact]
        public async Task GetDepartmentByIdAsync_BaseServiceGetNullDepartment_ReturnNull()
        {
            // Arrange
            Department department = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Department>(Arg.Any<GetDepartmentByIdCriteria>()).Returns(department);

            // Act
            var actual = await dataConfigurationService.GetDepartmentByIdAsync(21);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var criteria = new UpdateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDepartmentCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateDepartmentAsync(department);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(department.DepartmentId, criteria.DepartmentId);
            Assert.Equal(department.Code, criteria.Code);
            Assert.Equal(department.Name, criteria.Name);
            Assert.Equal(department.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Department

        #region Cost Center

        [Fact]
        public async Task GetAllCostCenterAsync_BaseServiceGetCostCenterList_ReturnCostCenterList()
        {
            // Arrange
            var expected = new List<CostCenter> { MockCostCenter() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<CostCenter>(Arg.Any<GetAllCostCenterCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllCostCenterAsync() as List<CostCenter>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateCostCenterAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var criteria = new CreateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCostCenterCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateCostCenterAsync(costCenter);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(costCenter.Code, criteria.Code);
            Assert.Equal(costCenter.Name, criteria.Name);
            Assert.Equal(costCenter.DepartmentId, criteria.DepartmentId);
            Assert.Equal(costCenter.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetCostCenterByIdAsync_BaseServiceGetCostCenterInstance_ReturnCostCenterInstance()
        {
            // Arrange
            var id = 31;
            var costCenter = MockCostCenter();
            var criteria = new GetCostCenterByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<CostCenter>(Arg.Do<GetCostCenterByIdCriteria>(x => criteria = x)).Returns(costCenter);

            // Act
            var actual = await dataConfigurationService.GetCostCenterByIdAsync(id);

            // Assert
            Assert.IsType<CostCenter>(actual);
            Assert.Equal(costCenter.CostCenterId, actual.CostCenterId);
            Assert.Equal(costCenter.Code, actual.Code);
            Assert.Equal(costCenter.Name, actual.Name);
            Assert.Equal(costCenter.IsActive, actual.IsActive);
            Assert.Equal(costCenter.CompanyId, actual.CompanyId);
            Assert.Equal(costCenter.CompanyCode, actual.CompanyCode);
            Assert.Equal(costCenter.DivisionId, actual.DivisionId);
            Assert.Equal(costCenter.DivisionCode, actual.DivisionCode);
            Assert.Equal(costCenter.DepartmentId, actual.DepartmentId);
            Assert.Equal(costCenter.DepartmentCode, actual.DepartmentCode);
            Assert.Equal(id, criteria.CostCenterId);
        }

        [Fact]
        public async Task GetCostCenterByIdAsync_BaseServiceGetNullCostCenter_ReturnNull()
        {
            // Arrange
            CostCenter costCenter = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<CostCenter>(Arg.Any<GetCostCenterByIdCriteria>()).Returns(costCenter);

            // Act
            var actual = await dataConfigurationService.GetCostCenterByIdAsync(21);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCostCenterAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var criteria = new UpdateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCostCenterCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateCostCenterAsync(costCenter);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(costCenter.CostCenterId, criteria.CostCenterId);
            Assert.Equal(costCenter.Code, criteria.Code);
            Assert.Equal(costCenter.Name, criteria.Name);
            Assert.Equal(costCenter.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Cost Center

        #region Section

        [Fact]
        public async Task GetAllSectionAsync_BaseServiceGetSectionList_ReturnSectionList()
        {
            // Arrange
            var expected = new List<Section> { MockSection() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Section>(Arg.Any<GetAllSectionCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllSectionAsync() as List<Section>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateSectionAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var section = MockSection();
            var criteria = new CreateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSectionCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateSectionAsync(section);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(section.Code, criteria.Code);
            Assert.Equal(section.Name, criteria.Name);
            Assert.Equal(section.CostCenterId, criteria.CostCenterId);
            Assert.Equal(section.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetSectionByIdAsync_BaseServiceGetSectionInstance_ReturnSectionInstance()
        {
            // Arrange
            var id = 41;
            var section = MockSection();
            var criteria = new GetSectionByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Section>(Arg.Do<GetSectionByIdCriteria>(x => criteria = x)).Returns(section);

            // Act
            var actual = await dataConfigurationService.GetSectionByIdAsync(id);

            // Assert
            Assert.IsType<Section>(actual);
            Assert.Equal(section.SectionId, actual.SectionId);
            Assert.Equal(section.Code, actual.Code);
            Assert.Equal(section.Name, actual.Name);
            Assert.Equal(section.IsActive, actual.IsActive);
            Assert.Equal(section.CompanyId, actual.CompanyId);
            Assert.Equal(section.CompanyCode, actual.CompanyCode);
            Assert.Equal(section.DivisionId, actual.DivisionId);
            Assert.Equal(section.DivisionCode, actual.DivisionCode);
            Assert.Equal(section.DepartmentId, actual.DepartmentId);
            Assert.Equal(section.DepartmentCode, actual.DepartmentCode);
            Assert.Equal(section.CostCenterId, actual.CostCenterId);
            Assert.Equal(section.CostCenterCode, actual.CostCenterCode);
            Assert.Equal(id, criteria.SectionId);
        }

        [Fact]
        public async Task GetSectionByIdAsync_BaseServiceGetNullSection_ReturnNull()
        {
            // Arrange
            Section section = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Section>(Arg.Any<GetSectionByIdCriteria>()).Returns(section);

            // Act
            var actual = await dataConfigurationService.GetSectionByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateSectionAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var section = MockSection();
            var criteria = new UpdateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateSectionCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateSectionAsync(section);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(section.SectionId, criteria.SectionId);
            Assert.Equal(section.Code, criteria.Code);
            Assert.Equal(section.Name, criteria.Name);
            Assert.Equal(section.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Section

        #region Job Category

        [Fact]
        public async Task GetAllJobCategoryAsync_BaseServiceGetJobCategoryList_ReturnSimpleItemList()
        {
            // Arrange
            var jobCategories = new List<JobCategory> { MockJobCategory() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<JobCategory>(Arg.Any<GetAllJobCategoryCriteria>()).Returns(jobCategories);
            // Act
            var actual = (await dataConfigurationService.GetAllJobCategoryAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(jobCategories.Count, actual.Count);
            Assert.Equal(jobCategories[0].JobCategoryId, actual[0].ItemId);
            Assert.Equal(jobCategories[0].Name, actual[0].ItemName);
            Assert.Equal(jobCategories[0].Description, actual[0].Description);
            Assert.Equal(jobCategories[0].IsActive, actual[0].IsActive);
        }

        [Fact]
        public async Task GetAllJobCategoryAsync_BaseServiceGetEmptyJobCategoryList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var jobCategories = new List<JobCategory>();

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<JobCategory>(Arg.Any<GetAllJobCategoryCriteria>()).Returns(jobCategories);
            // Act
            var actual = (await dataConfigurationService.GetAllJobCategoryAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(jobCategories.Count, actual.Count);
        }

        [Fact]
        public async Task CreateJobCategoryAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new CreateJobCategoryCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateJobCategoryCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateJobCategoryAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemName, criteria.Name);
            Assert.Equal(item.Description, criteria.Description);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetJobCategoryByIdAsync_BaseServiceGetJobCategoryInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var id = 12;
            var jobCategory = MockJobCategory();
            var criteria = new GetJobCategoryByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<JobCategory>(Arg.Do<GetJobCategoryByIdCriteria>(x => criteria = x)).Returns(jobCategory);

            // Act
            var actual = await dataConfigurationService.GetJobCategoryByIdAsync(id);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(jobCategory.JobCategoryId, actual.ItemId);
            Assert.Equal(jobCategory.Name, actual.ItemName);
            Assert.Equal(jobCategory.Description, actual.Description);
            Assert.Equal(jobCategory.IsActive, actual.IsActive);
            Assert.Equal(id, criteria.JobCategoryId);
        }

        [Fact]
        public async Task GetJobCategoryByIdAsync_BaseServiceGetNullJobCategory_ReturnNull()
        {
            // Arrange
            JobCategory jobCategory = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<JobCategory>(Arg.Any<GetJobCategoryByIdCriteria>()).Returns(jobCategory);

            // Act
            var actual = await dataConfigurationService.GetJobCategoryByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateJobCategoryAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new UpdateJobCategoryCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateJobCategoryCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateJobCategoryAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemId, criteria.JobCategoryId);
            Assert.Equal(item.ItemName, criteria.Name);
            Assert.Equal(item.Description, criteria.Description);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Job Category

        #region Grade Level

        [Fact]
        public async Task GetAllGradeLevelAsync_BaseServiceGetGradeLevelList_ReturnGradeLevelList()
        {
            // Arrange
            var expected = new List<GradeLevel> { MockGradeLevel() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<GradeLevel>(Arg.Any<GetAllGradeLevelCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllGradeLevelAsync() as List<GradeLevel>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateGradeLevelAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var criteria = new CreateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateGradeLevelCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateGradeLevelAsync(gradeLevel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(gradeLevel.Name, criteria.Name);
            Assert.Equal(gradeLevel.Description, criteria.Description);
            Assert.Equal(gradeLevel.JobCategoryId, criteria.JobCategoryId);
            Assert.Equal(gradeLevel.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetGradeLevelByIdAsync_BaseServiceGetGradeLevelInstance_ReturnGradeLevelInstance()
        {
            // Arrange
            var id = 51;
            var gradeLevel = MockGradeLevel();
            var criteria = new GetGradeLevelByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<GradeLevel>(Arg.Do<GetGradeLevelByIdCriteria>(x => criteria = x)).Returns(gradeLevel);

            // Act
            var actual = await dataConfigurationService.GetGradeLevelByIdAsync(id);

            // Assert
            Assert.IsType<GradeLevel>(actual);
            Assert.Equal(gradeLevel.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(gradeLevel.Name, actual.Name);
            Assert.Equal(gradeLevel.Description, actual.Description);
            Assert.Equal(gradeLevel.IsActive, actual.IsActive);
            Assert.Equal(gradeLevel.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(gradeLevel.JobCategoryName, actual.JobCategoryName);
            Assert.Equal(id, criteria.GradeLevelId);
        }

        [Fact]
        public async Task GetGradeLevelByIdAsync_BaseServiceGetNullGradeLevel_ReturnNull()
        {
            // Arrange
            GradeLevel gradeLevel = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<GradeLevel>(Arg.Any<GetGradeLevelByIdCriteria>()).Returns(gradeLevel);

            // Act
            var actual = await dataConfigurationService.GetGradeLevelByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateGradeLevelAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var criteria = new UpdateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateGradeLevelCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateGradeLevelAsync(gradeLevel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(gradeLevel.GradeLevelId, criteria.GradeLevelId);
            Assert.Equal(gradeLevel.Name, criteria.Name);
            Assert.Equal(gradeLevel.Description, criteria.Description);
            Assert.Equal(gradeLevel.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Grade Level

        #region Job Grade

        [Fact]
        public async Task GetAllJobGradelAsync_BaseServiceGetJobGradeList_ReturnJobGradeList()
        {
            // Arrange
            var expected = new List<JobGrade> { MockJobGrade() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<JobGrade>(Arg.Any<GetAllJobGradeCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllJobGradelAsync() as List<JobGrade>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateJobGradeAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var jobGrade = MockJobGrade();
            var criteria = new CreateJobGradeCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateJobGradeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateJobGradeAsync(jobGrade);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(jobGrade.Name, criteria.Name);
            Assert.Equal(jobGrade.Description, criteria.Description);
            Assert.Equal(jobGrade.GradeLevelId, criteria.GradeLevelId);
            Assert.Equal(jobGrade.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetJobGradeByIdAsync_BaseServiceGetJobGradeInstance_ReturnJobGradeInstance()
        {
            // Arrange
            var id = 61;
            var jobGrade = MockJobGrade();
            var criteria = new GetJobGradeByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<JobGrade>(Arg.Do<GetJobGradeByIdCriteria>(x => criteria = x)).Returns(jobGrade);

            // Act
            var actual = await dataConfigurationService.GetJobGradeByIdAsync(id);

            // Assert
            Assert.IsType<JobGrade>(actual);
            Assert.Equal(jobGrade, actual);
            Assert.Equal(id, criteria.JobGradeId);
        }

        [Fact]
        public async Task GetJobGradeByIdAsync_BaseServiceGetNullJobGrade_ReturnNull()
        {
            // Arrange
            JobGrade jobGrade = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<JobGrade>(Arg.Any<GetJobGradeByIdCriteria>()).Returns(jobGrade);

            // Act
            var actual = await dataConfigurationService.GetGradeLevelByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateJobGradeAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var jobGrade = MockJobGrade();
            var criteria = new UpdateJobGradeCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateJobGradeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateJobGradeAsync(jobGrade);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(jobGrade.JobGradeId, criteria.JobGradeId);
            Assert.Equal(jobGrade.Name, criteria.Name);
            Assert.Equal(jobGrade.Description, criteria.Description);
            Assert.Equal(jobGrade.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Job Grade

        #region Country

        [Fact]
        public async Task GetAllCountryAsync_BaseServiceGetCountryList_ReturnSimpleItemList()
        {
            // Arrange
            var countries = new List<Country> { MockCountry() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Country>(Arg.Any<GetAllCountryCriteria>()).Returns(countries);

            // Act
            var actual = (await dataConfigurationService.GetAllCountryAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(countries.Count, actual.Count);
            Assert.Equal(countries[0].CountryId, actual[0].ItemId);
            Assert.Equal(countries[0].Code, actual[0].ItemName);
            Assert.Equal(countries[0].Name, actual[0].Description);
            Assert.Equal(countries[0].IsActive, actual[0].IsActive);
        }

        [Fact]
        public async Task GetAllCountryAsync_BaseServiceGetEmptyCountryList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var countries = new List<Country>();

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Country>(Arg.Any<GetAllCountryCriteria>()).Returns(countries);
            // Act
            var actual = (await dataConfigurationService.GetAllCountryAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(countries.Count, actual.Count);
        }

        [Fact]
        public async Task CreateCountryAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new CreateCountryCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCountryCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateCountryAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetCountryByIdAsync_BaseServiceGetCountryInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var id = 14;
            var country = MockCountry();
            var criteria = new GetCountryByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Country>(Arg.Do<GetCountryByIdCriteria>(x => criteria = x)).Returns(country);

            // Act
            var actual = await dataConfigurationService.GetCountryByIdAsync(id);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(country.CountryId, actual.ItemId);
            Assert.Equal(country.Code, actual.ItemName);
            Assert.Equal(country.Name, actual.Description);
            Assert.Equal(country.IsActive, actual.IsActive);
            Assert.Equal(id, criteria.CountryId);
        }

        [Fact]
        public async Task GetCountryByIdAsync_BaseServiceGetNullCountry_ReturnNull()
        {
            // Arrange
            Country country = null;
            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<Country>(Arg.Any<GetCountryByIdCriteria>()).Returns(country);

            // Act
            var actual = await dataConfigurationService.GetCountryByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCountryAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new UpdateCountryCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCountryCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateCountryAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemId, criteria.CountryId);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Country

        #region City

        [Fact]
        public async Task GetAllCityAsync_BaseServiceGetCityList_ReturnCityList()
        {
            // Arrange
            var expected = new List<City> { MockCity() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<City>(Arg.Any<GetAllCityCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllCityAsync() as List<City>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateCityAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var city = MockCity();
            var criteria = new CreateCityCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCityCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateCityAsync(city);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(city.Code, criteria.Code);
            Assert.Equal(city.Name, criteria.Name);
            Assert.Equal(city.CountryId, criteria.CountryId);
            Assert.Equal(city.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetCityByIdAsync_BaseServiceGetCityInstance_ReturnCityInstance()
        {
            // Arrange
            var id = 22;
            var criteria = new GetCityByIdCriteria();
            var city = MockCity();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<City>(Arg.Do<GetCityByIdCriteria>(x => criteria = x)).Returns(city);

            // Act
            var actual = await dataConfigurationService.GetCityByIdAsync(id);

            // Assert
            Assert.IsType<City>(actual);
            Assert.Equal(city, actual);
            Assert.Equal(id, criteria.CityId);
        }

        [Fact]
        public async Task GetCityByIdAsync_BaseServiceGetNullCity_ReturnNull()
        {
            // Arrange
            City city = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<City>(Arg.Any<GetCityByIdCriteria>()).Returns(city);

            // Act
            var actual = await dataConfigurationService.GetCityByIdAsync(21);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCityAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var city = MockCity();
            var criteria = new UpdateCityCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCityCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateCityAsync(city);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(city.CityId, criteria.CityId);
            Assert.Equal(city.Code, criteria.Code);
            Assert.Equal(city.Name, criteria.Name);
            Assert.Equal(city.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion City

        #region Office

        [Fact]
        public async Task GetAllOfficeAsync_BaseServiceGetOfficeList_ReturnOfficeList()
        {
            // Arrange
            var expected = new List<Office> { MockOffice() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Office>(Arg.Any<GetAllOfficeCriteria>()).Returns(expected);

            // Act
            var actual = await dataConfigurationService.GetAllOfficeAsync() as List<Office>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CreateOfficeAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var office = MockOffice();
            var criteria = new CreateOfficeCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateOfficeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateOfficeAsync(office);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(office.Code, criteria.Code);
            Assert.Equal(office.Name, criteria.Name);
            Assert.Equal(office.CityId, criteria.CityId);
            Assert.Equal(office.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetOfficeByIdAsync_BaseServiceGetOfficeInstance_ReturnOfficeInstance()
        {
            // Arrange
            var id = 23;
            var criteria = new GetOfficeByIdCriteria();
            var office = MockOffice();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Office>(Arg.Do<GetOfficeByIdCriteria>(x => criteria = x)).Returns(office);

            // Act
            var actual = await dataConfigurationService.GetOfficeByIdAsync(id);

            // Assert
            Assert.IsType<Office>(actual);
            Assert.Equal(office, actual);
            Assert.Equal(id, criteria.OfficeId);
        }

        [Fact]
        public async Task GetOfficeByIdAsync_BaseServiceGetNullOffice_ReturnNull()
        {
            // Arrange
            Office office = null;
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Office>(Arg.Any<GetOfficeByIdCriteria>()).Returns(office);

            // Act
            var actual = await dataConfigurationService.GetOfficeByIdAsync(21);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateOfficeAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var office = MockOffice();
            var criteria = new UpdateOfficeCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateOfficeCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateOfficeAsync(office);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(office.OfficeId, criteria.OfficeId);
            Assert.Equal(office.Code, criteria.Code);
            Assert.Equal(office.Name, criteria.Name);
            Assert.Equal(office.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Office

        #region RefResource Item

        [Fact]
        public async Task GetAllRefResourceItemAsync_BaseServiceGetReferenceResourceModelList_ReturnSimpleItemList()
        {
            // Arrange
            var refName = "xxxzzz";
            var items = new List<ReferenceResourceModel> { MockReferenceResourceModel() };
            var criteria = new GetRefResourceByNamesCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Do<GetRefResourceByNamesCriteria>(x => criteria = x)).Returns(items);

            // Act
            var actual = (await dataConfigurationService.GetAllRefResourceItemAsync(refName)) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(items.Count, actual.Count);
            Assert.Equal(items[0].ResourceId, actual[0].ItemId);
            Assert.Equal(items[0].Value, actual[0].ItemName);
            Assert.Equal(items[0].Description, actual[0].Description);
            Assert.Equal(items[0].IsActive, actual[0].IsActive);
            Assert.Equal(refName, criteria.RefNames);
        }

        [Fact]
        public async Task GetAllRefResourceItemAsync_BaseServiceGetEmptyReferenceResourceModelList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var items = new List<ReferenceResourceModel>();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(items);

            // Act
            var actual = (await dataConfigurationService.GetAllRefResourceItemAsync("aaa")) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(items.Count, actual.Count);
        }

        [Fact]
        public async Task CreateRefResourceItemAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var refName = "xxxzzz";
            var item = MockSimpleItem();
            var criteria = new CreateRefResourceCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateRefResourceCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateRefResourceItemAsync(refName, item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemName, criteria.Value);
            Assert.Equal(item.Description, criteria.Description);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
            Assert.Equal(refName, criteria.RefName);
        }

        [Fact]
        public async Task GetRefResourceByNameAndIdAsync_BaseServiceGetReferenceResourceModelInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var id = 19;
            var refName = "xxxzzz";
            var item = MockReferenceResourceModel();
            var criteria = new GetRefResourceByNameAndIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<ReferenceResourceModel>(Arg.Do<GetRefResourceByNameAndIdCriteria>(x => criteria = x)).Returns(item);

            // Act
            var actual = await dataConfigurationService.GetRefResourceByNameAndIdAsync(refName, id);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(item.ResourceId, actual.ItemId);
            Assert.Equal(item.Value, actual.ItemName);
            Assert.Equal(item.Description, actual.Description);
            Assert.Equal(item.IsActive, actual.IsActive);
            Assert.Equal(id, criteria.ResourceId);
            Assert.Equal(refName, criteria.RefName);
        }

        [Fact]
        public async Task GetRefResourceByNameAndIdAsync_BaseServiceGetNullReferenceResourceModel_ReturnNull()
        {
            // Arrange
            ReferenceResourceModel refModel = null;
            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNameAndIdCriteria>()).Returns(refModel);

            // Act
            var actual = await dataConfigurationService.GetRefResourceByNameAndIdAsync("cca", 11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateRefResourceItemAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var refName = "xxxzzz";
            var item = MockSimpleItem();
            var criteria = new UpdateRefResourceItemCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateRefResourceItemCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateRefResourceItemAsync(refName, item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemId, criteria.ResourceId);
            Assert.Equal(item.ItemName, criteria.Value);
            Assert.Equal(item.Description, criteria.Description);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion RefResource Item

        #region Currency

        [Fact]
        public async Task GetAllCurrencyAsync_BaseServiceGetCurrencyList_ReturnSimpleItemList()
        {
            // Arrange
            var currencies = new List<Currency> { MockCurrency() };

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Currency>(Arg.Any<GetAllCurrencyCriteria>()).Returns(currencies);

            // Act
            var actual = (await dataConfigurationService.GetAllCurrencyAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(currencies.Count, actual.Count);
            Assert.Equal(currencies[0].CurrencyId, actual[0].ItemId);
            Assert.Equal(currencies[0].CurrencyCode, actual[0].ItemName);
            Assert.Equal(currencies[0].CurrencyName, actual[0].Description);
            Assert.Equal(currencies[0].IsActive, actual[0].IsActive);
        }

        [Fact]
        public async Task GetAllCurrencyAsync_BaseServiceGetEmptyCurrencyList_ReturnEmptySimpleItemList()
        {
            // Arrange
            var currencies = new List<Currency>();

            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Currency>(Arg.Any<GetAllCurrencyCriteria>()).Returns(currencies);
            // Act
            var actual = (await dataConfigurationService.GetAllCurrencyAsync()) as List<SimpleItem>;

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Equal(currencies.Count, actual.Count);
        }

        [Fact]
        public async Task CreateCurrencyAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new CreateCurrencyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCurrencyCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.CreateCurrencyAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemName, criteria.CurrencyCode);
            Assert.Equal(item.Description, criteria.CurrencyName);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task GetCurrencyByIdAsync_BaseServiceGetCurrencyInstance_ReturnSimpleItemInstance()
        {
            // Arrange
            var id = 12;
            var currency = MockCurrency();
            var criteria = new GetCurrencyByIdCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataEntityAsync<Currency>(Arg.Do<GetCurrencyByIdCriteria>(x => criteria = x)).Returns(currency);

            // Act
            var actual = await dataConfigurationService.GetCurrencyByIdAsync(id);

            // Assert
            Assert.IsType<SimpleItem>(actual);
            Assert.Equal(currency.CurrencyId, actual.ItemId);
            Assert.Equal(currency.CurrencyCode, actual.ItemName);
            Assert.Equal(currency.CurrencyName, actual.Description);
            Assert.Equal(currency.IsActive, actual.IsActive);
            Assert.Equal(id, criteria.CurrencyId);
        }

        [Fact]
        public async Task GetCurrencyByIdAsync_BaseServiceGetNullCurrency_ReturnNull()
        {
            // Arrange
            Currency currency = null;
            var dataConfigurationService = GetDataConfigurationService();

            baseService.GetDataEntityAsync<Currency>(Arg.Any<GetCurrencyByIdCriteria>()).Returns(currency);

            // Act
            var actual = await dataConfigurationService.GetCurrencyByIdAsync(11);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task UpdateCurrencyAsync_BaseServiceExecuteNonQuery_ReturnCommandResult()
        {
            // Arrange
            var expected = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var criteria = new UpdateCurrencyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCurrencyCriteria>(x => criteria = x)).Returns(expected);

            // Act
            var actual = await dataConfigurationService.UpdateCurrencyAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.Equal(expected, actual);
            Assert.Equal(item.ItemId, criteria.CurrencyId);
            Assert.Equal(item.ItemName, criteria.CurrencyCode);
            Assert.Equal(item.Description, criteria.CurrencyName);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        #endregion Currency

        #region Common

        [Fact]
        public async Task GetDivisionSelectListByCompanyIdAsync_BaseServiceGetDivisionList_ReturnSelectList()
        {
            // Arrange
            var id = 10;
            var divisions = new List<Division> { MockDivision() };
            var criteria = new GetDivisionByCompanyIdCriteria();
            var expected = new SelectList(divisions, nameof(Division.DivisionId), nameof(Division.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Division>(Arg.Do<GetDivisionByCompanyIdCriteria>(x => criteria = x)).Returns(divisions);

            // Act
            var actual = await dataConfigurationService.GetDivisionSelectListByCompanyIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.CompanyId);
        }

        [Fact]
        public async Task GetDepartmentSelectListByDivisionIdAsync_BaseServiceGetDepartmentList_ReturnSelectList()
        {
            // Arrange
            var id = 11;
            var departments = new List<Department> { MockDepartment() };
            var criteria = new GetDepartmentByDivisionIdCriteria();
            var expected = new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Department>(Arg.Do<GetDepartmentByDivisionIdCriteria>(x => criteria = x)).Returns(departments);

            // Act
            var actual = await dataConfigurationService.GetDepartmentSelectListByDivisionIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.DivisionId);
        }

        [Fact]
        public async Task GetCostCenterSelectListByDepartmentIdAsync_BaseServiceGetCostCenterList_ReturnSelectList()
        {
            // Arrange
            var id = 12;
            var costCenters = new List<CostCenter> { MockCostCenter() };
            var criteria = new GetCostCenterDepartmentIdCriteria();
            var expected = new SelectList(costCenters, nameof(CostCenter.CostCenterId), nameof(CostCenter.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<CostCenter>(Arg.Do<GetCostCenterDepartmentIdCriteria>(x => criteria = x)).Returns(costCenters);

            // Act
            var actual = await dataConfigurationService.GetCostCenterSelectListByDepartmentIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.DepartmentId);
        }

        [Fact]
        public async Task GetSectionSelectListByCostCenterIdAsync_BaseServiceGetSectionList_ReturnSelectList()
        {
            // Arrange
            var id = 13;
            var sections = new List<Section> { MockSection() };
            var criteria = new GetSectionByCostCenterIdCriteria();
            var expected = new SelectList(sections, nameof(Section.SectionId), nameof(CostCenter.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Section>(Arg.Do<GetSectionByCostCenterIdCriteria>(x => criteria = x)).Returns(sections);

            // Act
            var actual = await dataConfigurationService.GetSectionSelectListByCostCenterIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.CostCenterId);
        }

        [Fact]
        public async Task GetCompanyBVISelectListAsync_BaseServiceGetCompanyList_ReturnSelectList()
        {
            // Arrange
            var companies = new List<Company> { MockCompany() };
            var expected = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);

            // Act
            var actual = await dataConfigurationService.GetCompanyBVISelectListAsync();

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
        }

        [Fact]
        public async Task GetJobCategorySelectListAsync_BaseServiceGetJobCategoryList_ReturnSelectList()
        {
            // Arrange
            var jobCategoryList = new List<JobCategory> { MockJobCategory() };
            var expected = new SelectList(jobCategoryList, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<JobCategory>(Arg.Any<GetAllJobCategoryCriteria>()).Returns(jobCategoryList);

            // Act
            var actual = await dataConfigurationService.GetJobCategorySelectListAsync();

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
        }

        [Fact]
        public async Task GetGradeLevelSelectListByJobCateogryIdAsync_BaseServiceGetGradeLevelList_ReturnSelectList()
        {
            // Arrange
            var id = 14;
            var gradeLevelList = new List<GradeLevel> { MockGradeLevel() };
            var criteria = new GetGradeLevelByJobCategoryIdCriteria();
            var expected = new SelectList(gradeLevelList, nameof(GradeLevel.GradeLevelId), nameof(GradeLevel.Name));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<GradeLevel>(Arg.Do<GetGradeLevelByJobCategoryIdCriteria>(x => criteria = x)).Returns(gradeLevelList);

            // Act
            var actual = await dataConfigurationService.GetGradeLevelSelectListByJobCateogryIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.JobCategoryId);
        }

        [Fact]
        public async Task GetCitySelectListByCountryIdAsync_BaseServiceGetCityList_ReturnSelectList()
        {
            // Arrange
            var id = 15;
            var cityList = new List<City> { MockCity() };
            var criteria = new GetCityByCountryIdCriteria();
            var expected = new SelectList(cityList, nameof(City.CityId), nameof(City.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<City>(Arg.Do<GetCityByCountryIdCriteria>(x => criteria = x)).Returns(cityList);

            // Act
            var actual = await dataConfigurationService.GetCitySelectListByCountryIdAsync(id);

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
            Assert.Equal(id, criteria.CountryId);
        }

        [Fact]
        public async Task GetCountrySelectListAsync_BaseServiceGetCountryList_ReturnSelectList()
        {
            // Arrange
            var countries = new List<Country> { MockCountry() };
            var expected = new SelectList(countries, nameof(Country.CountryId), nameof(Country.Code));
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<Country>(Arg.Any<GetAllCountryCriteria>()).Returns(countries);

            // Act
            var actual = await dataConfigurationService.GetCountrySelectListAsync();

            // Assert
            Assert.IsType<SelectList>(actual);
            Assert.Equal(expected.DataValueField, actual.DataValueField);
            Assert.Equal(expected.DataTextField, actual.DataTextField);
            Assert.Equal(expected.Items, actual.Items);
        }

        #endregion Common

        #region Initialization

        private IDataConfigurationService GetDataConfigurationService()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var dataConfigurationService = new DataConfigurationService(baseService, sessionService);

            return dataConfigurationService;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ISessionService FakeSessionService()
        {
            var sessionServiceInstance = Substitute.For<ISessionService>();
            sessionServiceInstance.UserId = 99;

            return sessionServiceInstance;
        }

        private SimpleItem MockSimpleItem()
        {
            return new SimpleItem
            {
                ItemId = 22,
                ItemName = "simple",
                Description = "desc1",
                IsActive = true
            };
        }

        private Company MockCompany()
        {
            return new Company
            {
                CompanyId = 11,
                Code = "company",
                Name = "descC",
                IsActive = true
            };
        }

        private Division MockDivision()
        {
            return new Division
            {
                DivisionId = 9,
                Code = "div",
                Name = "descd",
                IsActive = false,
                CompanyId = 1,
                CompanyCode = "OW"
            };
        }

        private Department MockDepartment()
        {
            return new Department
            {
                DepartmentId = 10,
                Code = "depart",
                Name = "descd",
                IsActive = true,
                CompanyId = 2,
                CompanyCode = "nex",
                DivisionId = 1,
                DivisionCode = "divc"
            };
        }

        private CostCenter MockCostCenter()
        {
            return new CostCenter
            {
                CostCenterId = 11,
                Code = "costcenter",
                Name = "desccc",
                IsActive = true,
                CompanyId = 3,
                CompanyCode = "nex1",
                DivisionId = 2,
                DivisionCode = "divc11",
                DepartmentId = 1,
                DepartmentCode = "Sm"
            };
        }

        private Section MockSection()
        {
            return new Section
            {
                SectionId = 11,
                Code = "section",
                Name = "descs",
                IsActive = true,
                CompanyId = 2,
                CompanyCode = "nex1",
                DivisionId = 3,
                DivisionCode = "divc11",
                DepartmentId = 4,
                DepartmentCode = "Sm",
                CostCenterId = 5,
                CostCenterCode = "cost"
            };
        }

        private JobCategory MockJobCategory()
        {
            return new JobCategory
            {
                JobCategoryId = 22,
                Name = "JobCategory",
                Description = "descs11",
                IsActive = true
            };
        }

        private GradeLevel MockGradeLevel()
        {
            return new GradeLevel
            {
                GradeLevelId = 13,
                Name = "GradeLevel",
                Description = "descs",
                IsActive = true,
                JobCategoryId = 2,
                JobCategoryName = "jobname"
            };
        }

        private JobGrade MockJobGrade()
        {
            return new JobGrade
            {
                JobGradeId = 14,
                Name = "JobGrade",
                Description = "descjg",
                IsActive = true,
                JobCategoryId = 2,
                JobCategoryName = "jobname",
                GradeLevelId = 3,
                GradeLevelName = "gradele"
            };
        }

        private Country MockCountry()
        {
            return new Country
            {
                CountryId = 99,
                Code = "USA",
                Name = "America",
                IsActive = true,
            };
        }

        private City MockCity()
        {
            return new City
            {
                CityId = 15,
                Code = "HCM",
                Name = "Ho Chi Minh",
                IsActive = true,
                CountryId = 3,
                CountryCode = "VN"
            };
        }

        private Office MockOffice()
        {
            return new Office
            {
                OfficeId = 16,
                Code = "Nex",
                Name = "nexcelsolution",
                IsActive = true,
                CountryId = 3,
                CountryCode = "VN",
                CityId = 2,
                CityCode = "hcmmm"
            };
        }

        private ReferenceResourceModel MockReferenceResourceModel()
        {
            return new ReferenceResourceModel
            {
                ResourceId = 80,
                RefName = "ahihi",
                Value = "val",
                Description = "desne",
                IsActive = true
            };
        }

        private Currency MockCurrency()
        {
            return new Currency
            {
                CurrencyId = 12,
                CurrencyCode = "usdt",
                CurrencyName = "descC",
                IsActive = true
            };
        }

        private CommandResult MockSuccessfulCommandResult()
        {
            return new CommandResult
            {
                Code = 0,
                Message = "Successful!!!"
            };
        }

        #endregion Initialization
    }
}