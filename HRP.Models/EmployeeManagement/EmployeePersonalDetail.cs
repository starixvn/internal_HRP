namespace HRP.Models.EmployeeManagement
{
    using System;

    public class EmployeePersonalDetail
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

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

        public string ProfilePic { get; set; }

        public string ECName { get; set; }

        public string ECRelationship { get; set; }

        public string ECAddress { get; set; }

        public string ECTelephoneNo { get; set; }
    }
}