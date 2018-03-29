namespace HRP.Criteria.Payslip
{
    using Fanex.Data.Repository;

    public class GetPayslipDetailsCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string PayrollIds { get; set; }

        public override bool IsValid() => UserId > 0;

        public override string GetSettingKey() => "Payslip_GetDetails";
    }
}