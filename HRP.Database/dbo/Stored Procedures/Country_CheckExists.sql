
CREATE PROCEDURE [dbo].[Country_CheckExists]
	@CountryId SMALLINT = 0
	,@Code NVARCHAR(50)
AS
/*
Created: 20180207@Raymond.Le
Task: Check Country item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Country WITH (NOLOCK)
	WHERE Code = @Code
		AND CountryId <> @CountryId
END