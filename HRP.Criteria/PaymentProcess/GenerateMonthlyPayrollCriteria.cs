namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class GenerateMonthlyPayrollCriteria : CommandBaseCriteria
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public string PayrollXMLDoc { get; set; }

        public string BasicSalaryXMLDoc { get; set; }
        public string SalaryXMLDoc { get; set; }
        public string SalaryAdditionXMLDoc { get; set; }
        public string SalaryDeductionXMLDoc { get; set; }

        public string BasicAllowanceXMLDoc { get; set; }
        public string AllowanceXMLDoc { get; set; }
        public string AllowanceAdditionXMLDoc { get; set; }
        public string AllowanceDeductionXMLDoc { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GenerateMonthlyPayroll";

#pragma warning disable S1067 // Expressions should not be too complex

#pragma warning disable S1541 // Methods and properties should not be too complex

        public override bool IsValid() => CompanyId > 0 && UpdatedBy > 0
#pragma warning restore S1541 // Methods and properties should not be too complex
            && !string.IsNullOrWhiteSpace(PayrollXMLDoc)
            && !string.IsNullOrWhiteSpace(BasicSalaryXMLDoc)
            && !string.IsNullOrWhiteSpace(SalaryXMLDoc)
            && !string.IsNullOrWhiteSpace(BasicAllowanceXMLDoc)
            && !string.IsNullOrWhiteSpace(AllowanceXMLDoc);

#pragma warning restore S1067 // Expressions should not be too complex
    }
}