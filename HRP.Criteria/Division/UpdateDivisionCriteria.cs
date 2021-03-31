namespace HRP.Criteria
{
    public class UpdateDivisionCriteria : BaseNonQueryCommand
    {
        public int DivisionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Division_Update";

        public override bool IsValid() => DivisionId > 0 && !string.IsNullOrWhiteSpace(Code) && UpdatedBy > 0;
    }
}