namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public enum MonthlySalaryDeductionType
    {
        [Display(Name = nameof(ViewLang.UPLAbsenteeism), ResourceType = typeof(ViewLang))]
        Absenteeism = 1,

        [Display(Name = nameof(ViewLang.Tardiness), ResourceType = typeof(ViewLang))]
        Tardiness,

        [Display(Name = nameof(ViewLang.SalaryAdvance), ResourceType = typeof(ViewLang))]
        SalaryAdvance,

        [Display(Name = nameof(ViewLang.Others), ResourceType = typeof(ViewLang))]
        Others
    }
}