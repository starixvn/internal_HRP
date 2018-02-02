namespace HRP.Criteria.UnitTests.Employee
{
    using Criteria.Employee;
    using Xunit;

    public class UpdateEmploymentCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateEmploymentCriteria();
            var expected = "Employment_Update";

            // Act
            var actual = criteria.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ParamsAreInvalid_ReturnFalse()
        {
            // Arrange
            var criteria = new UpdateEmploymentCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateEmploymentCriteria
            {
                EmployeeId = 1,
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
                UpdatedBy = 1
            };

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}