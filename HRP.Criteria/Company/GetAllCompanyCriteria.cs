namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllCompanyCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Company_GetAll";
    }
}