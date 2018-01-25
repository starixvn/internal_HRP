CREATE PROCEDURE [dbo].[Currency_GetActiveCurrencyCodes]
AS
BEGIN
	SET NOCOUNT ON

	SELECT c.CurrencyCode
	FROM dbo.Currency AS c WITH (NOLOCK)
	WHERE c.IsActive = 1
	ORDER BY c.[Order]
END