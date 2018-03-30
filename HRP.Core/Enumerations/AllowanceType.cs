namespace HRP.Core.Enumerations
{
    using System.ComponentModel.DataAnnotations;

    public enum AllowanceType
    {
        Subsistence = 1,

        [Display(Name = "Home Passage")]
        HomePassage,

        Housing,

        [Display(Name = "Mobile Phone")]
        MobilePhone,

        HPI,

        [Display(Name = "Duty Meal")]
        DutyMeal,

        Secondment,

        Others
    }
}