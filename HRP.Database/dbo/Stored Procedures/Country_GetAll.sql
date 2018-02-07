
CREATE PROCEDURE [dbo].[Country_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20171205@Tim.Trinh
Task: Get all country
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CountryId
		,Code
		,[Name]
		,IsActive
	FROM dbo.Country WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
	ORDER BY IsActive DESC
		,Code 
END