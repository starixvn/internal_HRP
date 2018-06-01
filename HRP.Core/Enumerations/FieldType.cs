namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum FieldType
    {
        [Display(Name = "Company Local")]
        CompanyLocal = 1,

        [Display(Name = "Company BVI")]
        CompanyBVI = 2,

        [Display(Name = "Currency")]
        Currency = 3,

        //[Display(Name = "Basic Salary Change Reason")]
        //BasicSalaryChangeReason = 4,

        [Display(Name = "Nationality")]
        Nationality = 5,

        [Display(Name = "No of Children")]
        NoOfChildren = 6,

        [Display(Name = "Job Title Local")]
        JobTitleLocal = 7,

        [Display(Name = "Job Title BVI Change Reason")]
        JobTitleBVIChangeReason = 8,

        [Display(Name = "Date Commencement Change Reason - Group")]
        DateCommencementGroupChangeReason = 9,

        [Display(Name = "Employee ID Change Reason")]
        EmployeeIDChangeReason = 10,

        [Display(Name = "Employment Type")]
        EmploymentType = 11,

        [Display(Name = "Employment Status")]
        EmploymentStatus = 12,

        [Display(Name = "Job Category")]
        JobCategory = 13,

        [Display(Name = "Job Category Change Reason")]
        JobCategoryChangeReason = 14,

        [Display(Name = "Grade Level")]
        GradeLevel = 15,

        [Display(Name = "Job Grade")]
        JobGrade = 16,

        [Display(Name = "Job Grade Change Reason")]
        JobGradeChangeReason = 17,

        [Display(Name = "Division")]
        Division = 18,

        [Display(Name = "Division Change Reason")]
        DivisionChangeReason = 19,

        [Display(Name = "Department")]
        Department = 20,

        [Display(Name = "Department Change Reason")]
        DepartmentChangeReason = 21,

        [Display(Name = "Section")]
        Section = 22,

        [Display(Name = "Section Change Reason")]
        SectionChangeReason = 23,

        [Display(Name = "Cost Center")]
        CostCenter = 24,

        [Display(Name = "Cost Center Change Reason")]
        CostCenterChangeReason = 25,

        [Display(Name = "Country")]
        Country = 26,

        [Display(Name = "City")]
        City = 27,

        [Display(Name = "Office")]
        Office = 28,

        [Display(Name = "Work Permit Type 1")]
        WorkPermitType1 = 29,

        [Display(Name = "Work Permit Type 2")]
        WorkPermitType2 = 30,

        [Display(Name = "Type Of Bank Account")]
        TypeOfBankAccount = 31,

        [Display(Name = "Payment Transaction - Cash")]
        PaymentTransactionCash = 32,

        //[Display(Name = "Contra Co. Account")]
        //ContraCoAccount = 33,

        [Display(Name = "Company BVI Change Reason")]
        CompanyBVIChangeReason = 34,

        [Display(Name = "Company Local Change Reason")]
        CompanyLocalChangeReason = 35,

        [Display(Name = "Payment Transaction - eBanking")]
        PaymentTransactionEbanking = 36,

        [Display(Name = "Payment Transaction - Direct Banking")]
        PaymentTransactionDirectBanking = 37,

        [Display(Name = "Job Title Local Change Reason")]
        JobTitleLocalChangeReason = 38,

        [Display(Name = "Date Commencement Change Reason - Company")]
        DateCommencementCompanyChangeReason = 39,
    }
}