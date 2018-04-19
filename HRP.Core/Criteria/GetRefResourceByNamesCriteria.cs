namespace HRP.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetRefResourceByNamesCriteria : CriteriaBase
    {
        public string RefNames { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => !string.IsNullOrEmpty(RefNames);

        public override string GetSettingKey() => "RefResource_GetByNames";
    }
}