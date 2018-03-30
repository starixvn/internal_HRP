namespace HRP.Core.Enumerations
{
    using System.ComponentModel;

    public enum MonthlySalaryAdditionType
    {
        [Description("Back Pay")]
        BackPay = 1,

        Adjustments,

        [Description("Others")]
        Others1,

        [Description("")]
        Others2
    }
}