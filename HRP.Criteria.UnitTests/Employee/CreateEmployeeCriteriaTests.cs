namespace HRP.Criteria.UnitTests
{
    using HRP.Criteria.Employee;
    using Xunit;

    public class CreateEmployeeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateEmployeeCriteria();
            var expected = "Employee_CreateEmployee";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreInvalid_ReturnFalse()
        {
            // Arrange
            var criteria = new CreateEmployeeCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateEmployeeCriteria
            {
                FullName = "thang",
                Nickname = "tim.trinh",
                Gender = 1,
                Nationality = 1,
                NRICNo = "a",
                MaritalStatus = 1,
                NoOfChildren = 1,
                OverseasPhoneNo = "0123",
                PermanentAddress = "test@gmail.com",
                ResidenceAddress = "test@gmail.com",
                CompanyId = 1,
                LocalCompanyId = 1,
                JobTitle = "SM",
                LocalJobTitleId = 1,
                CompanyCode = "DAR",
                EmployeeCode = "111",
                EmploymentType = 1,
                EmploymentStatus = 1,
                JobCategoryId = 1,
                JobGradeId = 1,
                GradeLevelId = 1,
                DivisionId = 1,
                DepartmentId = 1,
                SectionId = 1,
                CostCenterId = 1,
                Email = "test@gmail.com",
                CountryId = 1,
                CityId = 1,
                OfficeId = 1,
                WorkPermitCompanyId = 1,
                Type1 = 1,
                FullNameBVISalary = "tim",
                FullNameBVIAllowance = "tim",
                FullNameAdministrator = "tim",
                FullNamePFileAdmin = "tim",
                PDFPassword = "asdsd",
                CreatedBy = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}