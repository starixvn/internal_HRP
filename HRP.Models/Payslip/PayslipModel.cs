namespace HRP.Models.Payslip
{
    using HRP.Core.Enumerations;

    public class PayslipModel
    {
        public int PayrollId { get; set; }
        public string IDNo { get; set; }
        public string FullName { get; set; }
        public string Nickname { get; set; }
        public string Section { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsSent { get; set; }
        public MonthlyPayrollStatus PayrollStatus { get; set; }
    }
}