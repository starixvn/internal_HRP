namespace HRP.Models.Report
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public enum PaymentType
    {
        [Display(Name = nameof(ViewLang.BasicSalary), ResourceType = typeof(ViewLang))]
        BasicSalary = 1,
        [Display(Name = nameof(ViewLang.Allowances), ResourceType = typeof(ViewLang))]
        Allowances = 2
    }
}