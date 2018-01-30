namespace HRP.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetAllDepartmentCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Department_GetAll";

        public override bool IsValid() => true;
    }
}