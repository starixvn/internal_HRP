namespace HRP.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.CostCenter;
    using HRP.Criteria.Department;
    using HRP.Criteria.GradeLevel;
    using HRP.Criteria.Section;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NSubstitute;
    using Xunit;

    public class DataConfigurationServiceTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

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
        public async Task IsExistedCompanyBVIAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var item = MockSimpleItem();
            var criteria = new CheckCompanyExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckCompanyExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedCompanyBVIAsync(item);

            // Assert
            Assert.True(actual);
            Assert.Equal(item.ItemId, criteria.CompanyId);
            Assert.Equal(item.ItemName, criteria.Code);
        }

        [Fact]
        public async Task IsExistedCompanyBVIAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckCompanyExistsCriteria>()).Returns(new List<int>());

            // Act
            var actual = await dataConfigurationService.IsExistedCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public async Task CreateCompanyBVIAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var item = MockSimpleItem();
            var criteria = new CreateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateCompanyBVIAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateCompanyBVIAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateCompanyBVIAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var item = MockSimpleItem();
            var criteria = new UpdateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateCompanyBVIAsync(item);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(item.ItemId, criteria.CompanyId);
            Assert.Equal(item.ItemName, criteria.Code);
            Assert.Equal(item.Description, criteria.Name);
            Assert.Equal(item.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateCompanyBVIAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateCompanyCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCompanyCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateCompanyBVIAsync(new SimpleItem());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task IsExistedDivisionAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var division = MockDivision();
            var criteria = new CheckDivisionExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckDivisionExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedDivisionAsync(division);

            Assert.True(actual);
            Assert.Equal(division.DivisionId, criteria.DivisionId);
            Assert.Equal(division.Code, criteria.Code);
            Assert.Equal(division.CompanyId, criteria.CompanyId);
        }

        [Fact]
        public async Task IsExistedDivisionAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var listInteger = new List<int>();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckDivisionExistsCriteria>()).Returns(listInteger);

            // Act
            var actual = await dataConfigurationService.IsExistedDivisionAsync(new Division());

            Assert.False(actual);
        }

        [Fact]
        public async Task CreateDivisionAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var division = MockDivision();
            var criteria = new CreateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDivisionCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateDivisionAsync(division);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
            Assert.Equal(division.Code, criteria.Code);
            Assert.Equal(division.Name, criteria.Name);
            Assert.Equal(division.CompanyId, criteria.CompanyId);
            Assert.Equal(division.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateDivisionAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDivisionCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateDivisionAsync(new Division());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateDivisionAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var division = MockDivision();
            var criteria = new UpdateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDivisionCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateDivisionAsync(division);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(division.DivisionId, criteria.DivisionId);
            Assert.Equal(division.Code, criteria.Code);
            Assert.Equal(division.Name, criteria.Name);
            Assert.Equal(division.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateDivisionAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateDivisionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDivisionCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateDivisionAsync(new Division());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task IsExistedDepartmentAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var department = MockDepartment();
            var criteria = new CheckDepartmentExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckDepartmentExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedDepartmentAsync(department);

            Assert.True(actual);
            Assert.Equal(department.DepartmentId, criteria.DepartmentId);
            Assert.Equal(department.Code, criteria.Code);
            Assert.Equal(department.DivisionId, criteria.DivisionId);
        }

        [Fact]
        public async Task IsExistedDepartmentAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckDepartmentExistsCriteria>()).Returns(new List<int>());

            // Act
            var actual = await dataConfigurationService.IsExistedDepartmentAsync(new Department());

            Assert.False(actual);
        }

        [Fact]
        public async Task CreateDepartmentAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var department = MockDepartment();
            var criteria = new CreateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDepartmentCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateDepartmentAsync(department);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
            Assert.Equal(department.Code, criteria.Code);
            Assert.Equal(department.Name, criteria.Name);
            Assert.Equal(department.DivisionId, criteria.DivisionId);
            Assert.Equal(department.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateDepartmentAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateDepartmentCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateDepartmentAsync(new Department());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateDepartmentAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var department = MockDepartment();
            var criteria = new UpdateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDepartmentCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateDepartmentAsync(department);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(department.DepartmentId, criteria.DepartmentId);
            Assert.Equal(department.Code, criteria.Code);
            Assert.Equal(department.Name, criteria.Name);
            Assert.Equal(department.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateDepartmentAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateDepartmentCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateDepartmentCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateDepartmentAsync(new Department());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task IsExistedCostCenterAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var criteria = new CheckCostCenterExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckCostCenterExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedCostCenterAsync(costCenter);

            Assert.True(actual);
            Assert.Equal(costCenter.DepartmentId, criteria.DepartmentId);
            Assert.Equal(costCenter.Code, criteria.Code);
            Assert.Equal(costCenter.CostCenterId, criteria.CostCenterId);
        }

        [Fact]
        public async Task IsExistedCostCenterAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckCostCenterExistsCriteria>()).Returns(new List<int>());

            // Act
            var actual = await dataConfigurationService.IsExistedCostCenterAsync(new CostCenter());

            Assert.False(actual);
        }

        [Fact]
        public async Task CreateCostCenterAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var criteria = new CreateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCostCenterCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateCostCenterAsync(costCenter);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
            Assert.Equal(costCenter.Code, criteria.Code);
            Assert.Equal(costCenter.Name, criteria.Name);
            Assert.Equal(costCenter.DepartmentId, criteria.DepartmentId);
            Assert.Equal(costCenter.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateCostCenterAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateCostCenterCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateCostCenterAsync(new CostCenter());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateCostCenterAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var criteria = new UpdateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCostCenterCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateCostCenterAsync(costCenter);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(costCenter.CostCenterId, criteria.CostCenterId);
            Assert.Equal(costCenter.Code, criteria.Code);
            Assert.Equal(costCenter.Name, criteria.Name);
            Assert.Equal(costCenter.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateCostCenterAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateCostCenterCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateCostCenterCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateCostCenterAsync(new CostCenter());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task IsExistedSectionAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var section = MockSection();
            var criteria = new CheckSectionExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckSectionExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedSectionAsync(section);

            Assert.True(actual);
            Assert.Equal(section.SectionId, criteria.SectionId);
            Assert.Equal(section.Code, criteria.Code);
            Assert.Equal(section.CostCenterId, criteria.CostCenterId);
        }

        [Fact]
        public async Task IsExistedSectionAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckSectionExistsCriteria>()).Returns(new List<int>());

            // Act
            var actual = await dataConfigurationService.IsExistedSectionAsync(new Section());

            Assert.False(actual);
        }

        [Fact]
        public async Task CreateSectionAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var section = MockSection();
            var criteria = new CreateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSectionCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateSectionAsync(section);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Code), actual.Message);
            Assert.Equal(section.Code, criteria.Code);
            Assert.Equal(section.Name, criteria.Name);
            Assert.Equal(section.CostCenterId, criteria.CostCenterId);
            Assert.Equal(section.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateSectionAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSectionCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateSectionAsync(new Section());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateSectionAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var section = MockSection();
            var criteria = new UpdateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateSectionCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateSectionAsync(section);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(section.SectionId, criteria.SectionId);
            Assert.Equal(section.Code, criteria.Code);
            Assert.Equal(section.Name, criteria.Name);
            Assert.Equal(section.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateSectionAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateSectionCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateSectionCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateSectionAsync(new Section());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
        }

        #endregion Section

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
        public async Task IsExistedGradeLevelAsync_BaseServiceGetIntegerList_ReturnTrue()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var criteria = new CheckGradeLevelExistsCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Do<CheckGradeLevelExistsCriteria>(x => criteria = x)).Returns(new List<int> { 1 });

            // Act
            var actual = await dataConfigurationService.IsExistedGradeLevelAsync(gradeLevel);

            Assert.True(actual);
            Assert.Equal(gradeLevel.GradeLevelId, criteria.GradeLevelId);
            Assert.Equal(gradeLevel.Name, criteria.Name);
            Assert.Equal(gradeLevel.JobCategoryId, criteria.JobCategoryId);
        }

        [Fact]
        public async Task IsExistedGradeLevelAsync_BaseServiceGetEmptyIntegerList_ReturnFalse()
        {
            // Arrange
            var dataConfigurationService = GetDataConfigurationService();
            baseService.GetDataAsync<int>(Arg.Any<CheckGradeLevelExistsCriteria>()).Returns(new List<int>());

            // Act
            var actual = await dataConfigurationService.IsExistedGradeLevelAsync(new GradeLevel());

            Assert.False(actual);
        }

        [Fact]
        public async Task CreateGradeLevelAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var criteria = new CreateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateGradeLevelCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.CreateGradeLevelAsync(gradeLevel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, criteria.Name), actual.Message);
            Assert.Equal(gradeLevel.Name, criteria.Name);
            Assert.Equal(gradeLevel.Description, criteria.Description);
            Assert.Equal(gradeLevel.JobCategoryId, criteria.JobCategoryId);
            Assert.Equal(gradeLevel.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.CreatedBy);
        }

        [Fact]
        public async Task CreateGradeLevelAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new CreateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateGradeLevelCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.CreateGradeLevelAsync(new GradeLevel());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
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
        public async Task UpdateGradeLevelAsync_BaseServiceExecuteNonQuerySuccessful_ReturnCommandResultSuccessfulWithSuccessfulMessage()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var criteria = new UpdateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateGradeLevelCriteria>(x => criteria = x)).Returns(new CommandResult { Code = 0 });

            // Act
            var actual = await dataConfigurationService.UpdateGradeLevelAsync(gradeLevel);

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.True(actual.IsSuccessful);
            Assert.Equal(Message.ItemUpdatedSuccessful, actual.Message);
            Assert.Equal(gradeLevel.GradeLevelId, criteria.GradeLevelId);
            Assert.Equal(gradeLevel.Name, criteria.Name);
            Assert.Equal(gradeLevel.Description, criteria.Description);
            Assert.Equal(gradeLevel.IsActive, criteria.IsActive);
            Assert.Equal(sessionService.UserId, criteria.UpdatedBy);
        }

        [Fact]
        public async Task UpdateGradeLevelAsync_BaseServiceExecuteNonQueryNotSuccessful_ReturnCommandResultNotSuccessful()
        {
            // Arrange
            var criteria = new UpdateGradeLevelCriteria();
            var dataConfigurationService = GetDataConfigurationService();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateGradeLevelCriteria>(x => criteria = x)).Returns(new CommandResult());

            // Act
            var actual = await dataConfigurationService.UpdateGradeLevelAsync(new GradeLevel());

            // Assert
            Assert.IsType<CommandResult>(actual);
            Assert.False(actual.IsSuccessful);
            Assert.False(criteria.IsValid());
        }

        #endregion Grade Level

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

        #endregion Initialization

    }
}