namespace HRP.Criteria.Employee
{
    using System;

    public class UpdateEmploymentCriteria : CommandBaseCriteria
    {
        public int EmployeeId { get; set; }

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

        public string NickNamePFileAdmin { get; set; }

        public string FullNameBVISalary { get; set; }

        public string NickNameBVISalary { get; set; }

        public string FullNameBVIAllowance { get; set; }

        public string NickNameBVIAllowance { get; set; }

        public string FullNameLocalSalary { get; set; }

        public string NickNameLocalSalary { get; set; }

        public string FullNameAdministrator { get; set; }

        public string NickNameAdministrator { get; set; }

        public string FullNameEvaluator1 { get; set; }

        public string NickNameEvaluator1 { get; set; }

        public string FullNameEvaluator2 { get; set; }

        public string NickNameEvaluator2 { get; set; }

        public string FullNameReviewer { get; set; }

        public string NickNameReviewer { get; set; }

        public string FullNameManagement { get; set; }

        public string NickNameManagement { get; set; }

        public string FullNameIncrement { get; set; }

        public string NickNameIncrement { get; set; }

        public string FullNameBVIBonus { get; set; }

        public string NickNameBVIBonus { get; set; }

        public string FullNameLocalBonus { get; set; }

        public string NickNameLocalBonus { get; set; }

        public string PDFPassword { get; set; }

        public int CreatedBy { get; set; }

#pragma warning disable S1541 // Methods and properties should not be too complex
        public override bool IsValid() =>
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
            EmployeeId > 0
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

        public override string GetSettingKey() => "Employment_Update";
    }
}