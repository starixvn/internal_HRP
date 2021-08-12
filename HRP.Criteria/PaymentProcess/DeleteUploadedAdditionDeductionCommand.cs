namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class DeleteUploadedAdditionDeductionCommand : BaseNonQueryCommand
    {
        public int CompanyId { get; set; }
        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_DeleteUploadedAdditionDeduction";

        public override bool IsValid() => CompanyId > 0;
    }
}