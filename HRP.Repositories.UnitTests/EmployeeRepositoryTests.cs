namespace HRP.Repositories.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using NSubstitute;
    using Xunit;

    public class EmployeeRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public EmployeeRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region GetPersonalDetails

        [Fact]
        public void GetPersonalDetailsNotExistedEmployee_ReturnNull()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefault<CreateEmployeeViewModel>().Returns(x => null);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetPersonalDetails(criteria);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetPersonalDetails_EmployeeIsInvalid_ReturnNull()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            var employee = new CreateEmployeeViewModel();
            multiResultReader.ReadFirstOrDefault<CreateEmployeeViewModel>().Returns(employee);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetPersonalDetails(criteria);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetPersonalDetails_EmployeeIsValid_ReturnAsExpectedModel()
        {
            // Arrange
            var employee = new CreateEmployeeViewModel
            {
                EmployeeId = 1
            };
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefault<CreateEmployeeViewModel>().Returns(employee);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetPersonalDetails(criteria);

            // Assert
            Assert.IsType<CreateEmployeeViewModel>(actual);
            Assert.Equal(employee.EmployeeId, actual.EmployeeId);
            Assert.Equal(employee.FullName, actual.FullName);
            Assert.Equal(employee.Nickname, actual.Nickname);
            Assert.Equal(employee.Gender, actual.Gender);
            Assert.Equal(employee.DateOfBirth, actual.DateOfBirth);
            Assert.Equal(employee.PlaceOfBirth, actual.PlaceOfBirth);
            Assert.Equal(employee.Nationality, actual.Nationality);
            Assert.Equal(employee.NRICNo, actual.NRICNo);
            Assert.Equal(employee.MaritalStatus, actual.MaritalStatus);
            Assert.Equal(employee.NoOfChildren, actual.NoOfChildren);
            Assert.Equal(employee.OverseasPhoneNo, actual.OverseasPhoneNo);
            Assert.Equal(employee.LocalPhoneNo, actual.LocalPhoneNo);
            Assert.Equal(employee.OtherPhoneNo, actual.OtherPhoneNo);
            Assert.Equal(employee.PassportNo, actual.PassportNo);
            Assert.Equal(employee.DateIssue, actual.DateIssue);
            Assert.Equal(employee.ExpiredDate, actual.ExpiredDate);
            Assert.Equal(employee.PlaceOfIssue, actual.PlaceOfIssue);
            Assert.Equal(employee.Skype, actual.Skype);
            Assert.Equal(employee.PersonalEmail, actual.PersonalEmail);
            Assert.Equal(employee.PermanentAddress, actual.PermanentAddress);
            Assert.Equal(employee.ResidenceAddress, actual.ResidenceAddress);
            Assert.Equal(employee.MailingAddress, actual.MailingAddress);
            Assert.Equal(employee.ProfilePic, actual.ProfilePic);
            Assert.Equal(employee.ProfilePicture, actual.ProfilePicture);
            Assert.Equal(employee.IsHost, actual.IsHost);
            Assert.Null(actual.EmergencyContact);
            Assert.Empty(actual.GenderList);
            Assert.Empty(actual.NationalityList);
            Assert.Empty(actual.MaritalStatusList);
            Assert.Empty(actual.NoOfChildrenList);
        }

        #endregion GetPersonalDetails

        [Fact]
        public void GetDataToCreateEmployment_WhenCalled_ReturnAsExpectedModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            var companies = GetCompanyForTest();
            var jobCategories = GetJobCategoryForTest();
            var countries = GetCountryForTest();
            multiResultReader.Read<Company>().Returns(companies);
            multiResultReader.Read<JobCategory>().Returns(jobCategories);
            multiResultReader.Read<Country>().Returns(countries);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetDataToCreateEmployment(criteria);

            // Assert
            Assert.IsType<CreateEmploymentViewModel>(actual);
            Assert.Equal(0, actual.CompanyId);
            Assert.Equal(0, actual.LocalCompanyId);
            Assert.Null(actual.JobTitle);
            Assert.Equal(0, actual.LocalJobTitleId);
            Assert.Equal(DateTime.MinValue, actual.GroupStartDate);
            Assert.Null(actual.GroupEndDate);
            Assert.Null(actual.CompanyStartDate);
            Assert.Null(actual.CompanyEndDate);
            Assert.Null(actual.CompanyCode);
            Assert.Null(actual.EmployeeCode);
            Assert.Equal(0, actual.EmploymentType);
            Assert.Equal(0, actual.EmploymentStatus);
            Assert.Equal(0, actual.JobCategoryId);
            Assert.Equal(0, actual.JobGradeId);
            Assert.Equal(0, actual.GradeLevelId);
            Assert.Equal(0, actual.DivisionId);
            Assert.Equal(0, actual.DepartmentId);
            Assert.Equal(0, actual.CostCenterId);
            Assert.Equal(0, actual.SectionId);
            Assert.Equal(0, actual.CountryId);
            Assert.Equal(0, actual.CityId);
            Assert.Equal(0, actual.OfficeId);
            Assert.Null(actual.Email);
            Assert.Null(actual.DST);
            Assert.Null(actual.DBT);
            Assert.Null(actual.TaxIDNo);
            Assert.Null(actual.SSSNo);
            Assert.Null(actual.HealthInsNo);
            Assert.Null(actual.HDMFNo);
            Assert.Null(actual.FullNamePFileAdmin);
            Assert.Null(actual.NicknamePFileAdmin);
            Assert.Null(actual.PDFPassword);
            Assert.Equal(0, actual.WorkPermit.LocalCompanyId);
            Assert.Equal(0, actual.WorkPermit.Type1);
            Assert.Equal(DateTime.MinValue, actual.WorkPermit.StartDateType1);
            Assert.Equal(DateTime.MinValue, actual.WorkPermit.EndDateType1);
            Assert.Null(actual.WorkPermit.Type2);
            Assert.Null(actual.WorkPermit.StartDateType2);
            Assert.Null(actual.WorkPermit.EndDateType2);
            Assert.Null(actual.Paymaster.FullNameBVISalary);
            Assert.Null(actual.Paymaster.NicknameBVISalary);
            Assert.Null(actual.Paymaster.FullNameBVIAllowance);
            Assert.Null(actual.Paymaster.NicknameBVIAllowance);
            Assert.Null(actual.Paymaster.FullNameLocalSalary);
            Assert.Null(actual.Paymaster.NicknameLocalSalary);
            Assert.Equal(companies, actual.Companies);
            Assert.Equal(jobCategories, actual.JobCategories);
            Assert.Equal(countries, actual.Countries);
            Assert.Empty(actual.LocalCompanies);
            Assert.Empty(actual.LocalJobTitles);
            Assert.Empty(actual.EmploymentTypes);
            Assert.Empty(actual.EmploymentStatuses);
            Assert.Empty(actual.WorkPermitType1s);
            Assert.Empty(actual.WorkPermitType2s);
        }

        #region GetDataToUpdateEmployment

        [Fact]
        public void GetDataToUpdateEmployment_NotExistedEmployment_ReturnNull()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefault<UpdateEmploymentViewModel>().Returns(x => null);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetDataToUpdateEmployment(criteria);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetDataToUpdateEmployment_EmploymentIsInvalid_ReturnNull()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var repository = new EmployeeRepository();
            var employment = new UpdateEmploymentViewModel();
            multiResultReader.ReadFirstOrDefault<UpdateEmploymentViewModel>().Returns(employment);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetDataToUpdateEmployment(criteria);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void GetDataToUpdateEmployment_EmploymentIsValid_ReturnAsExpectedModel()
        {
            // Arrange
            var employment = new UpdateEmploymentViewModel
            {
                EmployeeId = 1
            };
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var companies = GetCompanyForTest();
            var jobCategories = GetJobCategoryForTest();
            var countries = GetCountryForTest();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefault<UpdateEmploymentViewModel>().Returns(employment);
            multiResultReader.Read<Company>().Returns(companies);
            multiResultReader.Read<JobCategory>().Returns(jobCategories);
            multiResultReader.Read<Country>().Returns(countries);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = repository.GetDataToUpdateEmployment(criteria);

            // Assert
            Assert.IsType<UpdateEmploymentViewModel>(actual);
            Assert.Equal(employment.EmployeeId, actual.EmployeeId);
            Assert.Equal(employment.CompanyId, actual.CompanyId);
            Assert.Equal(employment.OldCompanyId, actual.OldCompanyId);
            Assert.Equal(employment.LocalCompanyId, actual.LocalCompanyId);
            Assert.Equal(employment.OldLocalCompanyId, actual.OldLocalCompanyId);
            Assert.Equal(employment.JobTitle, actual.JobTitle);
            Assert.Equal(employment.OldJobTitle, actual.OldJobTitle);
            Assert.Equal(employment.LocalJobTitleId, actual.LocalJobTitleId);
            Assert.Equal(employment.OldLocalJobTitleId, actual.OldLocalJobTitleId);
            Assert.Equal(employment.GroupStartDate, actual.GroupStartDate);
            Assert.Equal(employment.OldGroupStartDate, actual.OldGroupStartDate);
            Assert.Equal(employment.GroupEndDate, actual.GroupEndDate);
            Assert.Equal(employment.OldGroupEndDate, actual.OldGroupEndDate);
            Assert.Equal(employment.CompanyStartDate, actual.CompanyStartDate);
            Assert.Equal(employment.OldCompanyStartDate, actual.OldCompanyStartDate);
            Assert.Equal(employment.CompanyEndDate, actual.CompanyEndDate);
            Assert.Equal(employment.OldCompanyEndDate, actual.OldCompanyEndDate);
            Assert.Equal(employment.CompanyCode, actual.CompanyCode);
            Assert.Equal(employment.EmployeeCode, actual.EmployeeCode);
            Assert.Equal(employment.OldEmployeeCode, actual.OldEmployeeCode);
            Assert.Equal(employment.EmploymentType, actual.EmploymentType);
            Assert.Equal(employment.EmploymentStatus, actual.EmploymentStatus);
            Assert.Equal(employment.OldEmploymentStatus, actual.OldEmploymentStatus);
            Assert.Equal(employment.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(employment.OldJobCategoryId, actual.OldJobCategoryId);
            Assert.Equal(employment.JobGradeId, actual.JobGradeId);
            Assert.Equal(employment.OldJobGradeId, actual.OldJobGradeId);
            Assert.Equal(employment.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(employment.OldGradeLevelId, actual.OldGradeLevelId);
            Assert.Equal(employment.DivisionId, actual.DivisionId);
            Assert.Equal(employment.OldDivisionId, actual.OldDivisionId);
            Assert.Equal(employment.DepartmentId, actual.DepartmentId);
            Assert.Equal(employment.OldDepartmentId, actual.OldDepartmentId);
            Assert.Equal(employment.CostCenterId, actual.CostCenterId);
            Assert.Equal(employment.OldCostCenterId, actual.OldCostCenterId);
            Assert.Equal(employment.SectionId, actual.SectionId);
            Assert.Equal(employment.OldSectionId, actual.OldSectionId);
            Assert.Equal(employment.CountryId, actual.CountryId);
            Assert.Equal(employment.CityId, actual.CityId);
            Assert.Equal(employment.OfficeId, actual.OfficeId);
            Assert.Equal(employment.Email, actual.Email);
            Assert.Equal(employment.DST, actual.DST);
            Assert.Equal(employment.DBT, actual.DBT);
            Assert.Equal(employment.TaxIDNo, actual.TaxIDNo);
            Assert.Equal(employment.SSSNo, actual.SSSNo);
            Assert.Equal(employment.HealthInsNo, actual.HealthInsNo);
            Assert.Equal(employment.HDMFNo, actual.HDMFNo);
            Assert.Equal(employment.FullNamePFileAdmin, actual.FullNamePFileAdmin);
            Assert.Equal(employment.NicknamePFileAdmin, actual.NicknamePFileAdmin);
            Assert.Equal(employment.PDFPassword, actual.PDFPassword);
            Assert.Null(actual.WorkPermit);
            Assert.Null(actual.Paymaster);
            Assert.Null(actual.EPA);
            Assert.Equal(companies, actual.Companies);
            Assert.Empty(actual.Divisions);
            Assert.Empty(actual.Departments);
            Assert.Empty(actual.CostCenters);
            Assert.Empty(actual.Sections);
            Assert.Equal(jobCategories, actual.JobCategories);
            Assert.Empty(actual.GradeLevels);
            Assert.Empty(actual.JobGrades);
            Assert.Equal(countries, actual.Countries);
            Assert.Empty(actual.Cities);
            Assert.Empty(actual.Offices);
            Assert.Empty(actual.LocalCompanies);
            Assert.Empty(actual.LocalJobTitles);
            Assert.Empty(actual.EmploymentTypes);
            Assert.Empty(actual.EmploymentStatuses);
            Assert.Empty(actual.WorkPermitType1s);
            Assert.Empty(actual.WorkPermitType2s);
            Assert.Empty(actual.CompanyChangeReasons);
            Assert.Empty(actual.LocalCompanyChangeReasons);
            Assert.Empty(actual.JobTitleChangeReasons);
            Assert.Empty(actual.LocalJobTitleChangeReasons);
            Assert.Empty(actual.GroupDateChangeReasons);
            Assert.Empty(actual.CompanyDateChangeReasons);
            Assert.Empty(actual.IDNoChangeReasons);
            Assert.Empty(actual.JobCategoryChangeReasons);
            Assert.Empty(actual.JobGradeChangeReasons);
            Assert.Empty(actual.DivisionChangeReasons);
            Assert.Empty(actual.DepartmentChangeReasons);
            Assert.Empty(actual.CostCenterChangeReasons);
            Assert.Empty(actual.SectionChangeReasons);
        }

        #endregion GetDataToUpdateEmployment

        [Fact]
        public void GetDataFilterForIndexPage_WhenCalled_ReturnAsExpectedModel()
        {
            // Arrange
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteia();
            var viewModel = new EmployeeManagementViewModel();
            var multiSelects = new List<MultiSelectViewModel>
            {
                new MultiSelectViewModel { Value = "1", Text = "DAR", Checked = true }
            };
            multiResultReader.ReadFirst<EmployeeManagementViewModel>().Returns(viewModel);
            multiResultReader.Read<MultiSelectViewModel>().Returns(multiSelects);
            objectDb.QueryMultiple(Arg.Any<ICriteria>()).Returns(multiResultReader);
            var repository = new EmployeeRepository();

            // Act
            var actual = repository.GetDataFilterForIndexPage(criteria);

            // Assert
            Assert.IsType<EmployeeManagementViewModel>(actual);
            Assert.Equal(viewModel.IsHostCreateEmployee, actual.IsHostCreateEmployee);
            Assert.Equal(viewModel.DepartmentIds, actual.DepartmentIds);
            Assert.Equal(viewModel.EmploymentStatusIds, actual.EmploymentStatusIds);
            Assert.Equal(viewModel.EmployeeId, actual.EmployeeId);
            Assert.Equal(viewModel.DisplayName, actual.DisplayName);
            Assert.Single(actual.HierarchyCompanies);
            Assert.Equal(multiSelects.First().Value, actual.HierarchyCompanies.First().Value);
            Assert.Equal(multiSelects.First().Text, actual.HierarchyCompanies.First().Text);
            Assert.Equal(multiSelects.First().Checked, actual.HierarchyCompanies.First().Checked);
            Assert.Equal(multiSelects.First().ParentId, actual.HierarchyCompanies.First().ParentId);
            Assert.Empty(actual.HierarchyCompanies.First().Items);
            Assert.Equal(multiSelects, actual.EmploymentStatuses);
            Assert.Equal(multiSelects.First().Value, actual.EmploymentStatuses.First().Value);
            Assert.Equal(multiSelects.First().Text, actual.EmploymentStatuses.First().Text);
            Assert.Equal(multiSelects.First().Checked, actual.EmploymentStatuses.First().Checked);
            Assert.Equal(multiSelects.First().ParentId, actual.EmploymentStatuses.First().ParentId);
        }

        #region Helpers

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteia()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("HRP_Test");

            return criteria;
        }

        private static IEnumerable<Company> GetCompanyForTest()
        {
            var companies = new List<Company>
            {
                new Company { CompanyId = 1, Code = "DAR", Name = "DAR", IsActive = true }
            };

            return companies;
        }

        private static IEnumerable<JobCategory> GetJobCategoryForTest()
        {
            var jobCategories = new List<JobCategory>
            {
                new JobCategory { JobCategoryId = 1, Name = "DAR", Description = "DAR", IsActive = true }
            };

            return jobCategories;
        }

        private static IEnumerable<Country> GetCountryForTest()
        {
            var countries = new List<Country>
            {
                new Country { CountryId = 1, Name = "DAR", Code = "DAR", IsActive = true }
            };

            return countries;
        }

        #endregion Helpers
    }
}