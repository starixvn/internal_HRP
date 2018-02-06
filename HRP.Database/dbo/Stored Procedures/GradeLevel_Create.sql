-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Create new item of Grade Level
-- =============================================
Create PROCEDURE [dbo].[GradeLevel_Create]
	@Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@JobCategoryId SMALLINT
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
		,@GradeLevelId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)


	SET @startRun = GETDATE()

	-- Check exists Department
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.GradeLevel_CheckExists @JobCategoryId = @JobCategoryId,
								@Name = @Name
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Name +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].GradeLevel(		
		Name
		,[Description]
		,IsActive	
		,JobCategoryId
	)	VALUES (
		@Name
		,@Description
		,@IsActive
		,@JobCategoryId
	)

	SET @GradeLevelId = SCOPE_IDENTITY()

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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'GradeLevel_Create'
		,@ObjectId = @GradeLevelId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END