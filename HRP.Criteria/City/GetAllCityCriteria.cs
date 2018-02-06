namespace HRP.Criteria.City
{
    using Fanex.Data.Repository;

    public class GetAllCityCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "City_GetAll";
    }
}