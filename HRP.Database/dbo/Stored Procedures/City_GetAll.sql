
CREATE PROCEDURE [dbo].[City_GetAll]
	@IsActive BIT = NULL
AS
/*
Created: 20180206@Raymond.Le
Task: Get all City
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT city.CityId
		,city.Code
		,city.[Name]
		,city.IsActive
		,city.CountryId
		,co.Code AS CountryCode
	FROM dbo.City AS city WITH (NOLOCK)
	LEFT JOIN dbo.Country AS co WITH (NOLOCK) ON co.CountryId = city.CountryId
	WHERE ISNULL(@IsActive, city.IsActive) = city.IsActive
	ORDER BY city.IsActive DESC
			,city.Code
END