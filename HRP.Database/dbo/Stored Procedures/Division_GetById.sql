
-- =============================================
-- Author:		Raymond.Le
-- Create date: 30/01/2018
-- Description:	Get division by divisionId
-- =============================================
CREATE PROCEDURE [dbo].[Division_GetById] @DivisionId SMALLINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DivisionId
		,d.Code
		,d.[Name]
		,d.IsActive
		,d.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Division AS d WITH (NOLOCK)
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON d.CompanyId = c.CompanyId
	WHERE DivisionId = @DivisionId
END