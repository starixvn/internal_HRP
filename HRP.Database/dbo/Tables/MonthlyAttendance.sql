CREATE TABLE [dbo].[MonthlyAttendance] (
    [EmployeeId]       INT            NOT NULL,
    [Month]            DATE           NOT NULL,
    [ActualWorkingDay] DECIMAL (5, 1) NOT NULL,
    CONSTRAINT [PK_MonthlyAttendance] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [Month] ASC)
);

