namespace HRP.Criteria.Division
{
    using Fanex.Data.Repository;

    public class GetAllDivisionCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Division_GetAll";
    }
}