CREATE TABLE [dbo].[DSTNetSalaryUploading] (
    [SessionId]         NVARCHAR (100)  NOT NULL,
    [RowNumber]         INT             NOT NULL,
    [EmployeeCode]      NVARCHAR (2000) NULL,
    [Currency]          NVARCHAR (20)   NULL,
    [NetSalaryReceived] VARCHAR (MAX)   NULL,
    CONSTRAINT [PK_DSTNetSalaryUploading] PRIMARY KEY CLUSTERED ([SessionId] ASC, [RowNumber] ASC)
);

