
CREATE PROCEDURE [dbo].[GradeLevel_CheckExists]
	@GradeLevelId SMALLINT = 0
	,@JobCategoryId SMALLINT = 0 
	,@Name NVARCHAR(50)
AS
/*
Created: 20180201@Raymond.Le
Task: Check GradeLevel item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@GradeLevelId > 0)
	BEGIN
		SELECT TOP 1 @JobCategoryId= JobCategoryId 
		FROM dbo.GradeLevel WITH (NOLOCK) 
		WHERE GradeLevelId = @GradeLevelId
	END

	SELECT 1
	FROM dbo.GradeLevel WITH (NOLOCK)
	WHERE Name = @Name
		AND JobCategoryId = @JobCategoryId
		AND GradeLevelId <> @GradeLevelId
END