namespace HRP.Models.Payslip
{
    using System.Collections.Generic;

    public class SendPayslipViewModel
    {
        public int CompanyId { get; set; }
        public IEnumerable<int> PayrollIds { get; set; } = new List<int>();
    }
}