namespace HRP.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class CreateEmployeeCriteria : CriteriaBase
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

        public int CompanyId { get; set; }

        public DateTime? CompanyChangeDate { get; set; }

        public int? CompanyReasonId { get; set; }

        public int LocalCompanyId { get; set; }

        public DateTime? LocalCompanyChangeDate { get; set; }

        public int? LocalCompanyReasonId { get; set; }

        public string JobTitle { get; set; }

        public DateTime? JobTitleChangeDate { get; set; }

        public int? JobTitleReasonId { get; set; }

        public int LocalJobTitleId { get; set; }

        public DateTime? LocalJobTitleChangeDate { get; set; }

        public int? LocalJobTitleReasonId { get; set; }

        public string CompanyCode { get; set; }

        public string EmployeeCode { get; set; }

        public DateTime? IDNoChangeDate { get; set; }

        public int? IDNoReasonId { get; set; }

        public int EmploymentType { get; set; }

        public int EmploymentStatus { get; set; }

        public DateTime? EmploymentStatusChangeDate { get; set; }

        public int JobCategoryId { get; set; }

        public DateTime? JobCategoryChangeDate { get; set; }

        public int? JobCategoryReasonId { get; set; }

        public int JobGradeId { get; set; }

        public int GradeLevelId { get; set; }

        public DateTime? JobGradeChangeDate { get; set; }

        public int? JobGradeReasonId { get; set; }

        public int DivisionId { get; set; }

        public DateTime? DivisionChangeDate { get; set; }

        public int? DivisionReasonId { get; set; }

        public int DepartmentId { get; set; }

        public DateTime? DepartmentChangeDate { get; set; }

        public int? DepartmentReasonId { get; set; }

        public int SectionId { get; set; }

        public DateTime? SectionChangeDate { get; set; }

        public int? SectionReasonId { get; set; }

        public int CostCenterId { get; set; }

        public DateTime? CostCenterChangeDate { get; set; }

        public int? CostCenterReasonId { get; set; }

        public DateTime GroupStartDate { get; set; }

        public DateTime? GroupEndDate { get; set; }

        public int? GroupDateReasonId { get; set; }

        public DateTime? CompanyStartDate { get; set; }

        public DateTime? CompanyEndDate { get; set; }

        public int? CompanyDateReasonId { get; set; }

        public string Email { get; set; }

        public int CountryId { get; set; }

        public int CityId { get; set; }

        public int OfficeId { get; set; }

        public int WorkPermitCompanyId { get; set; }

        public int Type1 { get; set; }

        public DateTime StartDateType1 { get; set; }

        public DateTime EndDateType1 { get; set; }

        public int? Type2 { get; set; }

        public DateTime? StartDateType2 { get; set; }

        public DateTime? EndDateType2 { get; set; }

        public int? DST { get; set; }

        public int? DBT { get; set; }

        public string TaxIDNo { get; set; }

        public string SSSNo { get; set; }

        public string HealthInsNo { get; set; }

        public string HDMFNo { get; set; }

        public string FullNamePFileAdmin { get; set; }

        public string NicknamePFileAdmin { get; set; }

        public string FullNameBVISalary { get; set; }

        public string NicknameBVISalary { get; set; }

        public string FullNameBVIAllowance { get; set; }

        public string NicknameBVIAllowance { get; set; }

        public string FullNameLocalSalary { get; set; }

        public string NicknameLocalSalary { get; set; }

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

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Employee_CreateEmployee";

#pragma warning disable S1541 // Methods and properties should not be too complex
        public override bool IsValid() =>
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
            !string.IsNullOrEmpty(FullName)
            && !string.IsNullOrEmpty(Nickname)
            && CompanyId > 0
            && LocalCompanyId > 0
            && !string.IsNullOrWhiteSpace(JobTitle)
            && LocalJobTitleId > 0
            && !string.IsNullOrWhiteSpace(CompanyCode)
            && !string.IsNullOrWhiteSpace(EmployeeCode)
            && EmploymentType > 0
            && EmploymentStatus > 0
            && JobCategoryId > 0
            && JobGradeId > 0
            && GradeLevelId > 0
            && DivisionId > 0
            && DepartmentId > 0
            && SectionId > 0
            && CostCenterId > 0
            && !string.IsNullOrWhiteSpace(Email)
            && CountryId > 0
            && CityId > 0
            && OfficeId > 0
            && WorkPermitCompanyId > 0
            && Type1 > 0
            && !string.IsNullOrWhiteSpace(FullNameBVISalary)
            && !string.IsNullOrWhiteSpace(FullNameBVIAllowance)
            && !string.IsNullOrWhiteSpace(FullNameAdministrator)
            && !string.IsNullOrWhiteSpace(PDFPassword)
            && !string.IsNullOrWhiteSpace(FullNamePFileAdmin)
            && CreatedBy > 0;
#pragma warning restore S1067 // Expressions should not be too complex
    }
}