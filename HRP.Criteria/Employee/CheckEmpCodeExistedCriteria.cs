namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class CheckEmpCodeExistedCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string CompanyCode { get; set; }

        public string EmployeeCode { get; set; }

        public override bool IsValid() => EmployeeId > 0 && !string.IsNullOrEmpty(CompanyCode) && !string.IsNullOrEmpty(EmployeeCode);

        public override string GetSettingKey() => "Employee_CheckEmpCodeExisted";
    }
}