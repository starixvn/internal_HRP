
CREATE PROCEDURE [dbo].[City_GetByCountryId] @CountryId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get city by country id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT CityId
		,Code
		,[Name]
		,IsActive
	FROM dbo.City WITH (NOLOCK)
	WHERE CountryId = @CountryId
		AND ISNULL(@IsActive, IsActive) = IsActive
END