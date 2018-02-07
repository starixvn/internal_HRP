
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/06/2018
-- Description:	Get City By city Id
-- =============================================
Create PROCEDURE [dbo].City_GetById @CityId SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT city.CityId
		,city.Code
		,city.Name
		,city.IsActive
		,city.CountryId
		,co.Code AS CountryCode
	FROM dbo.City AS city WITH (NOLOCK)
	LEFT JOIN dbo.Country AS co WITH (NOLOCK) ON co.CountryId = city.CountryId
	WHERE city.CityId = @CityId
END