namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetDivisionByCompanyIdCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => CompanyId > 0;

        public override string GetSettingKey() => "Division_GetByCompanyId";
    }
}