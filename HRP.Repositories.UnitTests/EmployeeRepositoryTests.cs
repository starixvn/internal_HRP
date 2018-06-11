namespace HRP.Repositories.UnitTests
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Employee;
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

        #region GetEmploymentByEmployeeIdAsync

        [Fact]
        public async Task GetEmploymentByEmployeeIdAsync_EmploymentIsNotExisted_ReturnNull()
        {
            // Arrange
            Employment employment = null;
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefaultAsync<Employment>().Returns(employment);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = await repository.GetEmploymentByEmployeeIdAsync(criteria);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task GetEmploymentByEmployeeIdAsync_EmploymentIsExisted_ReturnAsExpectedModel()
        {
            // Arrange
            var employment = new Employment();
            var workPermit = new WorkPermit();
            var paymaster = new Paymaster();
            var epa = new EPA();
            var multiResultReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var repository = new EmployeeRepository();
            multiResultReader.ReadFirstOrDefaultAsync<Employment>().Returns(employment);
            multiResultReader.ReadFirstOrDefaultAsync<WorkPermit>().Returns(workPermit);
            multiResultReader.ReadFirstOrDefaultAsync<Paymaster>().Returns(paymaster);
            multiResultReader.ReadFirstOrDefaultAsync<EPA>().Returns(epa);
            objectDb.QueryMultipleAsync(Arg.Any<ICriteria>()).Returns(multiResultReader);

            // Act
            var actual = await repository.GetEmploymentByEmployeeIdAsync(criteria);

            // Assert
            Assert.Equal(employment.CompanyId, actual.CompanyId);
            Assert.Equal(employment.LocalCompanyId, actual.LocalCompanyId);
            Assert.Equal(employment.JobTitle, actual.JobTitle);
            Assert.Equal(employment.LocalJobTitleId, actual.LocalJobTitleId);
            Assert.Equal(employment.GroupStartDate, actual.GroupStartDate);
            Assert.Equal(employment.GroupEndDate, actual.GroupEndDate);
            Assert.Equal(employment.CompanyStartDate, actual.CompanyStartDate);
            Assert.Equal(employment.CompanyEndDate, actual.CompanyEndDate);
            Assert.Equal(employment.CompanyCode, actual.CompanyCode);
            Assert.Equal(employment.EmployeeCode, actual.EmployeeCode);
            Assert.Equal(employment.EmploymentType, actual.EmploymentType);
            Assert.Equal(employment.EmploymentStatus, actual.EmploymentStatus);
            Assert.Equal(employment.JobCategoryId, actual.JobCategoryId);
            Assert.Equal(employment.JobGradeId, actual.JobGradeId);
            Assert.Equal(employment.GradeLevelId, actual.GradeLevelId);
            Assert.Equal(employment.DivisionId, actual.DivisionId);
            Assert.Equal(employment.DepartmentId, actual.DepartmentId);
            Assert.Equal(employment.CostCenterId, actual.CostCenterId);
            Assert.Equal(employment.SectionId, actual.SectionId);
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
            Assert.Equal(workPermit, actual.WorkPermit);
            Assert.Equal(paymaster, actual.Paymaster);
            Assert.Equal(epa, actual.EPA);
        }

        #endregion GetEmploymentByEmployeeIdAsync

        #region Helpers

        private static IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        private static ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("HRP_Test");

            return criteria;
        }

        #endregion Helpers
    }
}