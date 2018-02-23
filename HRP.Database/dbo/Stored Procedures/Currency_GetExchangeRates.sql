
CREATE PROCEDURE [dbo].[Currency_GetExchangeRates] @Month INT
	,@Year INT
AS
/*
Created: 20171222@Elton.Ton
Task: Get active currency code
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT c.FromCurrencyId
		,c.ToCurrencyId
		,c.ExchangeRate
	FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
	WHERE MONTH(c.ExchangeRateMonth) = @Month
		AND YEAR(c.ExchangeRateMonth) = @Year
END