namespace HRP.Core.Enumerations
{
    using System.ComponentModel;

    public enum MonthlySalaryDeductionType
    {
        [Description("UPL/Absenteeism")]
        Absenteeism = 1,

        Tardiness,

        [Description("Salary Advance")]
        SalaryAdvance,

        Others
    }
}