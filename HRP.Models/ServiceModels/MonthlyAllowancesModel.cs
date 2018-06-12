namespace HRP.Models.ServiceModels
{
    using System;
    using System.Collections.Generic;

    public class MonthlyAllowancesModel
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public MonthlyAllowanceModel Allowance { get; set; } = new MonthlyAllowanceModel();

        public MonthlyPayrollBasicAllowanceModel BasicAllowance { get; set; } = new MonthlyPayrollBasicAllowanceModel();

        public IList<MonthlyAllowanceAdditionModel> Additions { get; set; } = new List<MonthlyAllowanceAdditionModel>();

        public IList<MonthlyAllowanceDeductionModel> Deductions { get; set; } = new List<MonthlyAllowanceDeductionModel>();
    }
}