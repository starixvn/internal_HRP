
CREATE PROCEDURE [dbo].[JobGrade_GetByGradeLevelId] @GradeLevelId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180116@Tim.Trinh
Task: Get job grade by grade level
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
	FROM dbo.JobGrade AS jg WITH (NOLOCK)
		INNER JOIN dbo.GradeLevel AS gl WITH (NOLOCK) ON jg.GradeLevelId = gl.GradeLevelId
	WHERE gl.GradeLevelId = @GradeLevelId
		AND ISNULL(@IsActive, jg.IsActive) = gl.IsActive
END