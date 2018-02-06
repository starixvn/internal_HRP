namespace HRP.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetAllSectionCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Section_GetAll";

        public override bool IsValid() => true;
    }
}