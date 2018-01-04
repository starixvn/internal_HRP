namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetSectionByDepartmentIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override bool IsValid() => DepartmentId > 0;

        public override string GetSettingKey() => "Section_GetByDepartmentId";
    }
}