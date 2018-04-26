
CREATE PROCEDURE [dbo].[Currency_GetCurrencyCodesForSetting] @Month DATE
AS
/*
Created: 20180405@Elton.Ton
Task: Get currency codes for setting
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @currentDate DATE = GETDATE()

	IF MONTH(@currentDate) = MONTH(@Month)
		AND YEAR(@currentDate) = YEAR(@Month)
	BEGIN
		SELECT c.CurrencyId
			,c.CurrencyCode
		FROM dbo.Currency AS c WITH (NOLOCK)
		WHERE EXISTS (
				SELECT 1
				FROM dbo.CurrencyExchangeRate AS cer WITH (NOLOCK)
				WHERE MONTH(cer.ExchangeRateMonth) = MONTH(@Month)
					AND YEAR(cer.ExchangeRateMonth) = YEAR(@Month)
					AND (
						cer.FromCurrencyId = c.CurrencyId
						OR cer.ToCurrencyId = c.CurrencyId
						)
				)
			OR c.IsActive = 1
		ORDER BY c.[Order]
	END
	ELSE IF (
			MONTH(@currentDate) < MONTH(@Month)
			AND YEAR(@currentDate) = YEAR(@Month)
			)
		OR YEAR(@currentDate) < YEAR(@Month)
	BEGIN
		SELECT c.CurrencyId
			,c.CurrencyCode
		FROM dbo.Currency AS c WITH (NOLOCK)
		WHERE c.IsActive = 1
		ORDER BY c.[Order]
	END
	ELSE
	BEGIN
		SELECT c.CurrencyId
			,c.CurrencyCode
		FROM dbo.Currency AS c WITH (NOLOCK)
		WHERE EXISTS (
				SELECT 1
				FROM dbo.CurrencyExchangeRate AS cer WITH (NOLOCK)
				WHERE MONTH(cer.ExchangeRateMonth) = MONTH(@Month)
					AND YEAR(cer.ExchangeRateMonth) = YEAR(@Month)
					AND (
						cer.FromCurrencyId = c.CurrencyId
						OR cer.ToCurrencyId = c.CurrencyId
						)
				)
		ORDER BY c.[Order]
	END
END