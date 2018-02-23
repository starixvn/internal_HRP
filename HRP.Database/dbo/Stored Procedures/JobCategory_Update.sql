
CREATE PROCEDURE [dbo].[JobCategory_Update] @JobCategoryId SMALLINT
	,@Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Update JobCategory
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

	-- Check exists JobCategory
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.JobCategory_CheckExists @JobCategoryId = @JobCategoryId,
								@Name = @Name
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Name + ' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT JobCategoryId AS '@jid'
				,Name AS '@n'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.JobCategory WITH (NOLOCK)
			WHERE JobCategoryId = @JobCategoryId
			FOR XML PATH('r')
				,TYPE
			)

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].JobCategory WITH (ROWLOCK,UPDLOCK)
	SET Name = @Name
		,[Description] = @Description
		,IsActive = @IsActive
	WHERE JobCategoryId = @JobCategoryId

	--Write Log
	SET @xml = (
			SELECT JobCategoryId AS '@jid'
				,Name AS '@n'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.JobCategory WITH (NOLOCK)
			WHERE JobCategoryId = @JobCategoryId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'JobCategory_Update'
		,@ObjectId = @JobCategoryId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END