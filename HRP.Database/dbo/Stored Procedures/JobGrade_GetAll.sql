
CREATE PROCEDURE [dbo].[JobGrade_GetAll]
	@IsActive BIT = NULL
AS
/*
Created: 20180206@Raymond.Le
Task: Get all Job grade
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT jg.JobGradeId
		,jg.[Name]
		,jg.[Description]
		,jg.IsActive
		,jg.GradeLevelId
		,gl.[Name] AS GradeLevelName
		,jc.JobCategoryId
		,jc.[Name] AS JobCategoryName
	FROM dbo.JobGrade AS jg WITH (NOLOCK)
	LEFT JOIN dbo.GradeLevel AS gl WITH (NOLOCK) ON jg.GradeLevelId = gl.GradeLevelId
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON jc.JobCategoryId = gl.JobCategoryId
	WHERE ISNULL(@IsActive, jg.IsActive) = jg.IsActive
	ORDER BY jg.IsActive DESC
			,jg.[Name]
END