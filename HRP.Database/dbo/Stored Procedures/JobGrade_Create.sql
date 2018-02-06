-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Create new item of Grade Level
-- =============================================
Create PROCEDURE [dbo].JobGrade_Create
	@Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@GradeLevelId SMALLINT
	,@IsActive BIT  = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT	
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML
		,@JobGradeId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)


	SET @startRun = GETDATE()

	-- Check exists Department
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.JobGrade_CheckExists @GradeLevelId = @GradeLevelId,
								@Name = @Name
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Name +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].JobGrade(		
		Name
		,[Description]
		,IsActive	
		,GradeLevelId
	)	VALUES (
		@Name
		,@Description
		,@IsActive
		,@GradeLevelId
	)

	SET @JobGradeId = SCOPE_IDENTITY()

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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'JobGrade_Create'
		,@ObjectId = @JobGradeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END