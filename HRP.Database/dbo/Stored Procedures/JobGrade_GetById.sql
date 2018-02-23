
CREATE PROCEDURE [dbo].[JobGrade_GetById] @JobGradeId SMALLINT
AS
/*
Created: 20180206@Raymond.Le
Task: Get JobGrade By Id
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
		,g.[Name] AS GradeLevelName
		,jc.JobCategoryId
		,jc.Name AS JobCategoryName
	FROM dbo.JobGrade AS jg WITH (NOLOCK) 
	LEFT JOIN dbo.GradeLevel AS g WITH (NOLOCK) ON g.GradeLevelId = jg.GradeLevelId
	LEFT JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON g.JobCategoryId = jc.JobCategoryId
	WHERE jg.JobGradeId = @JobGradeId
END