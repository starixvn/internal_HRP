namespace HRP.Models.MonthlyPayroll
{
    using System.Collections.Generic;

    public class MonthlyAllowancesViewModel
    {
        public int CompanyId { get; set; }

        public MonthlyAllowanceModel Allowance { get; set; }

        public IEnumerable<MonthlyAllowanceAdditionModel> Additions { get; set; }

        public IEnumerable<MonthlyAllowanceDeductionModel> Deductions { get; set; }

        public bool CanEdit { get; set; }
    }
}