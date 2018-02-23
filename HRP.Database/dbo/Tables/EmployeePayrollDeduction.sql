CREATE TABLE [dbo].[EmployeePayrollDeduction] (
    [EmployeeId]               INT           NOT NULL,
    [RoomingChargesCurrencyId] INT           NULL,
    [RoomingChargesAmount]     VARCHAR (MAX) NULL,
    [FitnessFirstCurrencyId]   INT           NULL,
    [FitnessFirstAmount]       VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);



