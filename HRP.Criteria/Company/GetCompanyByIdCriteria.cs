namespace HRP.Criteria.Company
{
    using Fanex.Data.Repository;

    public class GetCompanyByIdCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public override bool IsValid() => CompanyId > 0;

        public override string GetSettingKey() => "DataConfiguration_GetCompanyBVIById";
    }
}