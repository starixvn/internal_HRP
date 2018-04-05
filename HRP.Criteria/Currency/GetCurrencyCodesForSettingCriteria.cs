namespace HRP.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetCurrencyCodesForSettingCriteria : CriteriaBase
    {
        public DateTime Month { get; set; }

        public override string GetSettingKey() => "Currency_GetCurrencyCodesForSetting";

        public override bool IsValid() => true;
    }
}
