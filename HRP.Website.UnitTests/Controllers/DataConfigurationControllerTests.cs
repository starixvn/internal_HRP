namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Division;
    using HRP.Models.DataConfiguration.GradeLevel;
    using HRP.Models.DataConfiguration.JobGrade;
    using HRP.Models.DataConfiguration.Section;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class DataConfigurationControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IDataConfigurationService dataConfigurationService;

        [Fact]
        public void Index_ServiceGetFieldList_ReturnViewResultWithFieldList()
        {
            // Arrange
            var fieldList = new List<Field> { MockField() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDataForIndexPage().Returns(fieldList);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IEnumerable<Field>>(viewResult.ViewData.Model);
            Assert.Equal(fieldList, viewModel);
        }

        #region CompanyBVI

        [Fact]
        public async Task GetAllCompanyBVI_ServiceGetSimpleItemList_ReturnPartialViewWithSimpleItemTableViewModel()
        {
            // Arrange
            var items = new List<SimpleItem> { MockSimpleItem() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllCompanyBVIAsync().Returns(items);

            // Act
            var actual = await controller.GetAllCompanyBVI();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SimpleItemTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SimpleItemTable", viewResult.ViewName);
            Assert.Equal(items, viewModel.Items);
            Assert.Equal(ViewLang.CompanyBVI, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCompanyBVI, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCompanyBVI, viewModel.LinkUpdateItem);
        }

        [Fact]
        public void CreateCompanyBVI_WhenCall_ReturnPartialViewWithCreateSimpleItemViewModel()
        {
            // Arrange
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateCompanyBVI();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(ViewLang.CompanyBVI, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCompanyBVI, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCompanyBVIPost_CompanyItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCompanyBVIAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.CreateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task CreateCompanyBVIPost_CompanyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCompanyBVIAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.CreateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCompanyBVI_ServiceGetCompanyItem_ReturnPartialViewWithUpdateSimpleItemViewModel()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVIByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCompanyBVI(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSimpleItem", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal(item, viewModel.Item);
            Assert.Equal(ViewLang.CompanyBVI, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCompanyBVI, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateCompanyBVI_ServiceGetNullCompanyItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            SimpleItem item = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVIByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCompanyBVI(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateCompanyBVIPost_CompanyItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCompanyBVIAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.UpdateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateCompanyBVIPost_CompanyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCompanyBVIAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.UpdateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion CompanyBVI

        #region Division

        [Fact]
        public async Task GetAllDivision_ServiceGetDivisionList_ReturnPartialViewWithDivisionTableViewModel()
        {
            // Arrange
            var divisions = new List<Division> { MockDivision() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllDivisionAsync().Returns(divisions);

            // Act
            var actual = await controller.GetAllDivision();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<DivisionTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_DivisionTable", viewResult.ViewName);
            Assert.Equal(divisions, viewModel.Divisions);
            Assert.Equal(ViewLang.Division, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDivision, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateDivision, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateDivision_WhenCall_ReturnPartialViewWithCreateDivisionViewModel()
        {
            // Arrange
            var companySelectList = MockCompanySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVISelectListAsync().Returns(companySelectList);

            // Act
            var actual = await controller.CreateDivision();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateDivisionViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateDivision", viewResult.ViewName);
            Assert.Equal(companySelectList, viewModel.CompanyList);
            Assert.Equal(ViewLang.Division, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDivision, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateDivisionPost_DivisionItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDivisionAsync(Arg.Any<Division>()).Returns(true);

            // Act
            var actual = await controller.CreateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, division.Code), errors["Division.Code"]);
        }

        [Fact]
        public async Task CreateDivisionPost_DivisionItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDivisionAsync(Arg.Any<Division>()).Returns(false);
            dataConfigurationService.CreateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.CreateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateDivision_ServiceGetDivisionItem_ReturnPartialViewWithUpdateDivisionViewModel()
        {
            // Arrange
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDivisionByIdAsync(Arg.Any<int>()).Returns(division);

            // Act
            var actual = await controller.UpdateDivision(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateDivision", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateDivisionViewModel>(viewResult.ViewData.Model);
            Assert.Equal(division, viewModel.Division);
            Assert.Equal(ViewLang.Division, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateDivision, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateDivision_ServiceGetNullDivisionItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            Division division = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDivisionByIdAsync(Arg.Any<int>()).Returns(division);

            // Act
            var actual = await controller.UpdateDivision(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateDivisionPost_DivisionItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDivisionAsync(Arg.Any<Division>()).Returns(true);

            // Act
            var actual = await controller.UpdateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, division.Code), errors["Division.Code"]);
        }

        [Fact]
        public async Task UpdateDivisionPost_DivisionItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDivisionAsync(Arg.Any<Division>()).Returns(false);
            dataConfigurationService.UpdateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Division

        #region Department

        [Fact]
        public async Task GetAllDepartment_ServiceGetDepartmentList_ReturnPartialViewWithDepartmentTableViewModel()
        {
            // Arrange
            var departments = new List<Department> { MockDepartment() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllDepartmentAsync().Returns(departments);

            // Act
            var actual = await controller.GetAllDepartment();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<DepartmentTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_DepartmentTable", viewResult.ViewName);
            Assert.Equal(departments, viewModel.Departments);
            Assert.Equal(ViewLang.Department, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDepartment, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateDepartment, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateDepartment_WhenCall_ReturnPartialViewWithCreateDepartmentViewModel()
        {
            // Arrange
            var companySelectList = MockCompanySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVISelectListAsync().Returns(companySelectList);

            // Act
            var actual = await controller.CreateDepartment();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateDepartmentViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateDepartment", viewResult.ViewName);
            Assert.Equal(companySelectList, viewModel.CompanyList);
            Assert.Equal(ViewLang.Department, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDepartment, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateDepartmentPost_DepartmentItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDepartmentAsync(Arg.Any<Department>()).Returns(true);

            // Act
            var actual = await controller.CreateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, department.Code), errors["Department.Code"]);
        }

        [Fact]
        public async Task CreateDepartmentPost_DepartmentItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDepartmentAsync(Arg.Any<Department>()).Returns(false);
            dataConfigurationService.CreateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.CreateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateDepartment_ServiceGetDepartmentItem_ReturnPartialViewWithUpdateDepartmentViewModel()
        {
            // Arrange
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDepartmentByIdAsync(Arg.Any<int>()).Returns(department);

            // Act
            var actual = await controller.UpdateDepartment(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateDepartment", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateDepartmentViewModel>(viewResult.ViewData.Model);
            Assert.Equal(department, viewModel.Department);
            Assert.Equal(ViewLang.Department, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateDepartment, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateDepartment_ServiceGetNullDepartmentItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            Department department = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDepartmentByIdAsync(Arg.Any<int>()).Returns(department);

            // Act
            var actual = await controller.UpdateDepartment(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateDepartmentPost_DepartmentItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDepartmentAsync(Arg.Any<Department>()).Returns(true);

            // Act
            var actual = await controller.UpdateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, department.Code), errors["Department.Code"]);
        }

        [Fact]
        public async Task UpdateDepartmentPost_DepartmentItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedDepartmentAsync(Arg.Any<Department>()).Returns(false);
            dataConfigurationService.UpdateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Department

        #region Cost Center

        [Fact]
        public async Task GetAllCostCenter_ServiceGetCostCenterList_ReturnPartialViewWithCostCenterTableViewModel()
        {
            // Arrange
            var costCenters = new List<CostCenter> { MockCostCenter() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllCostCenterAsync().Returns(costCenters);

            // Act
            var actual = await controller.GetAllCostCenter();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CostCenterTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CostCenterTable", viewResult.ViewName);
            Assert.Equal(costCenters, viewModel.CostCenters);
            Assert.Equal(ViewLang.CostCenter, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCostCenter, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCostCenter, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateCostCenter_WhenCall_ReturnPartialViewWithCreateCostCenterViewModel()
        {
            // Arrange
            var companySelectList = MockCompanySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVISelectListAsync().Returns(companySelectList);

            // Act
            var actual = await controller.CreateCostCenter();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateCostCenterViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateCostCenter", viewResult.ViewName);
            Assert.Equal(companySelectList, viewModel.CompanyList);
            Assert.Equal(ViewLang.CostCenter, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCostCenter, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCostCenterPost_CostCenterItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCostCenterAsync(Arg.Any<CostCenter>()).Returns(true);

            // Act
            var actual = await controller.CreateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, costCenter.Code), errors["CostCenter.Code"]);
        }

        [Fact]
        public async Task CreateCostCenterPost_CostCenterItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCostCenterAsync(Arg.Any<CostCenter>()).Returns(false);
            dataConfigurationService.CreateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.CreateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCostCenter_ServiceGetCostCenterItem_ReturnPartialViewWithUpdateCostCenterViewModel()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCostCenterByIdAsync(Arg.Any<int>()).Returns(costCenter);

            // Act
            var actual = await controller.UpdateCostCenter(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateCostCenter", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateCostCenterViewModel>(viewResult.ViewData.Model);
            Assert.Equal(costCenter, viewModel.CostCenter);
            Assert.Equal(ViewLang.CostCenter, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCostCenter, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateCostCenter_ServiceGetNullCostCenterItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            CostCenter costCenter = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCostCenterByIdAsync(Arg.Any<int>()).Returns(costCenter);

            // Act
            var actual = await controller.UpdateCostCenter(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateCostCenterPost_CostCenterItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCostCenterAsync(Arg.Any<CostCenter>()).Returns(true);

            // Act
            var actual = await controller.UpdateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, costCenter.Code), errors["CostCenter.Code"]);
        }

        [Fact]
        public async Task UpdateCostCenterPost_CostCenterItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCostCenterAsync(Arg.Any<CostCenter>()).Returns(false);
            dataConfigurationService.UpdateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Cost Center

        #region Section

        [Fact]
        public async Task GetAllSection_ServiceGetSectionList_ReturnPartialViewWithSectionTableViewModel()
        {
            // Arrange
            var sections = new List<Section> { MockSection() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllSectionAsync().Returns(sections);

            // Act
            var actual = await controller.GetAllSection();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SectionTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SectionTable", viewResult.ViewName);
            Assert.Equal(sections, viewModel.Sections);
            Assert.Equal(ViewLang.Section, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateSection, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateSection, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateSection_WhenCall_ReturnPartialViewWithCreateSectionViewModel()
        {
            // Arrange
            var companySelectList = MockCompanySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCompanyBVISelectListAsync().Returns(companySelectList);

            // Act
            var actual = await controller.CreateSection();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSectionViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSection", viewResult.ViewName);
            Assert.Equal(companySelectList, viewModel.CompanyList);
            Assert.Equal(ViewLang.Section, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateSection, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateSectionPost_SectionItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedSectionAsync(Arg.Any<Section>()).Returns(true);

            // Act
            var actual = await controller.CreateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, section.Code), errors["Section.Code"]);
        }

        [Fact]
        public async Task CreateSectionPost_SectionItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedSectionAsync(Arg.Any<Section>()).Returns(false);
            dataConfigurationService.CreateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.CreateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateSection_ServiceGetSectionItem_ReturnPartialViewWithUpdateSectionViewModel()
        {
            // Arrange
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetSectionByIdAsync(Arg.Any<int>()).Returns(section);

            // Act
            var actual = await controller.UpdateSection(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSection", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSectionViewModel>(viewResult.ViewData.Model);
            Assert.Equal(section, viewModel.Section);
            Assert.Equal(ViewLang.Section, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateSection, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateSection_ServiceGetNullSectionItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            Section section = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetSectionByIdAsync(Arg.Any<int>()).Returns(section);

            // Act
            var actual = await controller.UpdateSection(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateSectionPost_SectionItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedSectionAsync(Arg.Any<Section>()).Returns(true);

            // Act
            var actual = await controller.UpdateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, section.Code), errors["Section.Code"]);
        }

        [Fact]
        public async Task UpdateSectionPost_SectionItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedSectionAsync(Arg.Any<Section>()).Returns(false);
            dataConfigurationService.UpdateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.UpdateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Section

        #region Grade Level

        [Fact]
        public async Task GetAllGradeLevel_ServiceGetGradeLevelList_ReturnPartialViewWithGradeLevelTableViewModel()
        {
            // Arrange
            var gradeLevelList = new List<GradeLevel> { MockGradeLevel() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllGradeLevelAsync().Returns(gradeLevelList);

            // Act
            var actual = await controller.GetAllGradeLevel();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<GradeLevelTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_GradeLevelTable", viewResult.ViewName);
            Assert.Equal(gradeLevelList, viewModel.GradeLevelList);
            Assert.Equal(ViewLang.GradeLevel, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateGradeLevel, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateGradeLevel, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateGradeLevel_WhenCall_ReturnPartialViewWithCreateGradeLevelViewModel()
        {
            // Arrange
            var jobCategorySelectList = MockJobCategorySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobCategorySelectListAsync().Returns(jobCategorySelectList);

            // Act
            var actual = await controller.CreateGradeLevel();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateGradeLevelViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateGradeLevel", viewResult.ViewName);
            Assert.Equal(jobCategorySelectList, viewModel.JobCategoryList);
            Assert.Equal(ViewLang.GradeLevel, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateGradeLevel, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateGradeLevelPost_GradeLevelItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedGradeLevelAsync(Arg.Any<GradeLevel>()).Returns(true);

            // Act
            var actual = await controller.CreateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, gradeLevel.Name), errors["GradeLevel.Name"]);
        }

        [Fact]
        public async Task CreateGradeLevelPost_GradeLevelItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedGradeLevelAsync(Arg.Any<GradeLevel>()).Returns(false);
            dataConfigurationService.CreateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.CreateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateGradeLevel_ServiceGetGradeLevelItem_ReturnPartialViewWithUpdateGradeLevelViewModel()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetGradeLevelByIdAsync(Arg.Any<int>()).Returns(gradeLevel);

            // Act
            var actual = await controller.UpdateGradeLevel(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateGradeLevel", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateGradeLevelViewModel>(viewResult.ViewData.Model);
            Assert.Equal(gradeLevel, viewModel.GradeLevel);
            Assert.Equal(ViewLang.GradeLevel, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateGradeLevel, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateGradeLevel_ServiceGetNullGradeLevelItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            GradeLevel gradeLevel = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetGradeLevelByIdAsync(Arg.Any<int>()).Returns(gradeLevel);

            // Act
            var actual = await controller.UpdateGradeLevel(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateGradeLevelPost_GradeLevelItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedGradeLevelAsync(Arg.Any<GradeLevel>()).Returns(true);

            // Act
            var actual = await controller.UpdateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, gradeLevel.Name), errors["GradeLevel.Name"]);
        }

        [Fact]
        public async Task UpdateGradeLevelPost_GradeLevelItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedGradeLevelAsync(Arg.Any<GradeLevel>()).Returns(false);
            dataConfigurationService.UpdateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.UpdateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Grade Level

        #region Job Grade

        [Fact]
        public async Task GetAllJobGrade_ServiceGetJobGradeList_ReturnPartialViewWithJobGradeTableViewModel()
        {
            // Arrange
            var jobGradeList = new List<JobGrade> { MockJobGrade() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllJobGradelAsync().Returns(jobGradeList);

            // Act
            var actual = await controller.GetAllJobGrade();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<JobGradeTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_JobGradeTable", viewResult.ViewName);
            Assert.Equal(jobGradeList, viewModel.JobGradeList);
            Assert.Equal(ViewLang.JobGrade, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobGrade, viewModel.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateJobGrade, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateJobGrade_WhenCall_ReturnPartialViewWithCreateJobGradeViewModel()
        {
            // Arrange
            var jobCategorySelectList = MockJobCategorySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobCategorySelectListAsync().Returns(jobCategorySelectList);

            // Act
            var actual = await controller.CreateJobGrade();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateJobGradeViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateJobGrade", viewResult.ViewName);
            Assert.Equal(jobCategorySelectList, viewModel.JobCategoryList);
            Assert.Equal(ViewLang.JobGrade, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobGrade, viewModel.LinkCreateItem);
        }

        [Fact]
        public async Task CreateJobGradePost_JobGradeItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var jobGrade = MockJobGrade();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedJobGradeAsync(Arg.Any<JobGrade>()).Returns(true);

            // Act
            var actual = await controller.CreateJobGrade(jobGrade);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, jobGrade.Name), errors["JobGrade.Name"]);
        }

        [Fact]
        public async Task CreateJobGradePost_JobGradeItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var jobGrade = MockJobGrade();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedJobGradeAsync(Arg.Any<JobGrade>()).Returns(false);
            dataConfigurationService.CreateJobGradeAsync(jobGrade).Returns(commandResult);

            // Act
            var actual = await controller.CreateJobGrade(jobGrade);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateJobGrade_ServiceGetJobGradeItem_ReturnPartialViewWithUpdateJobGradeViewModel()
        {
            // Arrange
            var jobGrade = MockJobGrade();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobGradeByIdAsync(Arg.Any<int>()).Returns(jobGrade);

            // Act
            var actual = await controller.UpdateJobGrade(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateJobGrade", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateJobGradeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(jobGrade, viewModel.JobGrade);
            Assert.Equal(ViewLang.JobGrade, viewModel.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateJobGrade, viewModel.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateJobGrade_ServiceGetNullJobGradeItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            JobGrade jobGrade = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobGradeByIdAsync(Arg.Any<int>()).Returns(jobGrade);

            // Act
            var actual = await controller.UpdateJobGrade(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateJobGradePost_JobGradeItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var jobGrade = MockJobGrade();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedJobGradeAsync(Arg.Any<JobGrade>()).Returns(true);

            // Act
            var actual = await controller.UpdateJobGrade(jobGrade);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, jobGrade.Name), errors["JobGrade.Name"]);
        }

        [Fact]
        public async Task UpdateJobGradePost_JobGradeItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var jobGrade = MockJobGrade();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedJobGradeAsync(Arg.Any<JobGrade>()).Returns(false);
            dataConfigurationService.UpdateJobGradeAsync(jobGrade).Returns(commandResult);

            // Act
            var actual = await controller.UpdateJobGrade(jobGrade);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Job Grade

        #region Commons

        [Fact]
        public async Task GetDivisionSelectListByCompanyId_ServiceGetDivisionSelectList_ReturnPartialViewWithDivisionSelectList()
        {
            // Arrange
            var id = 21;
            var divisionSelectList = MockDivisionSelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDivisionSelectListByCompanyIdAsync(id).Returns(divisionSelectList);

            // Act
            var actual = await controller.GetDivisionSelectListByCompanyId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(divisionSelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(divisionSelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(divisionSelectList.Items, viewModel.Items);
        }

        [Fact]
        public async Task GetDepartmentSelectListByDivisionId_ServiceGetDepartmentSelectList_ReturnPartialViewWithDepartmentSelectList()
        {
            // Arrange
            var id = 22;
            var departmentSelectList = MockDepartmentSelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDepartmentSelectListByDivisionIdAsync(id).Returns(departmentSelectList);

            // Act
            var actual = await controller.GetDepartmentSelectListByDivisionId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(departmentSelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(departmentSelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(departmentSelectList.Items, viewModel.Items);
        }

        [Fact]
        public async Task GetCostCenterSelectListByDepartmentId_ServiceGetCostCenterSelectList_ReturnPartialViewWithCostCenterSelectList()
        {
            // Arrange
            var id = 23;
            var costCenterSelectList = MockCostCenterSelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCostCenterSelectListByDepartmentIdAsync(id).Returns(costCenterSelectList);

            // Act
            var actual = await controller.GetCostCenterSelectListByDepartmentId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(costCenterSelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(costCenterSelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(costCenterSelectList.Items, viewModel.Items);
        }

        [Fact]
        public async Task GetSectionSelectListByCostCenterId_ServiceGetSectionSelectList_ReturnPartialViewWithSectionSelectList()
        {
            // Arrange
            var id = 24;
            var sectionSelectList = MockSectionSelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetSectionSelectListByCostCenterIdAsync(id).Returns(sectionSelectList);

            // Act
            var actual = await controller.GetSectionSelectListByCostCenterId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(sectionSelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(sectionSelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(sectionSelectList.Items, viewModel.Items);
        }

        [Fact]
        public async Task GetGradeLevelSelectListByJobCategoryId_ServiceGetGradeLevelSelectList_ReturnPartialViewWithGradeLevelSelectList()
        {
            // Arrange
            var id = 25;
            var gradeLevelSelectList = MockSectionSelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetGradeLevelSelectListByJobCateogryIdAsync(id).Returns(gradeLevelSelectList);

            // Act
            var actual = await controller.GetGradeLevelSelectListByJobCategoryId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(gradeLevelSelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(gradeLevelSelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(gradeLevelSelectList.Items, viewModel.Items);
        }

        #endregion Commons

        #region Initialization

        private DataConfigurationController GetDataConfigurationController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            dataConfigurationService = FakeDataConfigurationService();

            var controller = new DataConfigurationController(dataConfigurationService, sessionService, baseService);

            return controller;
        }

        private IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ISessionService FakeSessionService()
        {
            var sessionServiceInstance = Substitute.For<ISessionService>();
            sessionServiceInstance.UserId = 79;

            return sessionServiceInstance;
        }

        private static IDataConfigurationService FakeDataConfigurationService()
        {
            var dataConfigurationService = Substitute.For<IDataConfigurationService>();

            return dataConfigurationService;
        }

        private Field MockField()
        {
            return new Field
            {
                FieldId = 1,
                FieldName = "CompanyBVI",
                LinkGetAllItem = "Data/GetAll"
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

        private SelectList MockCompanySelectList()
        {
            var companies = new List<Company> { MockCompany() };

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        private SelectList MockJobCategorySelectList()
        {
            var jobCategoryList = new List<JobCategory> { MockJobCategory() };

            return new SelectList(jobCategoryList, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name));
        }

        private SelectList MockDivisionSelectList()
        {
            var divisions = new List<Division> { MockDivision() };

            return new SelectList(divisions, nameof(Division.DivisionId), nameof(Division.Code));
        }

        private SelectList MockDepartmentSelectList()
        {
            var departments = new List<Department> { MockDepartment() };

            return new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Code));
        }

        private SelectList MockCostCenterSelectList()
        {
            var costCenters = new List<CostCenter> { MockCostCenter() };

            return new SelectList(costCenters, nameof(CostCenter.CostCenterId), nameof(CostCenter.Code));
        }

        private SelectList MockSectionSelectList()
        {
            var sections = new List<Section> { MockSection() };

            return new SelectList(sections, nameof(Section.SectionId), nameof(Section.Code));
        }

        private SelectList MockGradeLevelSelectList()
        {
            var gradeLevelList = new List<GradeLevel> { MockGradeLevel() };

            return new SelectList(gradeLevelList, nameof(GradeLevel.GradeLevelId), nameof(GradeLevel.Name));
        }

        #endregion Initialization
    }
}