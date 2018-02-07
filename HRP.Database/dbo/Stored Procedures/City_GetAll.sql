-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/06/2018
-- Description:	Get all City
-- =============================================
create PROCEDURE [dbo].City_GetAll
	@IsActive BIT = NULL
AS
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