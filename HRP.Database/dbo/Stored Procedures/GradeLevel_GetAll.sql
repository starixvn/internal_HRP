
CREATE PROCEDURE [dbo].[GradeLevel_GetAll]
	@IsActive BIT = NULL
AS
/*
Created: 20180201@Raymond.Le
Task: Get all GradeLevel 
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
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