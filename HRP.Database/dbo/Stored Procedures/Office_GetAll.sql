-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/07/2018
-- Description:	Get all Office
-- =============================================
Create PROCEDURE [dbo].Office_GetAll
	@IsActive BIT = NULL
AS
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
	LEFT JOIN dbo.City AS city WITH (NOLOCK) ON o.CityId = city.CityId
	LEFT JOIN dbo.Country AS co WITH (NOLOCK) ON co.CountryId = city.CountryId
	WHERE ISNULL(@IsActive, o.IsActive) = o.IsActive
	ORDER BY o.IsActive DESC
			,o.Code
END