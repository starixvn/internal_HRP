
CREATE PROCEDURE [dbo].[Currency_GetActiveCurrencyCodes]
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
	SET NOCOUNT ON;

	SELECT c.CurrencyId
	,c.CurrencyCode
	FROM dbo.Currency AS c WITH (NOLOCK)
	WHERE c.IsActive = 1
	ORDER BY c.[Order]
END