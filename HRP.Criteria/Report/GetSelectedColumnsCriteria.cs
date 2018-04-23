namespace HRP.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetSelectedColumnsCriteria : CriteriaBase
    {
        public string SelectedIds { get; set; }

        public override string GetSettingKey() => "Report_GetSelectedColumns";

        public override bool IsValid() => !string.IsNullOrEmpty(SelectedIds);
    }
}
