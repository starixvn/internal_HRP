CREATE TABLE [dbo].[Division] (
    [DivisionId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]       NVARCHAR (50)  NULL,
    [Name]       NVARCHAR (200) NOT NULL,
    [CompanyId]  SMALLINT       NOT NULL,
    [IsActive]   BIT            NOT NULL,
    CONSTRAINT [PK_Division] PRIMARY KEY CLUSTERED ([DivisionId] ASC)
);



