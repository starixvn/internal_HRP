namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class CreateEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string PictureExtension { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Employee_CreateEmployee";

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(NickName);
        }
    }
}