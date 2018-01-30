namespace HRP.Criteria.Division
{
    using Fanex.Data.Repository;

    public class GetDivisionByIdCriteria : CriteriaBase
    {
        public int DivisionId { get; set; }

        public override string GetSettingKey() => "Division_GetById";

        public override bool IsValid() => DivisionId > 0;
    }
}