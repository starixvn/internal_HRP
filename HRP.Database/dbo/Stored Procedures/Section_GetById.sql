
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Get Section by section id
-- =============================================
Create PROCEDURE [dbo].[Section_GetById] @SectionId SMALLINT
AS
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