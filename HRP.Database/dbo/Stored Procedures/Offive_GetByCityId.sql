
CREATE PROCEDURE [dbo].[Offive_GetByCityId] @CityId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get office by city id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT OfficeId
		,Code
		,[Name]
		,IsActive
	FROM dbo.Office WITH (NOLOCK)
	WHERE CityId = @CityId
		AND ISNULL(@IsActive, IsActive) = IsActive
END