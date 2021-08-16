namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public enum MonthlySalaryAdditionType
    {
        [Display(Name = nameof(ViewLang.BackPay), ResourceType = typeof(ViewLang))]
        BackPay = 1,

        [Display(Name = nameof(ViewLang.Adjustments), ResourceType = typeof(ViewLang))]
        Adjustments,

        [Display(Name = nameof(ViewLang.Others), ResourceType = typeof(ViewLang))]
        Others1,

        [Display(Name = "")]
        Others2
    }
}