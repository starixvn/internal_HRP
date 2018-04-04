CREATE TABLE [dbo].[MonthlyPayroll] (
    [PayrollId]            INT     IDENTITY (1, 1) NOT NULL,
    [EmployeeId]           INT     NOT NULL,
    [Month]                DATE    NOT NULL,
    [SalaryEffectiveDate]  DATE    NULL,
    [SalaryChangeReasonId] TINYINT NULL,
    [GroupStartDate]       DATE    NULL,
    [GroupEndDate]         DATE    NULL,
    CONSTRAINT [PK_MonthlyPayroll] PRIMARY KEY CLUSTERED ([PayrollId] ASC)
);

