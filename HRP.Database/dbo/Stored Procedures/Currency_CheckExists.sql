
Create PROCEDURE [dbo].Currency_CheckExists
	@CurrencyId SMALLINT = 0
	,@CurrencyCode NVARCHAR(50)
AS
/*
Created: 20180222@Raymond.Le
Task: Check Currency item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Currency WITH (NOLOCK)
	WHERE CurrencyCode = @CurrencyCode
		AND CurrencyId <> @CurrencyId
END