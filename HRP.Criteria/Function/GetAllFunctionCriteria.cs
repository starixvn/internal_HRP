namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllFunctionCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Function_GetAll";
    }
}