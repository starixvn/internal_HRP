namespace HRP.Models.ServiceModels
{
    public class EmployeePayrollModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Code { get; set; }

        public int CompanyId { get; set; }

        public EmployeePayrollBasicSalaryModel BasicSalary { get; set; }

        public EmployeePayrollAllowanceModel Allowance { get; set; }

        public EmployeePayrollDeductionModel Deduction { get; set; }

        public bool CanRead { get; set; }

        public bool CanUpdate { get; set; }
    }
}
