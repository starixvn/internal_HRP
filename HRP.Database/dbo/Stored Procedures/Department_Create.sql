-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/31/2018
-- Description:	Create new item of Department
-- =============================================
CREATE PROCEDURE [dbo].[Department_Create]
	@Code VARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@DivisionId SMALLINT
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
		,@DepartmentId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)


	SET @startRun = GETDATE()

	-- Check exists Department
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Department_CheckExists @DivisionId = @DivisionId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].Department(
		Code
		,Name
		,DivisionId
		,IsActive	
	)	VALUES (
		@Code
		,@Name
		,@DivisionId
		,@IsActive
	)

	SET @DepartmentId = SCOPE_IDENTITY()

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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Department_Create'
		,@ObjectId = @DepartmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END