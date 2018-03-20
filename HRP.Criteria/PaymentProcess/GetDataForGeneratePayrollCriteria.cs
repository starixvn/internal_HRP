namespace HRP.Criteria.PaymentProcess
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataForGeneratePayrollCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GetDataForGenerateMonthlyPayroll";

        public override bool IsValid() => CompanyId > 0;
    }
}