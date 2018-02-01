namespace HRP.Criteria.Section
{
    using Fanex.Data.Repository;

    public class CheckSectionExistsCriteria : CriteriaBase
    {
        public int SectionId { get; set; }

        public int CostCenterId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "Section_CheckExists";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Code);
    }
}