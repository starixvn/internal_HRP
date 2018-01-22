namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetDepartmentByDivisionIdCriteria : CriteriaBase
    {
        public int DivisionId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => DivisionId > 0;

        public override string GetSettingKey() => "Department_GetByDivisionId";
    }
}