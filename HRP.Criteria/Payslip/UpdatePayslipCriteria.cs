namespace HRP.Criteria.Payslip
{
    public class UpdatePayslipCriteria : BaseNonQueryCommand
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string PayrollIds { get; set; }

        public override string GetSettingKey() => "Payslip_Update";

        public override bool IsValid() => CompanyId > 0 && UserId > 0 && !string.IsNullOrWhiteSpace(PayrollIds);
    }
}