CREATE TABLE [dbo].[MonthlySalaryDeduction] (
    [PayrollId]   INT             NOT NULL,
    [Type]        TINYINT         NOT NULL,
    [UnitAmount]  VARCHAR (MAX)   NULL,
    [NoOfDays]    DECIMAL (8, 2)  NULL,
    [NetSalary]   VARCHAR (MAX)   NULL,
    [Description] NVARCHAR (1000) NULL,
    CONSTRAINT [PK_MonthlySalaryDeduction_1] PRIMARY KEY CLUSTERED ([PayrollId] ASC, [Type] ASC)
);

