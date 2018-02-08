namespace HRP.Criteria.RefResource
{
    using Fanex.Data.Repository;

    public class GetRefResourceByNameAndIdCriteria : CriteriaBase
    {
        public int ResourceId { get; set; }

        public string RefName { get; set; }

        public override bool IsValid() => ResourceId > 0 && !string.IsNullOrWhiteSpace(RefName);

        public override string GetSettingKey() => "RefResource_GetByNameAndId";
    }
}