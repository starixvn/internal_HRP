
CREATE PROCEDURE [dbo].[Division_Create]
	@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@CompanyId SMALLINT
	,@IsActive BIT  = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT	
AS
/*
Created: 20180131@Raymond.Le
Task: Create new item of Division
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
		,@xml XML
		,@DivisionId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Division
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Division_CheckExists @CompanyId = @CompanyId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code +''' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].Division (
		Code
		,Name
		,CompanyId
		,IsActive	
	)	VALUES (
		@Code
		,@Name
		,@CompanyId
		,@IsActive
	)

	SET @DivisionId = SCOPE_IDENTITY()

	--Write Log
	SET @xml = (
			SELECT DivisionId AS '@did'
				,Code AS '@c'
				,Name AS '@n'
				,CompanyId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Division WITH (NOLOCK)
			WHERE DivisionId = @DivisionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Division_Create'
		,@ObjectId = @DivisionId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END