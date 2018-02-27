
CREATE PROCEDURE [dbo].[GradeLevel_Update] @GradeLevelId SMALLINT
	,@Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180201@Raymond.Le
Task: Update Grade level
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@JobCategoryId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Grade Level
	SELECT TOP 1 @JobCategoryId = JobCategoryId
	FROM dbo.GradeLevel WITH (NOLOCK)
	WHERE GradeLevelId = @GradeLevelId

	IF EXISTS (
			SELECT 1
			FROM dbo.GradeLevel WITH (NOLOCK)
			WHERE Name = @Name
				AND JobCategoryId = @JobCategoryId
				AND GradeLevelId <> @GradeLevelId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Name + ''' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT GradeLevelId AS '@gid'
				,Name AS '@n'
				,[Description] AS '@d'
				,JobCategoryId AS '@jid'
				,IsActive AS '@ia'
			FROM dbo.GradeLevel WITH (NOLOCK)
			WHERE GradeLevelId = @GradeLevelId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].GradeLevel	WITH (ROWLOCK,UPDLOCK)
	SET Name = @Name
		,[Description] = @Description
		,IsActive = @IsActive
	WHERE GradeLevelId = @GradeLevelId

	--Write Log
	SET @xml = (
			SELECT GradeLevelId AS '@gid'
				,Name AS '@n'
				,[Description] AS '@d'
				,JobCategoryId AS '@jid'
				,IsActive AS '@ia'
			FROM dbo.GradeLevel WITH (NOLOCK)
			WHERE GradeLevelId = @GradeLevelId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'GradeLevel_Update'
		,@ObjectId = @GradeLevelId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END