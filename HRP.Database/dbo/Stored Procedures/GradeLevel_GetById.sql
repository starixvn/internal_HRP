
CREATE PROCEDURE [dbo].[GradeLevel_GetById] @GradeLevelId SMALLINT
AS
/*
Created: 20180201@Raymond.Le
Task: Get grade level by GradeLevelId
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
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON g.JobCategoryId = jc.JobCategoryId
	WHERE g.GradeLevelId = @GradeLevelId
END