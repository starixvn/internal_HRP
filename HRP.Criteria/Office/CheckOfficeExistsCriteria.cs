namespace HRP.Criteria.Office
{
    using Fanex.Data.Repository;

    public class CheckOfficeExistsCriteria : CriteriaBase
    {
        public int OfficeId { get; set; }

        public int CityId { get; set; }

        public string Code { get; set; }

        public override string GetSettingKey() => "Office_CheckExists";

        public override bool IsValid() => CityId >= 0 && OfficeId >= 0 && !string.IsNullOrWhiteSpace(Code);
    }
}