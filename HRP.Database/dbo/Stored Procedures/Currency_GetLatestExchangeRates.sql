/*
Created: 20180122@Elton.Ton
Task: Get latest exchange rate
DB: bodb_HRP
Original:
*/
CREATE PROCEDURE [dbo].[Currency_GetLatestExchangeRates]
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @LatestMonth DATE

	SELECT TOP 1 @LatestMonth = c.ExchangeRateMonth
	FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
	WHERE MONTH(c.ExchangeRateMonth) <= MONTH(GETDATE())
	ORDER BY c.ExchangeRateMonth DESC

	SELECT c.FromCurrency, c.ToCurrency, c.ExchangeRate
	FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
	WHERE MONTH(c.ExchangeRateMonth) = MONTH(@LatestMonth) AND YEAR(c.ExchangeRateMonth) = YEAR(@LatestMonth)
END