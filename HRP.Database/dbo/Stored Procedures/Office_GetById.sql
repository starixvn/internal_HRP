
CREATE PROCEDURE [dbo].[Office_GetById] @OfficeId SMALLINT
AS
/*
Created: 20180207@Raymond.Le
Task: Get Office By Id 
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT o.OfficeId
		,o.Code
		,o.[Name]
		,o.IsActive
		,o.CityId
		,city.Code AS CityCode
		,co.CountryId
		,co.Code AS CountryCode
	FROM dbo.Office AS o WITH (NOLOCK)
	LEFT JOIN dbo.City AS city WITH (NOLOCK) ON city.CityId = o.CityId
	LEFT JOIN dbo.Country AS co WITH (NOLOCK) ON co.CountryId = city.CountryId
	WHERE o.OfficeId = @OfficeId
END