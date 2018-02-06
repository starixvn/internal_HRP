-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Get all GradeLevel
-- =============================================
CREATE PROCEDURE [dbo].[GradeLevel_GetAll]
	@IsActive BIT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT g.GradeLevelId
		,g.[Name]
		,g.[Description]
		,g.IsActive
		,g.JobCategoryId
		,jc.[Name] AS JobCategoryName
	FROM dbo.GradeLevel AS g WITH (NOLOCK)
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON jc.JobCategoryId = g.JobCategoryId
	WHERE ISNULL(@IsActive, g.IsActive) = g.IsActive
	ORDER BY g.IsActive DESC
			,g.[Name]
END