CREATE TABLE [dbo].[EmployeePayrollBasicSalaryHistory] (
    [HistoryId]     INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeId]    INT           NOT NULL,
    [CurrencyId]    INT           NULL,
    [Salary]        VARCHAR (MAX) NULL,
    [EffectiveDate] DATE          NULL,
    [ReasonId]      TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([HistoryId] ASC)
);



