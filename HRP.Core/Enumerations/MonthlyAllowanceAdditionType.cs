namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public enum MonthlyAllowanceAdditionType
    {
        [Display(Name = nameof(ViewLang.Subsistence), ResourceType = typeof(ViewLang))]
        Subsistence = 1,

        [Display(Name = nameof(ViewLang.HomePassage), ResourceType = typeof(ViewLang))]
        HomePassage,

        [Display(Name = nameof(ViewLang.Housing), ResourceType = typeof(ViewLang))]
        Housing,

        [Display(Name = nameof(ViewLang.MobilePhone), ResourceType = typeof(ViewLang))]
        MobilePhone,

        [Display(Name = nameof(ViewLang.HPI), ResourceType = typeof(ViewLang))]
        HPI,

        [Display(Name = nameof(ViewLang.DutyMeal), ResourceType = typeof(ViewLang))]
        DutyMeal,

        [Display(Name = nameof(ViewLang.Secondment), ResourceType = typeof(ViewLang))]
        Secondment,

        [Display(Name = nameof(ViewLang.Others), ResourceType = typeof(ViewLang))]
        Others,

        [Display(Name = nameof(ViewLang.SpecialAllowances), ResourceType = typeof(ViewLang))]
        SpecialAllowances,

        [Display(Name = nameof(ViewLang.OT), ResourceType = typeof(ViewLang))]
        OT
    }
}