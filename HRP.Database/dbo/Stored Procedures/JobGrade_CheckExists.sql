
CREATE PROCEDURE [dbo].[JobGrade_CheckExists]
	@JobGradeId SMALLINT = 0 
	,@GradeLevelId SMALLINT = 0
	,@Name NVARCHAR(50)
AS
/*
Created: 20180206@Raymond.Le
Task: Check Job Grade item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@JobGradeId > 0)
	BEGIN
		SELECT TOP 1 @GradeLevelId= GradeLevelId 
		FROM dbo.JobGrade WITH (NOLOCK) 
		WHERE JobGradeId = @JobGradeId
	END

	SELECT 1
	FROM dbo.JobGrade WITH (NOLOCK)
	WHERE Name = @Name
		AND GradeLevelId = @GradeLevelId
		AND JobGradeId <> @JobGradeId
END