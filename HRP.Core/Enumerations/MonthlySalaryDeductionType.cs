namespace HRP.Core.Enumerations
{
    using System.ComponentModel;

    public enum MonthlySalaryDeductionType
    {
        [Description("UPL/Absenteeism")]
        Absenteeism = 1,

        [Description("Tardiness")]
        Tardiness,

        [Description("Salary Advance")]
        SalaryAdvance,

        [Description("Others")]
        Others
    }
}