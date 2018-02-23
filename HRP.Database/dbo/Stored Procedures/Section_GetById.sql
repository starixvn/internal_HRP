
CREATE PROCEDURE [dbo].[Section_GetById] @SectionId SMALLINT
AS
/*
Created: 20180201@Raymond.Le
Task: Get Section by section id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT s.SectionId
		,s.Code
		,s.[Name]
		,s.IsActive
		,s.CostCenterId
		,cc.Code AS CostCenterCode
		,de.DepartmentId
		,de.Code AS DepartmentCode
		,di.DivisionId
		,di.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Section AS s WITH (NOLOCK)
	LEFT JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON s.CostCenterId = cc.CostCenterId
	LEFT JOIN dbo.Department AS de WITH (NOLOCK) ON cc.DepartmentId = de.DepartmentId
	LEFT JOIN dbo.Division AS di WITH (NOLOCK) ON de.DivisionId = di.DivisionId
	LEFT JOIn dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = di.CompanyId
	WHERE s.SectionId = @SectionId
END