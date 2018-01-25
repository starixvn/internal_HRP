CREATE TABLE [dbo].[Company] (
    [CompanyId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]      VARCHAR (50)   NULL,
    [Name]      NVARCHAR (200) NOT NULL,
    [IsActive]  BIT            NOT NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([CompanyId] ASC)
);

