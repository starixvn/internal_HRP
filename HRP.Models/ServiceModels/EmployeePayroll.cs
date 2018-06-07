namespace HRP.Models.ServiceModels
{
    public class EmployeePayroll
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Code { get; set; }

        public int CompanyId { get; set; }

        public EmployeePayrollBasicSalary BasicSalary { get; set; }

        public EmployeePayrollAllowance Allowance { get; set; }

        public EmployeePayrollDeduction Deduction { get; set; }
    }
}
