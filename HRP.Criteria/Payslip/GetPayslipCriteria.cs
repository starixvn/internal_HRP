namespace HRP.Criteria.Payslip
{
    using System;
    using Fanex.Data.Repository;

    public class GetPayslipCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public DateTime Month { get; set; }
        public int CompanyId { get; set; }
        public bool? IsSent { get; set; }

        public override bool IsValid() => UserId > 0 && CompanyId > 0;

        public override string GetSettingKey() => "Payslip_Get";
    }
}