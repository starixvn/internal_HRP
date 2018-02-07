namespace HRP.Criteria.Office
{
    using Fanex.Data.Repository;

    public class GetOfficeByIdCriteria : CriteriaBase
    {
        public int OfficeId { get; set; }

        public override string GetSettingKey() => "Office_GetById";

        public override bool IsValid() => OfficeId > 0;
    }
}