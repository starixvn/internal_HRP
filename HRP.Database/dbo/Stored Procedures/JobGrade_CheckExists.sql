-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/06/2018
-- Description:	Check Job Grade item existed
-- =============================================
Create PROCEDURE [dbo].JobGrade_CheckExists
	@JobGradeId SMALLINT = 0 
	,@GradeLevelId SMALLINT = 0
	,@Name NVARCHAR(50)
AS
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