-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Get all Section
-- =============================================
Create PROCEDURE [dbo].[Section_GetAll]
		@IsActive BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT s.SectionId
		,s.Code
		,s.[Name]
		,s.IsActive
		,s.CostCenterId
		,cc.Code AS CostCenterCode
		,dp.DepartmentId
		,dp.Code AS DepartmentCode
		,dv.DivisionId
		,dv.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Section AS s WITH (NOLOCK) 
	LEFT JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON cc.CostCenterId = s.CostCenterId
	LEFT JOIN dbo.Department AS dp WITH (NOLOCK) ON dp.DepartmentId = cc.DepartmentId
	LEFT JOIN dbo.Division AS dv WITH (NOLOCK) ON dv.DivisionId = dp.DivisionId
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON dv.CompanyId = c.CompanyId
	WHERE s.IsActive = ISNULL(@IsActive, s.IsActive)
	ORDER BY s.IsActive DESC
		,s.Code
END