namespace HRP.Criteria.Division
{
    using Fanex.Data.Repository;

    public class CheckDivisionExistsCriteria : CriteriaBase
    {
        public int DivisionId { get; set; }

        public int CompanyId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "Division_CheckExists";

        public override bool IsValid() => !string.IsNullOrEmpty(Code);
    }
}