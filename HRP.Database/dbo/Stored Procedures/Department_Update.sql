
CREATE PROCEDURE [dbo].[Department_Update] @DepartmentId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Update Department
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

	-- Check exists Department
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Department_CheckExists @DepartmentId = @DepartmentId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT DepartmentId AS '@deid'
				,Code AS '@c'
				,Name AS '@n'
				,DivisionId AS '@diid'
				,IsActive AS '@ia'
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
			FOR XML PATH('r')
				,TYPE
			)

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Department WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE DepartmentId = @DepartmentId

	--Write Log
	SET @xml = (
			SELECT DepartmentId AS '@deid'
				,Code AS '@c'
				,Name AS '@n'
				,DivisionId AS '@diid'
				,IsActive AS '@ia'
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Department_Update'
		,@ObjectId = @DepartmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END