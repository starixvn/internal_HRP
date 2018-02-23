
CREATE PROCEDURE [dbo].[CostCenter_GetAll]
		@IsActive BIT = NULL
AS
/*
Created: 20180201@Raymond.Le
Task: Get all Cost Centers
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT cc.CostCenterId
		,cc.Code
		,cc.[Name]
		,cc.IsActive
		,cc.DepartmentId
		,dp.Code AS DepartmentCode
		,dv.DivisionId
		,dv.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROm dbo.CostCenter AS cc WITH (NOLOCK)
	LEFT JOIN dbo.Department AS dp WITH (NOLOCK) ON dp.DepartmentId = cc.DepartmentId
	LEFT JOIN dbo.Division AS dv WITH (NOLOCK) ON dv.DivisionId = dp.DivisionId
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON dv.CompanyId = c.CompanyId
	WHERE cc.IsActive = ISNULL(@IsActive, cc.IsActive)
	ORDER BY cc.IsActive DESC
		,cc.Code
END