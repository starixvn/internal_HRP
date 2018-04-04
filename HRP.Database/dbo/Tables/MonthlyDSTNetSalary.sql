CREATE TABLE [dbo].[MonthlyDSTNetSalary] (
    [EmployeeId]        INT           NOT NULL,
    [Month]             DATE          NOT NULL,
    [CurrencyId]        SMALLINT      NOT NULL,
    [NetSalaryReceived] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_MonthlyDSTNetSalary] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [Month] ASC)
);

