namespace HRP.Core.Enumerations
{
    using System.ComponentModel;

    public enum BasicSalaryChangeReason
    {
        [Description("Annual Increment")]
        AnnualIncrement = 1,

        [Description("Promotion")]
        Promotion,

        [Description("Salary Adjustment")]
        SalaryAdjustment,

        [Description("Demotion")]
        Demotion,

        [Description("Penalty")]
        Penalty
    }
}