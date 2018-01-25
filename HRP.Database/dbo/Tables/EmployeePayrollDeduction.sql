CREATE TABLE [dbo].[EmployeePayrollDeduction] (
    [EmployeeId]                 INT           NOT NULL,
    [RoomingChargesCurrencyCode] VARCHAR (3)   NULL,
    [RoomingChargesAmount]       VARCHAR (MAX) NULL,
    [FitnessFirstCurrencyCode]   VARCHAR (3)   NULL,
    [FitnessFirstAmount]         VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([EmployeeId] ASC),
    CONSTRAINT [FK_EmployeePayrollDeduction_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [FK_EmployeePayrollDeduction_FitnessFirst_Currency] FOREIGN KEY ([FitnessFirstCurrencyCode]) REFERENCES [dbo].[Currency] ([CurrencyCode]),
    CONSTRAINT [FK_EmployeePayrollDeduction_RoomingCharges_Currency] FOREIGN KEY ([RoomingChargesCurrencyCode]) REFERENCES [dbo].[Currency] ([CurrencyCode])
);

