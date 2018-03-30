namespace HRP.Core.Enumerations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum AllowanceType
    {
        Subsistence = 1,

        [Description("Home Passage")]
        [Display(Name = "Home Passage")]
        HomePassage,

        Housing,

        [Description("Mobile Phone")]
        [Display(Name = "Mobile Phone")]
        MobilePhone,

        HPI,

        [Description("Duty Meal")]
        [Display(Name = "Duty Meal")]
        DutyMeal,

        Secondment,

        Others
    }
}