CREATE TABLE [dbo].[Country] (
    [CountryId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]      NVARCHAR (50)  NULL,
    [Name]      NVARCHAR (200) NOT NULL,
    [IsActive]  BIT            NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);



