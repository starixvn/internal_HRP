namespace HRP.Models.MonthlyPayroll
{
    using System.Collections.Generic;

    public class MonthlyAllowancesViewModel
    {
        public MonthlyAllowanceModel Allowance { get; set; }

        public IEnumerable<MonthlyAllowanceAdditionModel> Additions { get; set; }

        public IEnumerable<MonthlyAllowanceDeductionModel> Deductions { get; set; }
    }
}