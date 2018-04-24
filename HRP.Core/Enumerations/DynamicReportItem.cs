
namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum DynamicReportItem
    {
        [Display(Name = "ID No")]
        IDNo = 1,

        [Display(Name = "Full Name")]
        FullName,

        [Display(Name = "Nick Name")]
        Nickname,

        [Display(Name = "Division")]
        Division,

        [Display(Name = "Department")]
        Department,

        [Display(Name = "CostCenter")]
        CostCenter,

        [Display(Name = "Actual Basic Salary")]
        ActualBasicSalary,

        [Display(Name = "Basic Salary")]
        BasicSalary,

        [Display(Name = "Basic Salary Addition")]
        BasicSalaryAddition,

        [Display(Name = "Basic Salary Deduction")]
        BasicSalaryDeduction,

        [Display(Name = "Basic Salary Net")]
        BasicSalaryNet,

        [Display(Name = "Allowances Amount")]
        Allowances
    }
}
