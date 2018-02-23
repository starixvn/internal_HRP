CREATE TABLE [dbo].[CurrencyExchangeRate] (
    [ExchangeRateMonth] DATE            NOT NULL,
    [ExchangeRate]      DECIMAL (10, 5) NOT NULL,
    [FromCurrencyId]    INT             NOT NULL,
    [ToCurrencyId]      INT             NOT NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([ExchangeRateMonth] ASC, [FromCurrencyId] ASC, [ToCurrencyId] ASC)
);



