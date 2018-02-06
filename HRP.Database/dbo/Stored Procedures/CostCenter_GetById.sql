
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Get CostCenter by cost center id
-- =============================================
Create PROCEDURE [dbo].[CostCenter_GetById] @CostCenterId SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT cc.CostCenterId
		,cc.Code
		,cc.[Name]
		,cc.IsActive
		,cc.DepartmentId
		,de.Code AS DepartmentCode
		,di.DivisionId
		,di.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.CostCenter AS cc WITH (NOLOCK)
	LEFT JOIN dbo.Department AS de WITH (NOLOCK) ON cc.DepartmentId = de.DepartmentId
	LEFT JOIN dbo.Division AS di WITH (NOLOCK) ON de.DivisionId = di.DivisionId
	LEFT JOIn dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = di.CompanyId
	WHERE cc.CostCenterId = @CostCenterId
END