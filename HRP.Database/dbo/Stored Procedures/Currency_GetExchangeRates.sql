
CREATE PROCEDURE Currency_GetExchangeRates
	@Month	INT
	,@Year	INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT c.FromCurrency, c.ToCurrency, c.ExchangeRate
	FROM dbo.CurrencyExchangeRate AS c WITH (NOLOCK)
	WHERE MONTH(c.ExchangeRateMonth) = @Month AND YEAR(c.ExchangeRateMonth) = @Year
END