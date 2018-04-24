CREATE TABLE [dbo].[EmployeePayrollBasicSalary] (
    [EmployeeId]               INT           NOT NULL,
    [CurrencyId]               INT           NULL,
    [Salary]                   VARCHAR (MAX) NULL,
    [WorkingDays]              TINYINT       NOT NULL,
    [EbankingCurrencyId]       INT           NULL,
    [EbankingAmount]           VARCHAR (MAX) NULL,
    [EbankingAccountId]        INT           NULL,
    [DirectBanking1CurrencyId] INT           NULL,
    [DirectBanking1Amount]     VARCHAR (MAX) NULL,
    [DirectBanking1AccountId]  INT           NULL,
    [DirectBanking2CurrencyId] INT           NULL,
    [DirectBanking2Amount]     VARCHAR (MAX) NULL,
    [DirectBanking2AccountId]  INT           NULL,
    [CashCurrencyId]           INT           NULL,
    [CashAmount]               VARCHAR (MAX) NULL,
    [CashAccountId]            INT           NULL,
    [AutoCalculateOn]          VARCHAR (30)  NULL,
    CONSTRAINT [PK_EmployeePayrollBasicSalary] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);







