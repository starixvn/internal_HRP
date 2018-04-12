CREATE TABLE [dbo].[BankAccount] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]      INT            NULL,
    [BankType]        INT            NULL,
    [BankName]        NVARCHAR (70)  NOT NULL,
    [BankBranch]      NVARCHAR (70)  NULL,
    [BankAddress]     NVARCHAR (500) NULL,
    [BankSwiftCode]   VARCHAR (15)   NULL,
    [AccountName]     NVARCHAR (30)  NOT NULL,
    [AccountNumber]   VARCHAR (30)   NOT NULL,
    [AccountType]     INT            NULL,
    [BeneficiaryName] NVARCHAR (70)  NOT NULL,
    [IsActive]        BIT            NULL
);



