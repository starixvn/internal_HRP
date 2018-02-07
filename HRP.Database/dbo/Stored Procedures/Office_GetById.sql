
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/07/2018
-- Description:	Get Office By Id 
-- =============================================
Create PROCEDURE [dbo].Office_GetById @OfficeId SMALLINT
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
	LEFT JOIN dbo.City AS city WITH (NOLOCK) ON city.CityId = o.CityId
	LEFT JOIN dbo.Country AS co WITH (NOLOCK) ON co.CountryId = city.CountryId
	WHERE o.OfficeId = @OfficeId
END