CREATE TABLE [dbo].[EmployeePayrollAllowance] (
    [EmployeeId]               INT           NOT NULL,
    [CurrencyId]               INT           NULL,
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
    [SubsistenceCurrencyId]    INT           NULL,
    [SubsistenceAmount]        VARCHAR (MAX) NULL,
    [HomePassageCurrencyId]    INT           NULL,
    [HomePassageAmount]        VARCHAR (MAX) NULL,
    [HousingCurrencyId]        INT           NULL,
    [HousingAmount]            VARCHAR (MAX) NULL,
    [MobilePhoneCurrencyId]    INT           NULL,
    [MobilePhoneAmount]        VARCHAR (MAX) NULL,
    [HPICurrencyId]            INT           NULL,
    [HPIAmount]                VARCHAR (MAX) NULL,
    [DutyMealCurrencyId]       INT           NULL,
    [DutyMealAmount]           VARCHAR (MAX) NULL,
    [SecondmentCurrencyId]     INT           NULL,
    [SecondmentAmount]         VARCHAR (MAX) NULL,
    [OthersCurrencyId]         INT           NULL,
    [OthersAmount]             VARCHAR (MAX) NULL,
    CONSTRAINT [PK_EmployeePayrollAllowance] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);







