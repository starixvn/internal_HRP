CREATE TABLE [dbo].[Division] (
    [DivisionId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]       VARCHAR (10)   NOT NULL,
    [Name]       NVARCHAR (200) NOT NULL,
    [CompanyId]  SMALLINT       NOT NULL,
    [IsActive]   BIT            NOT NULL,
    CONSTRAINT [PK_Division] PRIMARY KEY CLUSTERED ([DivisionId] ASC)
);

