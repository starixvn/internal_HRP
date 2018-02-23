
Create PROCEDURE [dbo].Currency_GetAll @IsActive BIT = NULL
AS
/*
Created: 20180222@Raymond.Le
Task: Get all Currency
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CurrencyId
		,CurrencyCode
		,CurrencyName
		,IsActive
	FROM dbo.Currency WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
	ORDER BY IsActive DESC
		,CurrencyCode 
END