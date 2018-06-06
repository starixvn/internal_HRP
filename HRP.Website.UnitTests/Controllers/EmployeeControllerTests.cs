namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.Employee;
    using HRP.Models.Employee.Payroll;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class EmployeeControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICommonService commonService;
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

            string departmentIds = "1,2";
            string employmentStatusIds = "1,2";
            int? employeeId = 1;
            string displayName = "trinh dang thang";

            var employeeController = GetEmployeeController();
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);
            employeeService.GetEmployeesAsync(Arg.Any<EmployeeManagementViewModel>()).Returns(employees);

            //Act
            var actual = await employeeController.Index(departmentIds, employmentStatusIds, employeeId, displayName);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsAssignableFrom<EmployeeManagementViewModel>(viewResult.ViewData.Model);
            Assert.True(model.IsHostCreateEmployee);
            Assert.Equal(model.DepartmentIds, departmentIds);
            Assert.Equal(model.EmploymentStatusIds, employmentStatusIds);
            Assert.Equal(model.EmployeeId, employeeId);
            Assert.Equal(model.DisplayName, displayName);
            Assert.Equal(employees, model.Employees);
        }

        [Fact]
        public async Task GetDepartment_WhenCalled_ReturnAsExpectedDepartments()
        {
            // Arrange
            var preparedData = PrepareDataForGetDepartmentTest();
            var departmentIds = preparedData.Item3;
            var expectedCompanyList = preparedData.Item1;
            var expected = preparedData.Item2;

            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<CompanyTree>(Arg.Any<GetDepartmentByUserCriteria>()).Returns(expectedCompanyList);

            // Act
            var actual = await employeeController.GetDepartment(departmentIds);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected[0].Items[0].Items[0].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Items[0].Value);
            Assert.Equal(expected[0].Items[0].Items[0].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Items[0].Text);
            Assert.Equal(expected[0].Items[0].Items[0].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Items[0].Checked);

            Assert.Equal(expected[0].Items[0].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Value);
            Assert.Equal(expected[0].Items[0].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Text);
            Assert.Equal(expected[0].Items[0].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Items[0].Checked);

            Assert.Equal(expected[0].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Value);
            Assert.Equal(expected[0].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Text);
            Assert.Equal(expected[0].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[0].Checked);

            Assert.Equal(expected[1].Items[0].Items[0].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Items[0].Value);
            Assert.Equal(expected[1].Items[0].Items[0].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Items[0].Text);
            Assert.Equal(expected[1].Items[0].Items[0].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Items[0].Checked);

            Assert.Equal(expected[1].Items[0].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Value);
            Assert.Equal(expected[1].Items[0].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Text);
            Assert.Equal(expected[1].Items[0].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Items[0].Checked);

            Assert.Equal(expected[1].Value, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Value);
            Assert.Equal(expected[1].Text, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Text);
            Assert.Equal(expected[1].Checked, ((List<MultiSelectViewModel>)jsonResult.Value)[1].Checked);
        }

        [Fact]
        public async Task GetEmployeeStatus_WhenCalled_ReturnAsExpectedEmployeeStatus()
        {
            // Arrange
            var employeeStatus = "1";
            var expected = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel
                {
                    Value = "1",
                    Text = "Probation",
                    Checked = false
                },
                new MultiSelectViewModel
                {
                    Value = "2",
                    Text = "Probation",
                    Checked = false
                }
            };

            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<MultiSelectViewModel>(Arg.Any<GetEmployeeStatusByUserCriteria>()).Returns(expected);

            // Act
            var actual = await employeeController.GetEmployeeStatus(employeeStatus);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #region CreateEmployee

        [Fact]
        public async Task CreateEmployee_CheckViewResult_ReturnExpectedCreateEmployeeViewModel()
        {
            // Arrange
            var tuple = new Tuple<SelectList, SelectList, SelectList, SelectList>(
                    new SelectList(new List<string> { "item1" }, "val1", "text1"),
                    new SelectList(new List<string> { "item2" }, "val2", "text2"),
                    new SelectList(new List<string> { "item3" }, "val3", "text3"),
                    new SelectList(new List<string> { "item4" }, "val4", "text4")
                );
            var filter = new CachingFilterViewModel { DepartmentIds = "1,2", EmployeeId = 1, EmployeeName = "tim.trinh", EmploymentStatusIds = "1,2" };

            var employeeController = GetEmployeeController();
            employeeService.GetDataToCreateEmployeeAsync().Returns(tuple);

            // Act
            var actual = await employeeController.CreateEmployee(filter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<CreateEmployeeViewModel>(viewResult.Model);
            Assert.Equal("CreateEmployee", viewResult.ViewName);
            Assert.Equal(tuple.Item1.Items, viewModel.GenderList.Items);
            Assert.Equal(tuple.Item1.DataTextField, viewModel.GenderList.DataTextField);
            Assert.Equal(tuple.Item1.DataValueField, viewModel.GenderList.DataValueField);

            Assert.Equal(tuple.Item2.Items, viewModel.NationalityList.Items);
            Assert.Equal(tuple.Item2.DataTextField, viewModel.NationalityList.DataTextField);
            Assert.Equal(tuple.Item2.DataValueField, viewModel.NationalityList.DataValueField);

            Assert.Equal(tuple.Item3.Items, viewModel.MaritalStatusList.Items);
            Assert.Equal(tuple.Item3.DataTextField, viewModel.MaritalStatusList.DataTextField);
            Assert.Equal(tuple.Item3.DataValueField, viewModel.MaritalStatusList.DataValueField);

            Assert.Equal(tuple.Item4.Items, viewModel.NoOfChildrenList.Items);
            Assert.Equal(tuple.Item4.DataTextField, viewModel.NoOfChildrenList.DataTextField);
            Assert.Equal(tuple.Item4.DataValueField, viewModel.NoOfChildrenList.DataValueField);

            Assert.Equal(filter, viewModel.CachingFilter);
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
                kvp =>
                {
                    Assert.Equal(nameof(model.ProfilePicture), kvp.Key);
                    Assert.Equal(commandResult.Message, kvp.Value);
                }
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
                kvp =>
                {
                    Assert.Equal("nickname", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
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
        public async Task UpdateEmployee_WhenEmployeeIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var id = 12;
            Employee employee = null;
            var filter = new CachingFilterViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetEmployeeByIdAsync(Arg.Is<int>(x => x == id)).Returns(employee);

            // Act
            var result = await employeeController.UpdateEmployee(id, filter);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task UpdateEmployee_WhenEmployeeNotNull_ReturnViewResultWithCreateEmployeeViewModel()
        {
            // Arrange
            var id = 99;
            var employee = MockEmployee();
            var tuple = MockTupleSelectList();
            var contact = new EmergencyContact
            {
                EmployeeId = 11,
                Address = "asdasd"
            };
            var filter = new CachingFilterViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetEmployeeByIdAsync(Arg.Is<int>(x => x == id)).Returns(employee);
            employeeService.GetEmergencyContactByEmployeeIdAsync(Arg.Is<int>(x => x == id)).Returns(contact);
            employeeService.GetDataToCreateEmployeeAsync().Returns(tuple);

            // Act
            var result = await employeeController.UpdateEmployee(id, filter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<CreateEmployeeViewModel>(viewResult.Model);
            Assert.Equal("UpdateEmployee", viewResult.ViewName);
            Assert.Equal(employee.EmployeeId, viewModel.EmployeeId);
            Assert.Equal(employee.Nickname, viewModel.Nickname);
            Assert.Equal(employee.FullName, viewModel.FullName);
            Assert.Equal(employee.Gender, viewModel.Gender);
            Assert.Equal(employee.DateOfBirth, viewModel.DateOfBirth);
            Assert.Equal(employee.PlaceOfBirth, viewModel.PlaceOfBirth);
            Assert.Equal(employee.Nationality, viewModel.Nationality);
            Assert.Equal(employee.NRICNo, viewModel.NRICNo);
            Assert.Equal(employee.MaritalStatus, viewModel.MaritalStatus);
            Assert.Equal(employee.NoOfChildren, viewModel.NoOfChildren);
            Assert.Equal(employee.OverseasPhoneNo, viewModel.OverseasPhoneNo);
            Assert.Equal(employee.LocalPhoneNo, viewModel.LocalPhoneNo);
            Assert.Equal(employee.OtherPhoneNo, viewModel.OtherPhoneNo);
            Assert.Equal(employee.PassportNo, viewModel.PassportNo);
            Assert.Equal(employee.DateIssue, viewModel.DateIssue);
            Assert.Equal(employee.ExpiredDate, viewModel.ExpiredDate);
            Assert.Equal(employee.PlaceOfIssue, viewModel.PlaceOfIssue);
            Assert.Equal(employee.Skype, viewModel.Skype);
            Assert.Equal(employee.PersonalEmail, viewModel.PersonalEmail);
            Assert.Equal(employee.PermanentAddress, viewModel.PermanentAddress);
            Assert.Equal(employee.ResidenceAddress, viewModel.ResidenceAddress);
            Assert.Equal(employee.MailingAddress, viewModel.MailingAddress);
            Assert.Equal(employee.ProfilePic, viewModel.ProfilePic);
            Assert.Equal(employee.IsHost, viewModel.IsHost);
            Assert.Equal(tuple.Item1, viewModel.GenderList);
            Assert.Equal(tuple.Item2, viewModel.NationalityList);
            Assert.Equal(tuple.Item3, viewModel.MaritalStatusList);
            Assert.Equal(tuple.Item4, viewModel.NoOfChildrenList);
            Assert.Equal(filter, viewModel.CachingFilter);
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
                kvp =>
                {
                    Assert.Equal(nameof(model.ProfilePicture), kvp.Key);
                    Assert.Equal(commandResult.Message, kvp.Value);
                }
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
                kvp =>
                {
                    Assert.Equal("nickname", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
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
        public async Task CreateEmploymentGet_CheckViewResult_ReturnAsExpected()
        {
            //Arrange
            var viewName = "_CreateEmployment";
            var filter = new CachingFilterViewModel();
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Code = "DAR" }
            };
            var jobCategories = new List<JobCategory>
            {
                new JobCategory { JobCategoryId = 1, Name = "Staff" }
            };
            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel {RefName = ReferenceResource.Gender, Value = "a", ResourceId = 1 },
                new ReferenceResourceModel {RefName = ReferenceResource.Nationality, Value = "b", ResourceId = 11 },
                new ReferenceResourceModel {RefName = ReferenceResource.MaritalStatus, Value = "c", ResourceId = 2 },
                new ReferenceResourceModel {RefName = ReferenceResource.NoOfChildren, Value = "d", ResourceId = 33 }
            };
            var companySelectList = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
            var jobCategorySelectList = new SelectList(jobCategories, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name));
            var refResourceSelectList = new SelectList(refResources, nameof(ReferenceResourceModel.ResourceId), nameof(ReferenceResourceModel.Value));
            var employeeController = GetEmployeeController();
            commonService.GetSelectList(Arg.Any<IEnumerable<Company>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(companySelectList);
            commonService.GetSelectList(Arg.Any<IEnumerable<JobCategory>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(jobCategorySelectList);
            commonService.GetSelectList(Arg.Any<IEnumerable<ReferenceResourceModel>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(refResourceSelectList);

            //Act
            var actual = await employeeController.CreateEmployment(filter);

            //Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<CreateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(viewName, viewResult.ViewName);
            Assert.Equal(filter, viewModel.CachingFilter);
            Assert.Equal(companySelectList, viewModel.Companies);
            Assert.Equal(jobCategorySelectList, viewModel.JobCategories);
            Assert.Equal(refResourceSelectList, viewModel.LocalCompanies);
            Assert.Equal(refResourceSelectList, viewModel.LocalJobTitles);
            Assert.Equal(refResourceSelectList, viewModel.EmploymentTypes);
            Assert.Equal(refResourceSelectList, viewModel.EmploymentStatuses);
            Assert.Equal(refResourceSelectList, viewModel.Countries);
            Assert.Equal(refResourceSelectList, viewModel.Cities);
            Assert.Equal(refResourceSelectList, viewModel.Offices);
            Assert.Equal(refResourceSelectList, viewModel.WorkPermitType1s);
            Assert.Equal(refResourceSelectList, viewModel.WorkPermitType2s);
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
                kvp =>
                {
                    Assert.Equal("company", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
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
        public async Task UpdateEmploymentGet_CheckViewName_ReturnExpectedViewName()
        {
            // Arrange
            var expected = "_UpdateEmployment";
            var filter = new CachingFilterViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmploymentAsync(Arg.Any<int>()).Returns(new UpdateEmploymentViewModel());

            // Act
            var actual = await employeeController.UpdateEmployment(1, filter);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public async Task UpdateEmployment_ViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            UpdateEmploymentViewModel employment = null;
            var filter = new CachingFilterViewModel();
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmploymentAsync(Arg.Any<int>()).Returns(employment);

            // Act
            var result = await employeeController.UpdateEmployment(1, filter);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task UpdateEmploymentGet_ViewModelHasValue_ReturnExpectedViewModel()
        {
            // Arrange
            var employeeId = 1;
            var filter = new CachingFilterViewModel();
            var expected = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId
            };
            var employeeController = GetEmployeeController();
            employeeService.GetDataToUpdateEmploymentAsync(Arg.Any<int>()).Returns(expected);

            // Act
            var actual = await employeeController.UpdateEmployment(employeeId, filter);

            // Assert
            var viewResult = Assert.IsType<PartialViewResult>(actual);
            var viewModel = Assert.IsType<UpdateEmploymentViewModel>(viewResult.Model);
            Assert.Equal(filter, viewModel.CachingFilter);
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
                kvp =>
                {
                    Assert.Equal("company", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
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
            commonService.GetDivisionByCompanyIdAsync(1).Returns(expected);

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
            commonService.GetDepartmentByDivisionIdAsync(1).Returns(expected);

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
            commonService.GetSectionByCostCenterIdAsync(1).Returns(expected);

            // Act
            var actual = await employeeController.GetSectionByCostCenterId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task GetCostCenterByDepartmentId_WhenCalled_ReturnExpectedDivisions()
        {
            // Arrange
            var expected = new List<CostCenter>
            {
                new CostCenter { CostCenterId = 1, Code = "SM", Name = "Senior Management", IsActive = true }
            };
            var employeeController = GetEmployeeController();
            commonService.GetCostCenterByDepartmentIdAsync(1).Returns(expected);

            // Act
            var actual = await employeeController.GetCostCenterByDepartmentId(1);

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
            commonService.GetGradeLevelByJobCategoryIdAsync(1).Returns(expected);

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
            commonService.GetJobGradeByGradeLevelIdAsync(1).Returns(expected);

            // Act
            var actual = await employeeController.GetJobGradeByGradeLevelId(1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        [Fact]
        public async Task Payroll_InvalidEmployeeId_RedirectToErrorPage()
        {
            // Arrange
            var employeeId = 0;
            var filter = new CachingFilterViewModel();
            var employeeController = GetEmployeeController();
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(Task.FromResult<EmployeePayrollViewModel>(null));

            // Act
            var actual = await employeeController.Payroll(employeeId, filter);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("PageNotFound", redirectResult.ActionName);
            Assert.Equal("Error", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Payroll_ValidEmployeeId_ReturnView()
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
                    DirectBanking1AccountName = "Account 1",
                    DirectBanking2AccountId = 2,
                    DirectBanking2AccountName = "Account 2",
                }
            };
            var filter = new CachingFilterViewModel();
            payrollService.GetEmployeePayroll(employeeId, sessionService.UserId).Returns(viewModel);
            sessionService.HasPermissionOnCompany(viewModel.CompanyId, FunctionKey.EmployeeBankDetails, Policy.Host).Returns(true);

            // Act
            var actual = await employeeController.Payroll(employeeId, filter);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsType<EmployeePayrollViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Equal(filter, viewModelResult.CachingFilter);
        }

        [Fact]
        public async Task EditPayroll_ValidModel_ReturnExpectedCommandResult()
        {
            // Arrange
            var employeeId = 1;
            var model = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                BasicSalary = new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId },
                Allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId },
                Deduction = new EmployeePayrollDeductionModel { EmployeeId = employeeId }
            };
            var expected = new CommandResult { Code = 0 };
            var employeeController = GetEmployeeController();
            employeeController.Url = Substitute.For<IUrlHelper>();
            employeeController.Url.Action().ReturnsForAnyArgs("/Employee/Index");
            payrollService.UpdatePayroll(model, sessionService.UserId).Returns(expected);

            // Act
            var actual = await employeeController.EditPayroll(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected.IsSuccessful, jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Equal(expected.Message, jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
            Assert.Equal("/Employee/Index", jsonResult.Value.GetType().GetProperty("redirectTo").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditPayroll_InValidModel_ReturnErrors()
        {
            // Arrange
            var model = new EmployeePayrollViewModel();
            var employeeController = GetEmployeeController();
            employeeController.ModelState.AddModelError("employee", "error");

            // Act
            var actual = await employeeController.EditPayroll(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task CheckEmployeePayrollChangedInMonth_ExistChangeInMonth_ReturnTrue()
        {
            var employeeController = GetEmployeeController();
            baseService.GetDataAsync<int>(Arg.Any<CheckEmployeePayrollChangedInMonthCriteria>()).Returns(new List<int> { 1 });
            // Act
            var actual = await employeeController.CheckEmployeePayrollChangedInMonth(DateTime.Today, 1);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value);
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

        private static ICommonService FakeCommonService()
        {
            var service = Substitute.For<ICommonService>();

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
            commonService = FakeCommonService();
            employeeService = FakeEmployeeService();
            payrollService = FakeEmployeePayrollServicee();

            var controller = new EmployeeController(baseService, sessionService, commonService, employeeService, payrollService);

            return controller;
        }

        private Tuple<List<CompanyTree>, List<MultiSelectViewModel>, string> PrepareDataForGetDepartmentTest()
        {
            string departmentIds = "4";
            var expectedCompanyList = new List<CompanyTree>
            {
                new CompanyTree
                {
                    Id = 1,
                    Code = "DAR",
                    CategoryId = (int)CompanyTreeType.Company
                },
                new CompanyTree
                {
                    Id = 2,
                    Code = "GWG",
                    CategoryId = (int)CompanyTreeType.Company
                },
                new CompanyTree
                {
                    Id = 3,
                    Code = "SM",
                    CategoryId = (int)CompanyTreeType.Division,
                    ParentId = 1
                },
                new CompanyTree
                {
                    Id = 4,
                    Code = "NS",
                    CategoryId = (int)CompanyTreeType.Department,
                    ParentId = 3
                },
                new CompanyTree
                {
                    Id = 5,
                    Code = "GO",
                    CategoryId = (int)CompanyTreeType.Division,
                    ParentId = 2
                },
                new CompanyTree
                {
                    Id = 6,
                    Code = "OTH",
                    CategoryId = (int)CompanyTreeType.Department,
                    ParentId = 5
                }
            };

            var expected = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel
                {
                    Value = "1",
                    Text = "DAR",
                    Checked = true,
                    ParentId = "0",
                    Items = new List<MultiSelectViewModel>
                    {
                        new MultiSelectViewModel
                        {
                            Value = "3",
                            Text = "SM",
                            Checked = true,
                            ParentId = "1",
                            Items = new List<MultiSelectViewModel>
                            {
                                new MultiSelectViewModel
                                {
                                    Value = "4",
                                    Text = "NS",
                                    Checked = true,
                                    ParentId = "3"
                                }
                            }
                        }
                    }
                },
                new MultiSelectViewModel
                {
                    Value = "2",
                    Text = "GWG",
                    Checked = false,
                    ParentId = "0",
                    Items = new List<MultiSelectViewModel>
                    {
                        new MultiSelectViewModel
                        {
                            Value = "5",
                            Text = "GO",
                            Checked = false,
                            ParentId = "2",
                            Items = new List<MultiSelectViewModel>
                            {
                                new MultiSelectViewModel
                                {
                                    Value = "6",
                                    Text = "OTH",
                                    Checked = false,
                                    ParentId = "5"
                                }
                            }
                        }
                    }
                }
            };

            return new Tuple<List<CompanyTree>, List<MultiSelectViewModel>, string>(expectedCompanyList, expected, departmentIds);
        }

        private Employee MockEmployee()
        {
            return new Employee
            {
                EmployeeId = 12,
                FullName = "full",
                Nickname = "nick",
                Gender = 22,
                DateOfBirth = DateTime.Today,
                PlaceOfBirth = "VN",
                Nationality = 1,
                NRICNo = "1234",
                MaritalStatus = 3,
                NoOfChildren = 2,
                OverseasPhoneNo = "122234",
                LocalPhoneNo = "331234",
                OtherPhoneNo = "551234",
                PassportNo = "441234",
                DateIssue = DateTime.Today.AddDays(-2),
                ExpiredDate = DateTime.Today.AddDays(2),
                PlaceOfIssue = "UK",
                Skype = "zuck",
                PersonalEmail = "a@gnil.com",
                PermanentAddress = "123 address",
                ResidenceAddress = "4423 address",
                MailingAddress = "b@gmail.com",
                ProfilePic = "1.jpg",
                IsHost = false
            };
        }

        private static SelectList GetSelectList(IEnumerable<ReferenceResourceModel> refResource)
        {
            return new SelectList(refResource, "ResourceId", "Value");
        }

        private Tuple<SelectList, SelectList, SelectList, SelectList> MockTupleSelectList()
        {
            var refResources = new List<ReferenceResourceModel>
            {
                new ReferenceResourceModel {RefName = ReferenceResource.Gender, Value = "a", ResourceId = 1 },
                new ReferenceResourceModel {RefName = ReferenceResource.Nationality, Value = "b", ResourceId = 11 },
                new ReferenceResourceModel {RefName = ReferenceResource.MaritalStatus, Value = "c", ResourceId = 2 },
                new ReferenceResourceModel {RefName = ReferenceResource.NoOfChildren, Value = "d", ResourceId = 33 }
            };
            var genderSelectList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Gender)));
            var nationalSelectList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Nationality)));
            var maritalStatusSelectList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.MaritalStatus)));
            var noOfChildrenSelectList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.NoOfChildren)));

            return new Tuple<SelectList, SelectList, SelectList, SelectList>(genderSelectList, nationalSelectList, maritalStatusSelectList, noOfChildrenSelectList);
        }

        #endregion Helpers
    }
}