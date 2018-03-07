namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetAllEmployeeCodeByCompanyIdCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public override string GetSettingKey() => "Employment_GetAllEmployeeCodeByCompanyId";

        public override bool IsValid() => CompanyId > 0;
    }
}