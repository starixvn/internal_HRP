namespace HRP.Criteria.Office
{
    using Fanex.Data.Repository;

    public class GetAllOfficeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Office_GetAll";
    }
}