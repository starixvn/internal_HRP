CREATE TABLE [dbo].[MonthlyAllowanceAddition] (
    [PayrollId]    INT            NOT NULL,
    [Type]         TINYINT        NOT NULL,
    [NoOfDays]     DECIMAL (6, 2) NULL,
    [UnitAmount]   VARCHAR (MAX)  NULL,
    [TotalAmount]  VARCHAR (MAX)  NULL,
    [NetAllowance] VARCHAR (MAX)  NULL,
    [CurrencyId]   TINYINT        NULL,
    CONSTRAINT [PK_MonthlyAllowanceAddition_1] PRIMARY KEY CLUSTERED ([PayrollId] ASC, [Type] ASC)
);

