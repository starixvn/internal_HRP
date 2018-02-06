
-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/31/2018
-- Description:	Get Department by department id
-- =============================================
CREATE PROCEDURE [dbo].[Department_GetById] @DepartmentId SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT de.DepartmentId
		,de.Code
		,de.[Name]
		,de.IsActive
		,de.DivisionId
		,di.Code AS DivisionCode
		,c.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Department AS de WITH (NOLOCK)
	LEFT JOIN dbo.Division AS di WITH (NOLOCK) ON de.DivisionId = di.DivisionId
	LEFT JOIn dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = di.CompanyId
	WHERE de.DepartmentId = @DepartmentId
END