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

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GenerateMonthlyPayroll";

#pragma warning disable S1067 // Expressions should not be too complex

        public override bool IsValid() => CompanyId > 0 && UpdatedBy > 0
            && !string.IsNullOrWhiteSpace(PayrollXMLDoc)
            && !string.IsNullOrWhiteSpace(BasicSalaryXMLDoc)
            && !string.IsNullOrWhiteSpace(SalaryXMLDoc);

#pragma warning restore S1067 // Expressions should not be too complex
    }
}