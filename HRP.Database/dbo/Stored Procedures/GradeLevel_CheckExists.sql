-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Check GradeLevel item existed
-- =============================================
Create PROCEDURE [dbo].[GradeLevel_CheckExists]
	@GradeLevelId SMALLINT = 0
	,@JobCategoryId SMALLINT = 0 
	,@Name NVARCHAR(50)
AS
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