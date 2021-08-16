namespace HRP.Core.Enumerations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public enum BasicSalaryChangeReason
    {
        [Display(Name = nameof(ViewLang.AnnualIncrement), ResourceType = typeof(ViewLang))]
        AnnualIncrement = 1,

        [Display(Name = nameof(ViewLang.Promotion), ResourceType = typeof(ViewLang))]
        Promotion,

        [Display(Name = nameof(ViewLang.SalaryAdjustment), ResourceType = typeof(ViewLang))]
        SalaryAdjustment,

        [Display(Name = nameof(ViewLang.Demotion), ResourceType = typeof(ViewLang))]
        Demotion,

        [Display(Name = nameof(ViewLang.Penalty), ResourceType = typeof(ViewLang))]
        Penalty
    }
}