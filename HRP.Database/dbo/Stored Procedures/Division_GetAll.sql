-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/29/2018
-- Description:	Get all division
-- =============================================
CREATE PROCEDURE [dbo].[Division_GetAll]
	@IsActive BIT = NULL
AS
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