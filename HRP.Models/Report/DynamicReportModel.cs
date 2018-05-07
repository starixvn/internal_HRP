namespace HRP.Models.Report
{
    using System;
    using HRP.Core.Enumerations;

    public class DynamicReportModel
    {
        public int EmployeeId { get; set; }

        #region Personal Details

        public string FullName { get; set; }

        public string Nickname { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PlaceOfBirth { get; set; }

        public string Nationality { get; set; }

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

        public string EmergencyContactName { get; set; }

        public string EmergencyContactRelationship { get; set; }

        public string EmergencyContactAddress { get; set; }

        public string EmergencyContactPhoneNo1 { get; set; }

        public string EmergencyContactPhoneNo2 { get; set; }

        #endregion Personal Details

        #region Employment

        public string Company { get; set; }

        public string CompanyChangeDate { get; set; }

        public string CompanyReason { get; set; }

        public string LocalCompany { get; set; }

        public string LocalCompanyChangeDate { get; set; }

        public string LocalCompanyReason { get; set; }

        public string Department { get; set; }

        public string Section { get; set; }

        public string Division { get; set; }

        public string CostCenter { get; set; }

        #endregion Employment

        public string Salary { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public int? EbankingCurrencyId { get; set; }

        public string EbankingAmount { get; set; }

        public int? EbankingAccountId { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1Amount { get; set; }

        public int? DirectBanking1AccountId { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2Amount { get; set; }

        public int? DirectBanking2AccountId { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashAmount { get; set; }

        public int? CashAccountId { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public int? AllowanceCurrencyId { get; set; }

        public int? AllowanceDirectBanking1CurrencyId { get; set; }

        public string AllowanceDirectBanking1Amount { get; set; }

        public int? AllowanceDirectBanking1AccountId { get; set; }

        public int? AllowanceDirectBanking2CurrencyId { get; set; }

        public string AllowanceDirectBanking2Amount { get; set; }

        public int? AllowanceDirectBanking2AccountId { get; set; }

        public int? AllowanceCashCurrencyId { get; set; }

        public string AllowanceCashAmount { get; set; }

        public int? AllowanceCashAccountId { get; set; }

        public PayrollAutoCalculateOn AllowanceAutoCalculateOn { get; set; }

        public string SubsistenceAmount { get; set; }

        public string SubsistenceCurrency { get; set; }

        public string HousingAmount { get; set; }

        public string HousingCurrency { get; set; }

        public string HomePassageAmount { get; set; }

        public string HomePassageCurrency { get; set; }

        public string DutyMealAmount { get; set; }

        public string DutyMealCurrency { get; set; }

        public string HPIAmount { get; set; }

        public string HPICurrency { get; set; }

        public string MobilePhoneAmount { get; set; }

        public string MobilePhoneCurrency { get; set; }

        public string OthersAmount { get; set; }

        public string OthersCurrency { get; set; }

        public string SecondmentAmount { get; set; }

        public string SecondmentCurrency { get; set; }
    }
}