using Fanex.Data.Repository;

namespace HRP.Criteria.Employee
{
    public class GetEmployeeSuggestionByNameCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public override string GetSettingKey() => "Employee_GetSuggestionByName";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && Name.Length > 1;
    }
}