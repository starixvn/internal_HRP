CREATE TABLE [dbo].[Currency] (
    [CurrencyCode] NVARCHAR (50)  NOT NULL,
    [CurrencyName] NVARCHAR (200) NULL,
    [IsActive]     BIT            NOT NULL,
    [Order]        TINYINT        NULL,
    [CurrencyId]   SMALLINT       IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([CurrencyId] ASC)
);



