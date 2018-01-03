namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class CheckEmpCodeExistedCriteria : CriteriaBase
    {
        public string CompanyCode { get; set; }

        public string EmployeeCode { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(CompanyCode) && !string.IsNullOrEmpty(EmployeeCode);

        public override string GetSettingKey() => "Employee_CheckEmpCodeExisted";
    }
}