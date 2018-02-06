using System.ComponentModel.DataAnnotations;

namespace HRP.Core.Enumerations
{
    public enum FieldType
    {
        [Display(Name = "Company Local")]
        CompanyLocal = 1,

        [Display(Name = "Company BVI")]
        CompanyBVI = 2,

        [Display(Name = "Currency")]
        Currency = 3,

        [Display(Name = "Basic Salary Change Reason")]
        BasicSalaryChangeReason = 4,

        [Display(Name = "Nationality")]
        Nationality = 5,

        [Display(Name = "No of Children")]
        NoOfChildren = 6,

        [Display(Name = "Division")]
        Division = 18,

        [Display(Name = "Department")]
        Department = 20,

        [Display(Name = "Cost Center")]
        CostCenter = 24,

        [Display(Name = "Section")]
        Section = 22,

        [Display(Name = "Job Category")]
        JobCategory = 13,

        [Display(Name = "Grade Level")]
        GradeLevel = 15,

        [Display(Name = "Job Grade")]
        JobGrade = 16
    }
}