CREATE TABLE [dbo].[EmployeePayrollDeduction] (
    [EmployeeId]               INT           NOT NULL,
    [RoomingChargesCurrencyId] INT           NULL,
    [RoomingChargesAmount]     VARCHAR (MAX) NULL,
    [FitnessFirstCurrencyId]   INT           NULL,
    [FitnessFirstAmount]       VARCHAR (MAX) NULL,
    CONSTRAINT [PK_EmployeePayrollDeduction] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);





