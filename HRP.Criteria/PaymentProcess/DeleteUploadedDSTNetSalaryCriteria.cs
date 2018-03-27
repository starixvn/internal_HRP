namespace HRP.Criteria.PaymentProcess
{
    using System;

    public class DeleteUploadedDSTNetSalaryCriteria : CommandBaseCriteria
    {
        public int CompanyId { get; set; }

        public DateTime Month { get; set; }

        public override string GetSettingKey() => "PaymentProcess_DeleteUploadedDSTNetSalary";

        public override bool IsValid() => CompanyId > 0;
    }
}