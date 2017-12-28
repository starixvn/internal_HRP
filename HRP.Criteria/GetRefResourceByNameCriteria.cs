namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetRefResourceByNameCriteria : CriteriaBase
    {
        public string RefName { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(RefName);

        public override string GetSettingKey() => "RefResource_GetByName";
    }
}