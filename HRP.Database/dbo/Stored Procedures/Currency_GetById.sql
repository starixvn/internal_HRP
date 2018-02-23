
Create PROCEDURE [dbo].Currency_GetById 
	@CurrencyId SMALLINT = NULL
AS
/*
Created: 20180222@Raymond.Le
Task: Get Currency by id
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
	WHERE CurrencyId = @CurrencyId

END