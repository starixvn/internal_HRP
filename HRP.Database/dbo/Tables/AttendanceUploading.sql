CREATE TABLE [dbo].[AttendanceUploading] (
    [SessionId]        NVARCHAR (100)  NOT NULL,
    [RowNumber]        INT             NOT NULL,
    [EmployeeCode]     NVARCHAR (2000) NULL,
    [ActualWorkingDay] NVARCHAR (20)   NULL,
    CONSTRAINT [PK_PaymentProcessAttendanceUploading] PRIMARY KEY CLUSTERED ([SessionId] ASC, [RowNumber] ASC)
);

