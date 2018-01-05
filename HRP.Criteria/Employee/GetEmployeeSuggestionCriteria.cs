namespace HRP.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeSuggestionCriteria : CriteriaBase
    {
        public int? CompanyId { get; set; }

        public string IDNo { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(IDNo) && IDNo.Length > 1;

        public override string GetSettingKey() => "Employee_GetSuggestion";
    }
}