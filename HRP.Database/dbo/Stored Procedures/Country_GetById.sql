
CREATE PROCEDURE [dbo].[Country_GetById] 
	@CountryId SMALLINT = NULL
AS
/*
Created: 20180207@Raymond.Le
Task: Get Country by id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CountryId
		,Code
		,Name
		,IsActive
	FROM dbo.Country WITH (NOLOCK)
	WHERE CountryId = @CountryId

END