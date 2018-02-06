
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Get grade level by GradeLevelId
-- =============================================
Create PROCEDURE [dbo].GradeLevel_GetById @GradeLevelId SMALLINT
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
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON g.JobCategoryId = jc.JobCategoryId
	WHERE g.GradeLevelId = @GradeLevelId
END