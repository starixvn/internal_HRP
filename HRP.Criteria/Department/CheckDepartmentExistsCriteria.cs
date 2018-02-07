namespace HRP.Criteria.Department
{
    using Fanex.Data.Repository;

    public class CheckDepartmentExistsCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int DivisionId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "Department_CheckExists";

        public override bool IsValid() => DepartmentId >= 0 && DivisionId >= 0 && !string.IsNullOrWhiteSpace(Code);
    }
}