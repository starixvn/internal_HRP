namespace HRP.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class GetDataForDynamicReportCriteria : CriteriaBase
    {
        public int CompanyId { get; set; }
        public DateTime Month { get; set; }
        public bool? IsHasIDNo { get; set; }
        public bool? IsHasFullName { get; set; }
        public bool? IsHasNickName { get; set; }
        public bool? IsHasGender { get; set; }
        public bool? IsHasDateOfBirth { get; set; }
        public bool? IsHasPlaceOfBirth { get; set; }
        public bool? IsHasNationality { get; set; }

        public override string GetSettingKey() => "Report_GetDataForDynamicReport";

        public override bool IsValid() => CompanyId > 0;
    }
}