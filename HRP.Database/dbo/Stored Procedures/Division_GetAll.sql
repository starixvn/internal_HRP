
CREATE PROCEDURE [dbo].[Division_GetAll]
	@IsActive BIT = NULL
AS
/*
Created: 20180131@Raymond.Le
Task: Get all division
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.DivisionId
		,d.Code
		,d.[Name]
		,d.IsActive
		,d.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Division AS d WITH (NOLOCK)
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON d.CompanyId = c.CompanyId
	WHERE d.IsActive = ISNULL(@IsActive, d.IsActive)
	ORDER BY d.IsActive DESC
			,d.Code
END