namespace HRP.Criteria.Employee
{
    using System;

    public class CreateEmployeeCriteria : EmploymentBaseCriteria
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public int Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public int Nationality { get; set; }

        public string NRICNo { get; set; }

        public int MaritalStatus { get; set; }

        public int NoOfChildren { get; set; }

        public string OverseasPhoneNo { get; set; }

        public string LocalPhoneNo { get; set; }

        public string OtherPhoneNo { get; set; }

        public string PassportNo { get; set; }

        public DateTime DateIssue { get; set; }

        public DateTime ExpiredDate { get; set; }

        public string PlaceOfIssue { get; set; }

        public string Skype { get; set; }

        public string PersonalEmail { get; set; }

        public string PermanentAddress { get; set; }

        public string ResidenceAddress { get; set; }

        public string MailingAddress { get; set; }

        public string PictureExtension { get; set; }

        public string ECName { get; set; }

        public string ECRelationship { get; set; }

        public string ECAddress { get; set; }

        public string ECPhoneNo1 { get; set; }

        public string ECPhoneNo2 { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Employee_CreateEmployee";

#pragma warning disable S1541 // Methods and properties should not be too complex
        public override bool IsValid() =>
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
            !string.IsNullOrEmpty(FullName)
            && !string.IsNullOrEmpty(Nickname)
            && Gender > 0
            && Nationality > 0
            && !string.IsNullOrWhiteSpace(NRICNo)
            && MaritalStatus > 0
            && NoOfChildren > 0
            && !string.IsNullOrWhiteSpace(OverseasPhoneNo)
            && !string.IsNullOrWhiteSpace(PermanentAddress)
            && !string.IsNullOrWhiteSpace(ResidenceAddress)
            && IsValidParams()
            && CreatedBy > 0;
#pragma warning restore S1067 // Expressions should not be too complex
    }
}