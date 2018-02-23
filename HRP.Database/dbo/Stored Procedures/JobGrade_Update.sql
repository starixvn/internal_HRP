
CREATE PROCEDURE [dbo].[JobGrade_Update] @JobGradeId SMALLINT
	,@Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180206@Raymond.Le
Task: Update Job Grade
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

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Job Grade
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.JobGrade_CheckExists @JobGradeId = @JobGradeId,
								@Name = @Name
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Name + ' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT JobGradeId AS '@jid'
				,Name AS '@n'
				,[Description] AS '@d'
				,GradeLevelId AS '@gid'
				,IsActive AS '@ia'
			FROM dbo.JobGrade WITH (NOLOCK)
			WHERE JobGradeId = @JobGradeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].JobGrade WITH (ROWLOCK,UPDLOCK)
	SET Name = @Name
		,[Description] = @Description
		,IsActive = @IsActive
	WHERE JobGradeId = @JobGradeId

	--Write Log
	SET @xml = (
			SELECT JobGradeId AS '@jid'
				,Name AS '@n'
				,[Description] AS '@d'
				,GradeLevelId AS '@gid'
				,IsActive AS '@ia'
			FROM dbo.JobGrade WITH (NOLOCK)
			WHERE JobGradeId = @JobGradeId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'JobGrade_Update'
		,@ObjectId = @JobGradeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END