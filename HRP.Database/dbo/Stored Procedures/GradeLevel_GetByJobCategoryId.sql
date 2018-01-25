
CREATE PROCEDURE [dbo].[GradeLevel_GetByJobCategoryId] @JobCategoryId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180116@Tim.Trinh
Task: Get grade level by job category
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT gl.GradeLevelId
		,gl.[Name]
		,gl.[Description]
		,gl.IsActive
	FROM dbo.GradeLevel AS gl WITH (NOLOCK)
		INNER JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON jc.JobCategoryId = gl.JobCategoryId
	WHERE gl.JobCategoryId = @JobCategoryId
		AND ISNULL(@IsActive, gl.IsActive) = gl.IsActive
END