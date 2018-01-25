CREATE TABLE [dbo].[EmploymentPaymaster] (
    [EmployeeId]           INT           NOT NULL,
    [FullNameBVISalary]    NVARCHAR (70) NOT NULL,
    [NickNameBVISalary]    VARCHAR (30)  NULL,
    [FullNameBVIAllowance] NVARCHAR (70) NOT NULL,
    [NickNameBVIAllowance] VARCHAR (30)  NULL,
    [FullNameLocalSalary]  NVARCHAR (70) NULL,
    [NickNameLocalSalary]  VARCHAR (30)  NULL,
    CONSTRAINT [PK_EmploymentPaymaster] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

