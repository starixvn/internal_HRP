
CREATE PROCEDURE [dbo].[City_CheckExists]
	@CityId SMALLINT = 0
	,@CountryId SMALLINT = 0 
	,@Code NVARCHAR(50)
AS
/*
Created: 20180206@Raymond.Le
Task: Check City item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@CityId > 0)
	BEGIN
		SELECT TOP 1 @CountryId = CountryId 
		FROM dbo.City WITH (NOLOCK) 
		WHERE CityId = @CityId
	END

	SELECT 1
	FROM dbo.City WITH (NOLOCK)
	WHERE Code = @Code
		AND CountryId = @CountryId
		AND CityId <> @CityId
END