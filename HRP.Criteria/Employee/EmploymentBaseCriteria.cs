namespace HRP.Criteria.Employee
{
    using System;

    public abstract class EmploymentBaseCriteria : CommandBaseCriteria
    {
        public virtual int CompanyId { get; set; }

        public virtual int LocalCompanyId { get; set; }

        public virtual string JobTitle { get; set; }

        public virtual int LocalJobTitleId { get; set; }

        public virtual string EmployeeCode { get; set; }

        public virtual int EmploymentType { get; set; }

        public virtual int EmploymentStatus { get; set; }

        public virtual int JobCategoryId { get; set; }

        public virtual int JobGradeId { get; set; }

        public virtual int GradeLevelId { get; set; }

        public virtual int DivisionId { get; set; }

        public virtual int DepartmentId { get; set; }

        public virtual int SectionId { get; set; }

        public virtual int CostCenterId { get; set; }

        public virtual DateTime GroupStartDate { get; set; }

        public virtual DateTime? GroupEndDate { get; set; }

        public virtual DateTime? CompanyStartDate { get; set; }

        public virtual DateTime? CompanyEndDate { get; set; }

        public virtual string Email { get; set; }

        public virtual int CountryId { get; set; }

        public virtual int CityId { get; set; }

        public virtual int OfficeId { get; set; }

        public virtual int? WorkPermitCompanyId { get; set; }

        public virtual int? Type1 { get; set; }

        public virtual DateTime? StartDateType1 { get; set; }

        public virtual DateTime? EndDateType1 { get; set; }

        public virtual int? Type2 { get; set; }

        public virtual DateTime? StartDateType2 { get; set; }

        public virtual DateTime? EndDateType2 { get; set; }

        public virtual int? DST { get; set; }

        public virtual int? DBT { get; set; }

        public virtual string TaxIDNo { get; set; }

        public virtual string SSSNo { get; set; }

        public virtual string HealthInsNo { get; set; }

        public virtual string HDMFNo { get; set; }

        public virtual string FullNamePFileAdmin { get; set; }

        public virtual string NicknamePFileAdmin { get; set; }

        public virtual string FullNameBVISalary { get; set; }

        public virtual string NicknameBVISalary { get; set; }

        public virtual string FullNameBVIAllowance { get; set; }

        public virtual string NicknameBVIAllowance { get; set; }

        public virtual string FullNameLocalSalary { get; set; }

        public virtual string NicknameLocalSalary { get; set; }

        public virtual string FullNameAdministrator { get; set; }

        public virtual string NicknameAdministrator { get; set; }

        public virtual string FullNameEvaluator1 { get; set; }

        public virtual string NicknameEvaluator1 { get; set; }

        public virtual string FullNameEvaluator2 { get; set; }

        public virtual string NicknameEvaluator2 { get; set; }

        public virtual string FullNameReviewer { get; set; }

        public virtual string NicknameReviewer { get; set; }

        public virtual string FullNameManagement { get; set; }

        public virtual string NicknameManagement { get; set; }

        public virtual string FullNameIncrement { get; set; }

        public virtual string NicknameIncrement { get; set; }

        public virtual string FullNameBVIBonus { get; set; }

        public virtual string NicknameBVIBonus { get; set; }

        public virtual string FullNameLocalBonus { get; set; }

        public virtual string NicknameLocalBonus { get; set; }

        public virtual string PDFPassword { get; set; }

#pragma warning disable S1541 // Methods and properties should not be too complex
        public virtual bool IsValidParams() =>
#pragma warning restore S1541 // Methods and properties should not be too complex
#pragma warning disable S1067 // Expressions should not be too complex
            CompanyId > 0
            && LocalCompanyId > 0
            && LocalJobTitleId > 0
            && EmploymentType > 0
            && EmploymentStatus > 0
            && JobCategoryId > 0
            && JobGradeId > 0
            && GradeLevelId > 0
            && DivisionId > 0
            && DepartmentId > 0
            && SectionId > 0
            && CostCenterId > 0
            && CountryId > 0
            && CityId > 0
            && OfficeId > 0
            && !string.IsNullOrWhiteSpace(JobTitle)
            && !string.IsNullOrWhiteSpace(EmployeeCode)
            && !string.IsNullOrWhiteSpace(Email)
            && !string.IsNullOrWhiteSpace(FullNameBVISalary)
            && !string.IsNullOrWhiteSpace(FullNameBVIAllowance)
            && !string.IsNullOrWhiteSpace(FullNameAdministrator)
            && !string.IsNullOrWhiteSpace(PDFPassword)
            && !string.IsNullOrWhiteSpace(FullNamePFileAdmin);
#pragma warning restore S1067 // Expressions should not be too complex
    }
}