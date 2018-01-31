namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class CheckDivisionExistsCriteria : CriteriaBase
    {
        public int DivisionId { get; set; }

        public int CompanyId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "Division_CheckExists";

        public override bool IsValid() => DivisionId >= 0 && CompanyId >= 0 && !string.IsNullOrWhiteSpace(Code);
    }
}