namespace HRP.Criteria.EmergencyContact
{
    using Fanex.Data.Repository;

    public class GetEmergencyContactByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "EmergencyContact_GetByEmployeeId";

        public override bool IsValid() => EmployeeId > 0;
    }
}