CREATE TABLE [dbo].[Currency] (
    [CurrencyCode] VARCHAR (3)    NOT NULL,
    [CurrencyName] NVARCHAR (200) NOT NULL,
    [IsActive]     BIT            NOT NULL,
    [Order]        TINYINT        NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([CurrencyCode] ASC)
);

