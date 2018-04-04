CREATE TABLE [dbo].[MonthlyAllowance] (
    [PayrollId]                INT           NOT NULL,
    [CurrencyId]               INT           NULL,
    [TotalAmount]              VARCHAR (MAX) NULL,
    [DirectBanking1CurrencyId] INT           NULL,
    [DirectBanking1Amount]     VARCHAR (MAX) NULL,
    [DirectBanking1BankTypeId] TINYINT       NULL,
    [DirectBanking1AccountId]  INT           NULL,
    [DirectBanking2CurrencyId] INT           NULL,
    [DirectBanking2Amount]     VARCHAR (MAX) NULL,
    [DirectBanking2BankTypeId] TINYINT       NULL,
    [DirectBanking2AccountId]  INT           NULL,
    [CashCurrencyId]           INT           NULL,
    [CashAmount]               VARCHAR (MAX) NULL,
    [CashAccountId]            INT           NULL,
    [DSTNetSalaryCurrencyId]   INT           NULL,
    [DSTNetSalaryAmount]       VARCHAR (MAX) NULL,
    [AutoCalculateOn]          VARCHAR (30)  NULL,
    [TotalNetAmount]           VARCHAR (MAX) NULL,
    [Status]                   TINYINT       NULL,
    CONSTRAINT [PK_MonthlyAllowance_1] PRIMARY KEY CLUSTERED ([PayrollId] ASC)
);

