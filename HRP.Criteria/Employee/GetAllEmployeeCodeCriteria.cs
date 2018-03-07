namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetAllEmployeeCodeCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public override string GetSettingKey() => "Employment_GetAllEmployeeCode";

        public override bool IsValid() => CompanyId > 0;
    }
}