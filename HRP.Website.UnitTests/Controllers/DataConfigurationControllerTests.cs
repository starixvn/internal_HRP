namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.City;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Division;
    using HRP.Models.DataConfiguration.GradeLevel;
    using HRP.Models.DataConfiguration.JobGrade;
    using HRP.Models.DataConfiguration.Office;
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
        public void Index_ServiceGetFieldList_ReturnViewResultWithIndexViewModel()
        {
            // Arrange
            var fieldList = new List<Field> { MockField() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetDataForIndexPage().Returns(fieldList);

            // Act
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal(fieldList, viewModel.Fields);
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
            Assert.Equal(ViewLang.CompanyBVI, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCompanyBVI, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCompanyBVI, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public void CreateCompanyBVI_WhenCalled_ReturnPartialViewWithCreateSimpleItemViewModel()
        {
            // Arrange
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateCompanyBVI();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(ViewLang.CompanyBVI, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCompanyBVI, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCompanyBVIPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = new SimpleItem();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Item.ItemName", "Required");

            // Act
            var actual = await controller.CreateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Item.ItemName"]);
        }

        [Fact]
        public async Task CreateCompanyBVIPost_ModelStateIsValidAndCreateCompanyBVISuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, item.ItemName), returnValue["message"]);
        }

        [Fact]
        public async Task CreateCompanyBVIPost_ModelStateIsValidAndCreateCompanyBVIUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Item.ItemName"]);
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
            Assert.Equal(ViewLang.CompanyBVI, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCompanyBVI, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateCompanyBVIPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = new SimpleItem();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Item.ItemName", "Required");

            // Act
            var actual = await controller.UpdateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateCompanyBVIPost_ModelStateIsValidAndUpdateCompanyBVISuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCompanyBVIPost_ModelStateIsValidAndUpdateCompanyBVIUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateCompanyBVIAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCompanyBVI(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Item.ItemName"]);
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
            Assert.Equal(ViewLang.Division, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDivision, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateDivision, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateDivision_WhenCalled_ReturnPartialViewWithCreateDivisionViewModel()
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
            Assert.Equal(ViewLang.Division, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDivision, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateDivisionPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Division.Code", "Required");

            // Act
            var actual = await controller.CreateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Division.Code"]);
        }

        [Fact]
        public async Task CreateDivisionPost_ModelStateIsValidAndCreateDivisionSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.CreateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, division.Code), returnValue["message"]);
        }

        [Fact]
        public async Task CreateDivisionPost_ModelStateIsValidAndCreateDivisionUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.CreateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Division.Code"]);
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
            Assert.Equal(ViewLang.Division, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateDivision, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateDivisionPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Division.Code", "Required");

            // Act
            var actual = await controller.UpdateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Division.Code"]);
        }

        [Fact]
        public async Task UpdateDivisionPost_ModelStateIsValidAndUpdateDivisionSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateDivisionPost_ModelStateIsValidAndUpdateDivisionUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var division = MockDivision();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateDivisionAsync(division).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDivision(division);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Division.Code"]);
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
            Assert.Equal(ViewLang.Department, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDepartment, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateDepartment, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateDepartment_WhenCalled_ReturnPartialViewWithCreateDepartmentViewModel()
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
            Assert.Equal(ViewLang.Department, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateDepartment, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateDepartmentPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Department.Code", "Required");

            // Act
            var actual = await controller.CreateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Department.Code"]);
        }

        [Fact]
        public async Task CreateDepartmentPost_ModelStateIsValidAndCreateDepartmentSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.CreateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, department.Code), returnValue["message"]);
        }

        [Fact]
        public async Task CreateDepartmentPost_ModelStateIsValidAndCreateDepartmentUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.CreateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Department.Code"]);
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
            Assert.Equal(ViewLang.Department, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateDepartment, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateDepartmentPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Department.Code", "Required");

            // Act
            var actual = await controller.UpdateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Department.Code"]);
        }

        [Fact]
        public async Task UpdateDepartmentPost_ModelStateIsValidAndUpdateDepartmentSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateDepartmentPost_ModelStateIsValidAndUpdateDepartmentUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var department = MockDepartment();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateDepartmentAsync(department).Returns(commandResult);

            // Act
            var actual = await controller.UpdateDepartment(department);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Department.Code"]);
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
            Assert.Equal(ViewLang.CostCenter, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCostCenter, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCostCenter, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateCostCenter_WhenCalled_ReturnPartialViewWithCreateCostCenterViewModel()
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
            Assert.Equal(ViewLang.CostCenter, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCostCenter, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCostCenterPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("CostCenter.Code", "Required");

            // Act
            var actual = await controller.CreateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["CostCenter.Code"]);
        }

        [Fact]
        public async Task CreateCostCenterPost_ModelStateIsValidAndCreateCostCenterSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.CreateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, costCenter.Code), returnValue["message"]);
        }

        [Fact]
        public async Task CreateCostCenterPost_ModelStateIsValidAndCreateCostCenterUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.CreateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["CostCenter.Code"]);
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
            Assert.Equal(ViewLang.CostCenter, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCostCenter, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateCostCenterPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("CostCenter.Code", "Required");

            // Act
            var actual = await controller.UpdateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["CostCenter.Code"]);
        }

        [Fact]
        public async Task UpdateCostCenterPost_ModelStateIsValidAndUpdateCostCenterSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCostCenterPost_ModelStateIsValidAndUpdateCostCenterUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var costCenter = MockCostCenter();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateCostCenterAsync(costCenter).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCostCenter(costCenter);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["CostCenter.Code"]);
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
            Assert.Equal(ViewLang.Section, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateSection, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateSection, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateSection_WhenCalled_ReturnPartialViewWithCreateSectionViewModel()
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
            Assert.Equal(ViewLang.Section, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateSection, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateSectionPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var section = MockSection();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Section.Code", "Required");

            // Act
            var actual = await controller.CreateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Section.Code"]);
        }

        [Fact]
        public async Task CreateSectionPost_ModelStateIsValidAndCreateSectionSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.CreateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, section.Code), returnValue["message"]);
        }

        [Fact]
        public async Task CreateSectionPost_ModelStateIsValidAndCreateSectionUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.CreateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Section.Code"]);
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
            Assert.Equal(ViewLang.Section, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateSection, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateSectionPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var section = MockSection();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Section.Code", "Required");

            // Act
            var actual = await controller.UpdateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Section.Code"]);
        }

        [Fact]
        public async Task UpdateSectionPost_ModelStateIsValidAndUpdateSectionSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.UpdateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateSectionPost_ModelStateIsValidAndUpdateSectionUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var section = MockSection();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateSectionAsync(section).Returns(commandResult);

            // Act
            var actual = await controller.UpdateSection(section);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Section.Code"]);
        }

        #endregion Section

        #region Job Category

        [Fact]
        public async Task GetAllJobCategory_ServiceGetSimpleItemList_ReturnPartialViewWithSimpleItemTableViewModel()
        {
            // Arrange
            var items = new List<SimpleItem> { MockSimpleItem() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllJobCategoryAsync().Returns(items);

            // Act
            var actual = await controller.GetAllJobCategory();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SimpleItemTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SimpleItemTable", viewResult.ViewName);
            Assert.Equal(items, viewModel.Items);
            Assert.Equal(ViewLang.JobCategory, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobCategory, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateJobCategory, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public void CreateJobCategory_WhenCalled_ReturnPartialViewWithCreateSimpleItemViewModel()
        {
            // Arrange
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateJobCategory();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(ViewLang.JobCategory, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobCategory, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateJobCategoryPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Item.ItemName", "Required");

            // Act
            var actual = await controller.CreateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Item.ItemName"]);
        }

        [Fact]
        public async Task CreateJobCategoryPost_ModelStateIsValidAndCreateJobCategorySuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateJobCategoryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, item.ItemName), returnValue["message"]);
        }

        [Fact]
        public async Task CreateJobCategoryPost_ModelStateIsValidAndCreateJobCategoryUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateJobCategoryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateJobCategory_ServiceGetJobCategoryItem_ReturnPartialViewWithUpdateSimpleItemViewModel()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobCategoryByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateJobCategory(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSimpleItem", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal(item, viewModel.Item);
            Assert.Equal(ViewLang.JobCategory, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateJobCategory, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateJobCategory_ServiceGetNullJobCategoryItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            SimpleItem item = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetJobCategoryByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateJobCategory(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateJobCategoryPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("Item.ItemName", "Required");

            // Act
            var actual = await controller.UpdateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateJobCategoryPost_ModelStateIsValidAndUpdateCompanyBVISuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateJobCategoryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateJobCategoryPost_ModelStateIsValidAndUpdateJobCategoryUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateJobCategoryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateJobCategory(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["Item.ItemName"]);
        }

        #endregion Job Category

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
            Assert.Equal(ViewLang.GradeLevel, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateGradeLevel, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateGradeLevel, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateGradeLevel_WhenCalled_ReturnPartialViewWithCreateGradeLevelViewModel()
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
            Assert.Equal(ViewLang.GradeLevel, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateGradeLevel, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateGradeLevelPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("GradeLevel.Name", "Required");

            // Act
            var actual = await controller.CreateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["GradeLevel.Name"]);
        }

        [Fact]
        public async Task CreateGradeLevelPost_ModelStateIsValidAndCreateGradeLevelSuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.CreateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemCreatedSuccessful, gradeLevel.Name), returnValue["message"]);
        }

        [Fact]
        public async Task CreateGradeLevelPost_ModelStateIsValidAndCreateGradeLevelUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.CreateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.CreateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["GradeLevel.Name"]);
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
            Assert.Equal(ViewLang.GradeLevel, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateGradeLevel, viewModel.Field.LinkUpdateItem);
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
        public async Task UpdateGradeLevelPost_ModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            controller.ModelState.AddModelError("GradeLevel.Name", "Required");

            // Act
            var actual = await controller.UpdateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal("Required", errors["GradeLevel.Name"]);
        }

        [Fact]
        public async Task UpdateGradeLevelPost_ModelStateIsValidAndUpdateCompanyBVISuccessful_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.UpdateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(Message.ItemUpdatedSuccessful, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateGradeLevelPost_ModelStateIsValidAndUpdateGradeLevelUnsuccessful_ReturnJsonResultWithErrors()
        {
            // Arrange
            var commandResult = new CommandResult();
            var gradeLevel = MockGradeLevel();
            var controller = GetDataConfigurationController();
            dataConfigurationService.UpdateGradeLevelAsync(gradeLevel).Returns(commandResult);

            // Act
            var actual = await controller.UpdateGradeLevel(gradeLevel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, errors["GradeLevel.Name"]);
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
            Assert.Equal(ViewLang.JobGrade, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobGrade, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateJobGrade, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateJobGrade_WhenCalled_ReturnPartialViewWithCreateJobGradeViewModel()
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
            Assert.Equal(ViewLang.JobGrade, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateJobGrade, viewModel.Field.LinkCreateItem);
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
            Assert.Equal(ViewLang.JobGrade, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateJobGrade, viewModel.Field.LinkUpdateItem);
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

        #region Country

        [Fact]
        public async Task GetAllCountry_ServiceGetSimpleItemList_ReturnPartialViewWithSimpleItemTableViewModel()
        {
            // Arrange
            var items = new List<SimpleItem> { MockSimpleItem() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllCountryAsync().Returns(items);

            // Act
            var actual = await controller.GetAllCountry();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SimpleItemTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SimpleItemTable", viewResult.ViewName);
            Assert.Equal(items, viewModel.Items);
            Assert.Equal(ViewLang.Country, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCountry, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCountry, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public void CreateCountry_WhenCalled_ReturnPartialViewWithCreateSimpleItemViewModel()
        {
            // Arrange
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateCountry();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(ViewLang.Country, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCountry, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCountryPost_CountryItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCountryAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.CreateCountry(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task CreateCountryPost_CompanyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCountryAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.CreateCountryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateCountry(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCountry_ServiceGetCountryItem_ReturnPartialViewWithUpdateSimpleItemViewModel()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCountryByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCountry(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSimpleItem", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal(item, viewModel.Item);
            Assert.Equal(ViewLang.Country, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCountry, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateCountry_ServiceGetNullCountryItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            SimpleItem item = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCountryByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCountry(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateCountryPost_CountryItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCountryAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.UpdateCountry(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateCountryPost_CompanyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCountryAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.UpdateCountryAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCountry(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Country

        #region City

        [Fact]
        public async Task GetAllCity_ServiceGetCityList_ReturnPartialViewWithCityTableViewModel()
        {
            // Arrange
            var cities = new List<City> { MockCity() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllCityAsync().Returns(cities);

            // Act
            var actual = await controller.GetAllCity();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CityTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CityTable", viewResult.ViewName);
            Assert.Equal(cities, viewModel.Cities);
            Assert.Equal(ViewLang.City, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCity, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCity, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateCity_WhenCalled_ReturnPartialViewWithCreateDivisionViewModel()
        {
            // Arrange
            var countrySelectList = MockCountrySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCountrySelectListAsync().Returns(countrySelectList);

            // Act
            var actual = await controller.CreateCity();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateCityViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateCity", viewResult.ViewName);
            Assert.Equal(countrySelectList, viewModel.CountryList);
            Assert.Equal(ViewLang.City, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCity, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCityPost_CityItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var city = MockCity();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCityAsync(Arg.Any<City>()).Returns(true);

            // Act
            var actual = await controller.CreateCity(city);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, city.Code), errors["City.Code"]);
        }

        [Fact]
        public async Task CreateCityPost_CityItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var city = MockCity();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCityAsync(Arg.Any<City>()).Returns(false);
            dataConfigurationService.CreateCityAsync(city).Returns(commandResult);

            // Act
            var actual = await controller.CreateCity(city);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCity_ServiceGetCityItem_ReturnPartialViewWithUpdateCityViewModel()
        {
            // Arrange
            var city = MockCity();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCityByIdAsync(Arg.Any<int>()).Returns(city);

            // Act
            var actual = await controller.UpdateCity(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateCity", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateCityViewModel>(viewResult.ViewData.Model);
            Assert.Equal(city, viewModel.City);
            Assert.Equal(ViewLang.City, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCity, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateCity_ServiceGetNullCityItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            City city = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCityByIdAsync(Arg.Any<int>()).Returns(city);

            // Act
            var actual = await controller.UpdateCity(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateCityPost_CityItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var city = MockCity();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCityAsync(Arg.Any<City>()).Returns(true);

            // Act
            var actual = await controller.UpdateCity(city);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, city.Code), errors["City.Code"]);
        }

        [Fact]
        public async Task UpdateCityPost_CityItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var city = MockCity();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCityAsync(Arg.Any<City>()).Returns(false);
            dataConfigurationService.UpdateCityAsync(city).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCity(city);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion City

        #region Office

        [Fact]
        public async Task GetAllOffice_ServiceGetOfficeList_ReturnPartialViewWithOfficeTableViewModel()
        {
            // Arrange
            var offices = new List<Office> { MockOffice() };
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetAllOfficeAsync().Returns(offices);

            // Act
            var actual = await controller.GetAllOffice();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<OfficeTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_OfficeTable", viewResult.ViewName);
            Assert.Equal(offices, viewModel.Offices);
            Assert.Equal(ViewLang.Office, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateOffice, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateOffice, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task CreateOffice_WhenCalled_ReturnPartialViewWithCreateOfficeViewModel()
        {
            // Arrange
            var countrySelectList = MockCountrySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCountrySelectListAsync().Returns(countrySelectList);

            // Act
            var actual = await controller.CreateOffice();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateOfficeViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateOffice", viewResult.ViewName);
            Assert.Equal(countrySelectList, viewModel.CountryList);
            Assert.Equal(ViewLang.Office, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateOffice, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateOfficePost_OfficeItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var office = MockOffice();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedOfficeAsync(Arg.Any<Office>()).Returns(true);

            // Act
            var actual = await controller.CreateOffice(office);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, office.Code), errors["Office.Code"]);
        }

        [Fact]
        public async Task CreateOffice_OfficeItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var office = MockOffice();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedOfficeAsync(Arg.Any<Office>()).Returns(false);
            dataConfigurationService.CreateOfficeAsync(office).Returns(commandResult);

            // Act
            var actual = await controller.CreateOffice(office);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateOffice_ServiceGetOfficeItem_ReturnPartialViewWithUpdateOfficeViewModel()
        {
            // Arrange
            var office = MockOffice();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetOfficeByIdAsync(Arg.Any<int>()).Returns(office);

            // Act
            var actual = await controller.UpdateOffice(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateOffice", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateOfficeViewModel>(viewResult.ViewData.Model);
            Assert.Equal(office, viewModel.Office);
            Assert.Equal(ViewLang.Office, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateOffice, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateOffice_ServiceGetNullOfficeItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            Office office = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetOfficeByIdAsync(Arg.Any<int>()).Returns(office);

            // Act
            var actual = await controller.UpdateOffice(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateOfficePost_OfficeItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var office = MockOffice();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedOfficeAsync(Arg.Any<Office>()).Returns(true);

            // Act
            var actual = await controller.UpdateOffice(office);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, office.Code), errors["Office.Code"]);
        }

        [Fact]
        public async Task UpdateOfficePost_OfficeItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var office = MockOffice();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedOfficeAsync(Arg.Any<Office>()).Returns(false);
            dataConfigurationService.UpdateOfficeAsync(office).Returns(commandResult);

            // Act
            var actual = await controller.UpdateOffice(office);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Office

        #region RefResource Item

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task GetAllRefResourceItem_ServiceGetSimpleItemList_ReturnPartialViewWithSimpleItemTableViewModel(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var refName = string.Empty;
            var items = new List<SimpleItem> { MockSimpleItem() };
            var controller = GetDataConfigurationController();
            sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host).Returns(true);
            dataConfigurationService.GetAllRefResourceItemAsync(Arg.Do<string>(x => refName = x)).Returns(items);

            // Act
            var actual = await controller.GetAllRefResourceItem(fieldId);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SimpleItemTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SimpleItemTable", viewResult.ViewName);
            Assert.Equal(items, viewModel.Items);
            Assert.Equal(field, viewModel.Field);
            Assert.True(viewModel.IsHost);
            Assert.Equal(field.RefName, refName);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public void CreateRefResourceItem_WhenCalled_ReturnPartialViewWithCreateSimpleItemViewModel(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateRefResourceItem(fieldId);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(field, viewModel.Field);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task CreateRefResourceItemPost_RefResourceItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var refName = string.Empty;
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedRefResourceItemAsync(Arg.Do<string>(x => refName = x), Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.CreateRefResourceItem(fieldId, item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
            Assert.Equal(field.RefName, refName);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task CreateRefResourceItemPost_RefResourceItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedRefResourceItemAsync(Arg.Is(field.RefName), Arg.Is(item)).Returns(false);
            dataConfigurationService.CreateRefResourceItemAsync(Arg.Is(field.RefName), Arg.Is(item)).Returns(commandResult);

            // Act
            var actual = await controller.CreateRefResourceItem(fieldId, item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task UpdateRefResourceItem_ServiceGetRefResourceItem_ReturnPartialViewWithUpdateSimpleItemViewModel(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var refName = string.Empty;
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetRefResourceByNameAndIdAsync(Arg.Do<string>(x => refName = x), Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateRefResourceItem(fieldId, 11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSimpleItem", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal(item, viewModel.Item);
            Assert.Equal(field, viewModel.Field);
            Assert.Equal(field.RefName, refName);
        }

        [Fact]
        public async Task UpdateRefResourceItem_ServiceGetNullRefResourceItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            SimpleItem item = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetRefResourceByNameAndIdAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateRefResourceItem(30, 11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task UpdateRefResourceItemPost_RefResourceItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var refName = string.Empty;
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedRefResourceItemAsync(Arg.Do<string>(x => refName = x), Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.UpdateRefResourceItem(fieldId, item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
            Assert.Equal(field.RefName, refName);
        }

        [Theory]
        [InlineData(35)]
        [InlineData(34)]
        public async Task UpdateRefResourceItemPost_RefResourceItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage(int fieldId)
        {
            // Arrange
            var field = FieldsDictionary.Fields[fieldId];
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedRefResourceItemAsync(Arg.Is(field.RefName), Arg.Is(item)).Returns(false);
            dataConfigurationService.UpdateRefResourceItemAsync(Arg.Is(field.RefName), Arg.Is(item)).Returns(commandResult);

            // Act
            var actual = await controller.UpdateRefResourceItem(fieldId, item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion RefResource Item

        #region Currency

        [Fact]
        public async Task GetAllCurrency_ServiceGetSimpleItemList_ReturnPartialViewWithSimpleItemTableViewModel()
        {
            // Arrange
            var items = new List<SimpleItem> { MockSimpleItem() };
            var controller = GetDataConfigurationController();
            sessionService.HasPermissionOnFunction(FunctionKey.DataConfiguration, Policy.Host).Returns(true);
            dataConfigurationService.GetAllCurrencyAsync().Returns(items);

            // Act
            var actual = await controller.GetAllCurrency();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<SimpleItemTableViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_SimpleItemTable", viewResult.ViewName);
            Assert.Equal(items, viewModel.Items);
            Assert.Equal(ViewLang.Currency, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCurrency, viewModel.Field.LinkCreateItem);
            Assert.Equal(DataConfigurationActionLink.UpdateCurrency, viewModel.Field.LinkUpdateItem);
            Assert.True(viewModel.IsHost);
        }

        [Fact]
        public void CreateCurrency_WhenCalled_ReturnPartialViewWithCreateSimpleItemViewModel()
        {
            // Arrange
            var controller = GetDataConfigurationController();

            // Act
            var actual = controller.CreateCurrency();

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<CreateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal("_CreateSimpleItem", viewResult.ViewName);
            Assert.Equal(ViewLang.Currency, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.CreateCurrency, viewModel.Field.LinkCreateItem);
        }

        [Fact]
        public async Task CreateCurrencyPost_CurrencyItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCurrencyAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.CreateCurrency(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task CreateCurrencyPost_CurrencyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCurrencyAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.CreateCurrencyAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.CreateCurrency(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        [Fact]
        public async Task UpdateCurrency_ServiceGetCurrencyItem_ReturnPartialViewWithUpdateSimpleItemViewModel()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCurrencyByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCurrency(11);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal("_UpdateSimpleItem", viewResult.ViewName);
            var viewModel = Assert.IsAssignableFrom<UpdateSimpleItemViewModel>(viewResult.ViewData.Model);
            Assert.Equal(item, viewModel.Item);
            Assert.Equal(ViewLang.Currency, viewModel.Field.FieldName);
            Assert.Equal(DataConfigurationActionLink.UpdateCurrency, viewModel.Field.LinkUpdateItem);
        }

        [Fact]
        public async Task UpdateCurrency_ServiceGetNullCurrencyItem_ReturnContentResultWithMessageNotFound()
        {
            // Arrange
            SimpleItem item = null;
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCurrencyByIdAsync(Arg.Any<int>()).Returns(item);

            // Act
            var actual = await controller.UpdateCurrency(11);

            // Assert
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(Message.NotFoundError, viewResult.Content);
        }

        [Fact]
        public async Task UpdateCurrencyPost_CurrencyItemIsExistedAndModelStateIsInvalid_ReturnJsonResultWithErrors()
        {
            // Arrange
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCurrencyAsync(Arg.Any<SimpleItem>()).Returns(true);

            // Act
            var actual = await controller.UpdateCurrency(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            var errors = (IDictionary<string, string>)returnValue["errors"];
            Assert.False((bool)returnValue["isSuccessful"]);
            Assert.Equal(string.Format(Message.ItemNameExisted, item.ItemName), errors["Item.ItemName"]);
        }

        [Fact]
        public async Task UpdateCurrencyPost_CurrencyItemIsNonExistedAndModelStateIsValid_ReturnJsonResultWithSuccessfulMessage()
        {
            // Arrange
            var commandResult = MockSuccessfulCommandResult();
            var item = MockSimpleItem();
            var controller = GetDataConfigurationController();
            dataConfigurationService.IsExistedCurrencyAsync(Arg.Any<SimpleItem>()).Returns(false);
            dataConfigurationService.UpdateCurrencyAsync(item).Returns(commandResult);

            // Act
            var actual = await controller.UpdateCurrency(item);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var returnValue = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.True((bool)returnValue["isSuccessful"]);
            Assert.Equal(commandResult.Message, returnValue["message"]);
        }

        #endregion Currency

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
            var gradeLevelSelectList = MockGradeLevelSelectList();
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

        [Fact]
        public async Task GetCitySelectListByCountryId_ServiceGetCitySelectList_ReturnPartialViewWithCitySelectList()
        {
            // Arrange
            var id = 20;
            var citySelectList = MockCitySelectList();
            var controller = GetDataConfigurationController();
            dataConfigurationService.GetCitySelectListByCountryIdAsync(id).Returns(citySelectList);

            // Act
            var actual = await controller.GetCitySelectListByCountryId(id);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<SelectList>(viewResult.ViewData.Model);
            Assert.Equal("_SelectListItem", viewResult.ViewName);
            Assert.Equal(citySelectList.DataValueField, viewModel.DataValueField);
            Assert.Equal(citySelectList.DataTextField, viewModel.DataTextField);
            Assert.Equal(citySelectList.Items, viewModel.Items);
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

        private SelectList MockCountrySelectList()
        {
            var countries = new List<Country> { MockCountry() };

            return new SelectList(countries, nameof(Country.CountryId), nameof(Country.Code));
        }

        private SelectList MockCitySelectList()
        {
            var cities = new List<City> { MockCity() };

            return new SelectList(cities, nameof(City.CityId), nameof(City.Code));
        }

        #endregion Initialization
    }
}