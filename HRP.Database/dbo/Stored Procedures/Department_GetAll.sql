
CREATE PROCEDURE [dbo].[Department_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20180207@Raymond.Le
Task: Get all department
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT dp.DepartmentId
		,dp.Code
		,dp.[Name]
		,dp.IsActive
		,dp.DivisionId
		,dv.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Department AS dp WITH (NOLOCK)
	LEFT JOIN dbo.Division AS dv WITH (NOLOCK) ON dv.DivisionId = dp.DivisionId
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON dv.CompanyId = c.CompanyId
	WHERE dp.IsActive = ISNULL(@IsActive, dp.IsActive)
	ORDER BY dp.IsActive DESC
		,dp.Code
END