namespace HRP.Criteria.PaymentProcess
{
    using Fanex.Data.Repository;

    public class GetDataForSelectWorkingMonthCompanyCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "PaymentProcess_GetDataForSelectWorkingMonthCompany";

        public override bool IsValid() => UserId > 0;
    }
}