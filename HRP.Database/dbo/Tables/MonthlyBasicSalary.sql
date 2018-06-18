CREATE TABLE [dbo].[MonthlyBasicSalary] (
    [PayrollId]                INT           NOT NULL,
    [OldCurrencyId]            INT           NULL,
    [OldSalary]                VARCHAR (MAX) NULL,
    [CurrencyId]               INT           NULL,
    [Salary]                   VARCHAR (MAX) NULL,
    [WorkingDays]              TINYINT       NULL,
    [EbankingCurrencyId]       INT           NULL,
    [EbankingAmount]           VARCHAR (MAX) NULL,
    [DirectBanking1CurrencyId] INT           NULL,
    [DirectBanking1Amount]     VARCHAR (MAX) NULL,
    [DirectBanking2CurrencyId] INT           NULL,
    [DirectBanking2Amount]     VARCHAR (MAX) NULL,
    [CashCurrencyId]           INT           NULL,
    [CashAmount]               VARCHAR (MAX) NULL,
    [AutoCalculateOn]          VARCHAR (30)  NULL,
    [AfterAdjustmentAmount]    VARCHAR (MAX) NULL,
    [AfterAdjustmentReason]    VARCHAR (MAX) NULL,
    [CreatedBy]                INT           NULL,
    [CreatedDate]              DATETIME      NULL,
    CONSTRAINT [PK_MonthlyBasicSalary_1] PRIMARY KEY CLUSTERED ([PayrollId] ASC)
);



