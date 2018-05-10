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

        public string PersonalEmail { get; set; }

        public string MaritalStatus { get; set; }

        public string NoOfChildren { get; set; }

        public string LocalPhoneNo { get; set; }

        public string OverseasPhoneNo { get; set; }

        public string PassportNo { get; set; }

        public string OtherPhoneNo { get; set; }

        public DateTime ExpiredDate { get; set; }

        public DateTime DateIssue { get; set; }

        public string PlaceOfIssue { get; set; }

        public string Skype { get; set; }

        public string ResidenceAddress { get; set; }

        public string PermanentAddress { get; set; }

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

        public string JobTitle { get; set; }

        public DateTime? JobTitleChangeDate { get; set; }

        public string JobTitleReason { get; set; }

        public string LocalJobTitle { get; set; }

        public DateTime? LocalJobTitleChangeDate { get; set; }

        public string LocalJobTitleReason { get; set; }

        public DateTime? GroupStartDate { get; set; }

        public DateTime? GroupEndDate { get; set; }

        public string GroupDateReason { get; set; }

        public DateTime? CompanyStartDate { get; set; }

        public DateTime? CompanyEndDate { get; set; }

        public string CompanyDateReason { get; set; }

        public string GroupLengthOfService
        {
            get
            {
                if (GroupStartDate.HasValue)
                {
                    var endDate = GroupEndDate ?? DateTime.Today;
                    var totalDays = (endDate - GroupStartDate.Value).TotalDays;

                    return $"{totalDays} day(s)";
                }

                return string.Empty;
            }
        }

        public string CompanyLengthOfService
        {
            get
            {
                if (CompanyStartDate.HasValue)
                {
                    var endDate = CompanyEndDate ?? DateTime.Today;
                    var totalDays = (endDate - CompanyStartDate.Value).TotalDays;

                    return $"{totalDays} day(s)";
                }

                return string.Empty;
            }
        }

        public string IDNo { get; set; }

        public DateTime IDNoChangeDate { get; set; }

        public string IDNoChangeReason { get; set; }

        public string EmploymentType { get; set; }

        public string EmploymentStatus { get; set; }

        public DateTime EmploymentStatusChangeDate { get; set; }

        public string JobCategory { get; set; }

        public DateTime JobCategoryChangeDate { get; set; }

        public string JobCategoryReason { get; set; }

        public string GradeLevel { get; set; }

        public string JobGrade { get; set; }

        public DateTime JobGradeChangeDate { get; set; }

        public string JobGradeReason { get; set; }

        public string Department { get; set; }

        public DateTime DepartmentChangeDate { get; set; }

        public string DepartmentReason { get; set; }

        public string Section { get; set; }

        public DateTime SectionChangeDate { get; set; }

        public string SectionReason { get; set; }

        public string Division { get; set; }

        public DateTime DivisionChangeDate { get; set; }

        public string DivisionReason { get; set; }

        public string CostCenter { get; set; }

        public DateTime CostCenterChangeDate { get; set; }

        public string CostCenterReason { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Office { get; set; }

        public string WorkPermitCompany { get; set; }

        public string WorkPermitType1 { get; set; }

        public DateTime StartDateType1 { get; set; }

        public DateTime EndDateType1 { get; set; }

        public string WorkPermitType2 { get; set; }

        public DateTime StartDateType2 { get; set; }

        public DateTime EndDateType2 { get; set; }

        public string DSTCompany { get; set; }

        public string DBTCompany { get; set; }

        public string TaxIDNo { get; set; }

        public string SSSNo { get; set; }

        public string HealthInsNo { get; set; }

        public string HDMFNo { get; set; }

        public string FullNamePFileAdmin { get; set; }

        public string NicknamePFileAdmin { get; set; }

        public string FullNameBVISalary { get; set; }

        public string NickNameBVISalary { get; set; }

        public string FullNameBVIAllowance { get; set; }

        public string NickNameBVIAllowance { get; set; }

        public string FullNameLocalSalary { get; set; }

        public string NickNameLocalSalary { get; set; }

        public string FullNameAdministrator { get; set; }

        public string NicknameAdministrator { get; set; }

        public string FullNameEvaluator1 { get; set; }

        public string NicknameEvaluator1 { get; set; }

        public string FullNameEvaluator2 { get; set; }

        public string NicknameEvaluator2 { get; set; }

        public string FullNameReviewer { get; set; }

        public string NicknameReviewer { get; set; }

        public string FullNameManagement { get; set; }

        public string NicknameManagement { get; set; }

        public string FullNameIncrement { get; set; }

        public string NicknameIncrement { get; set; }

        public string FullNameBVIBonus { get; set; }

        public string NicknameBVIBonus { get; set; }

        public string FullNameLocalBonus { get; set; }

        public string NicknameLocalBonus { get; set; }

        public string PDFPassword { get; set; }

        #endregion Employment

        public string Salary { get; set; }

        public int? CurrencyId { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string Reason { get; set; }

        public int? WorkingDays { get; set; }

        public int? EbankingCurrencyId { get; set; }

        public string EbankingCurrencyCode { get; set; }

        public string EbankingAmount { get; set; }

        public string EbankingAccount { get; set; }

        public int? DirectBanking1CurrencyId { get; set; }

        public string DirectBanking1CurrencyCode { get; set; }

        public string DirectBanking1Amount { get; set; }

        public string DirectBanking1Account { get; set; }

        public int? DirectBanking2CurrencyId { get; set; }

        public string DirectBanking2CurrencyCode { get; set; }

        public string DirectBanking2Amount { get; set; }

        public string DirectBanking2Account { get; set; }

        public int? CashCurrencyId { get; set; }

        public string CashCurrencyCode { get; set; }

        public string CashAmount { get; set; }

        public string CashAccount { get; set; }

        public PayrollAutoCalculateOn AutoCalculateOn { get; set; }

        public int? AllowanceCurrencyId { get; set; }

        public string AllowanceCurrencyCode { get; set; }

        public string AllowanceAmount { get; set; }

        public int? AllowanceDirectBanking1CurrencyId { get; set; }

        public string AllowanceDirectBanking1CurrencyCode { get; set; }

        public string AllowanceDirectBanking1Amount { get; set; }

        public string AllowanceDirectBanking1Account { get; set; }

        public int? AllowanceDirectBanking2CurrencyId { get; set; }

        public string AllowanceDirectBanking2CurrencyCode { get; set; }

        public string AllowanceDirectBanking2Amount { get; set; }

        public string AllowanceDirectBanking2Account { get; set; }

        public int? AllowanceCashCurrencyId { get; set; }

        public string AllowanceCashCurrencyCode { get; set; }

        public string AllowanceCashAmount { get; set; }

        public string AllowanceCashAccount { get; set; }

        public PayrollAutoCalculateOn AllowanceAutoCalculateOn { get; set; }

        public string SubsistenceAmount { get; set; }

        public int? SubsistenceCurrencyId { get; set; }

        public string SubsistenceCurrencyCode { get; set; }

        public string HousingAmount { get; set; }

        public int? HousingCurrencyId { get; set; }

        public string HousingCurrencyCode { get; set; }

        public string HomePassageAmount { get; set; }

        public int? HomePassageCurrencyId { get; set; }

        public string HomePassageCurrencyCode { get; set; }

        public string DutyMealAmount { get; set; }

        public int? DutyMealCurrencyId { get; set; }

        public string DutyMealCurrencyCode { get; set; }

        public string HPIAmount { get; set; }

        public int? HPICurrencyId { get; set; }

        public string HPICurrencyCode { get; set; }

        public string MobilePhoneAmount { get; set; }

        public int? MobilePhoneCurrencyId { get; set; }

        public string MobilePhoneCurrencyCode { get; set; }

        public string OthersAmount { get; set; }

        public int? OthersCurrencyId { get; set; }

        public string OthersCurrencyCode { get; set; }

        public string SecondmentAmount { get; set; }

        public int? SecondmentCurrencyId { get; set; }

        public string SecondmentCurrencyCode { get; set; }

        public string RoomingChargesCurrencyCode { get; set; }

        public string RoomingChargesAmount { get; set; }

        public string FitnessFirstCurrencyCode { get; set; }

        public string FitnessFirstAmount { get; set; }
    }
}