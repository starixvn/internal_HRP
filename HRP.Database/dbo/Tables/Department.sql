CREATE TABLE [dbo].[Department] (
    [DepartmentId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [DivisionId]   SMALLINT       NOT NULL,
    [IsActive]     BIT            NOT NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);



