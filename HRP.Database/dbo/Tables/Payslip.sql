CREATE TABLE [dbo].[Payslip] (
    [PayrollId] INT NOT NULL,
    [IsSent]    BIT NOT NULL,
    CONSTRAINT [PK_Payslip] PRIMARY KEY CLUSTERED ([PayrollId] ASC)
);

