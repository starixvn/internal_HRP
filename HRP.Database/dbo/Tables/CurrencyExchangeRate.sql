CREATE TABLE [dbo].[CurrencyExchangeRate] (
    [ExchangeRateMonth] DATE            NOT NULL,
    [FromCurrency]      VARCHAR (3)     NOT NULL,
    [ToCurrency]        VARCHAR (3)     NOT NULL,
    [ExchangeRate]      DECIMAL (10, 5) NOT NULL,
    CONSTRAINT [PK_CurrencyExchangeRate] PRIMARY KEY CLUSTERED ([ExchangeRateMonth] ASC, [FromCurrency] ASC, [ToCurrency] ASC)
);

