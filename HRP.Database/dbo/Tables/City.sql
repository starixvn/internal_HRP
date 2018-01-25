CREATE TABLE [dbo].[City] (
    [CityId]    SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]      VARCHAR (10)   NOT NULL,
    [Name]      NVARCHAR (200) NOT NULL,
    [CountryId] SMALLINT       NOT NULL,
    [IsActive]  BIT            NOT NULL,
    CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED ([CityId] ASC)
);

