CREATE TABLE [dbo].[Country] (
    [CountryId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]      VARCHAR (10)   NOT NULL,
    [Name]      NVARCHAR (200) NOT NULL,
    [IsActive]  BIT            NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);

