CREATE TABLE [dbo].[PaymentProcess] (
    [CompanyId]                        SMALLINT       NOT NULL,
    [Month]                            DATE           NOT NULL,
    [AttendanceSessionId]              NVARCHAR (100) NULL,
    [AttendanceFileName]               NVARCHAR (50)  NULL,
    [DSTNetSalarySessionId]            NVARCHAR (100) NULL,
    [DSTNetSalaryFileName]             NVARCHAR (50)  NULL,
    [IsPayrollGenerated]               BIT            NOT NULL,
    [IsAttendanceFileSubmitted]        BIT            NULL,
    [IsDSTNetSalaryFileSubmitted]      BIT            NULL,
    [AdditionDeductionSessionId]       NVARCHAR (100) NULL,
    [AdditionDeductionFileName]        NVARCHAR (50)  NULL,
    [IsAdditionDeductionFileSubmitted] BIT            NULL,
    CONSTRAINT [PK_PaymentProcess] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [Month] ASC)
);



