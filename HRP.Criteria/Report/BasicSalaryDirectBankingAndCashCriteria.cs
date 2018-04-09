namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class BasicSalaryDirectBankingAndCashCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "Report_GetBasicSalaryDirectBankingAndCash";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}