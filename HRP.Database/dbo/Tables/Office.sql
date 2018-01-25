CREATE TABLE [dbo].[Office] (
    [OfficeId] SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Code]     VARCHAR (10)   NOT NULL,
    [Name]     NVARCHAR (200) NOT NULL,
    [CityId]   SMALLINT       NOT NULL,
    [IsActive] BIT            NOT NULL,
    CONSTRAINT [PK_Office] PRIMARY KEY CLUSTERED ([OfficeId] ASC)
);

