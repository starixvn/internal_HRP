namespace HRP.Models.Report
{
    using System;

    public class DynamicReportModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string MaritalStatus { get; set; }

        public string NoOfChildren { get; set; }

        public string Nationality { get; set; }

        public string NRICNo { get; set; }

        public string PassportNo { get; set; }

        public DateTime DateIssue { get; set; }

        public DateTime ExpiredDate { get; set; }

        public string PlaceOfIssue { get; set; }

        public string OverseasPhoneNo { get; set; }

        public string LocalPhoneNo { get; set; }

        public string OtherPhoneNo { get; set; }

        public string Skype { get; set; }

        public string PersonalEmail { get; set; }

        public string PermanentAddress { get; set; }

        public string ResidenceAddress { get; set; }

        public string MailingAddress { get; set; }
    }
}