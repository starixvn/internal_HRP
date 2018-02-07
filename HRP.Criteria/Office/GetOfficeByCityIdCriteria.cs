namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetOfficeByCityIdCriteria : CriteriaBase
    {
        public int CityId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => CityId > 0;

        public override string GetSettingKey() => "Office_GetByCityId";
    }
}