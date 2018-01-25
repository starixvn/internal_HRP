CREATE TABLE [dbo].[EmployeePayrollBasicSalaryHistory] (
    [HistoryId]     INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeId]    INT           NOT NULL,
    [Salary]        VARCHAR (MAX) NULL,
    [CurrencyCode]  VARCHAR (3)   NULL,
    [EffectiveDate] DATE          NULL,
    [ReasonId]      TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([HistoryId] ASC),
    CONSTRAINT [FK_EmployeePayrollBasicSalaryHistory_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId])
);

