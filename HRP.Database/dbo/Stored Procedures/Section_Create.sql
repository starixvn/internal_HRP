
CREATE PROCEDURE [dbo].[Section_Create]
	@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@CostCenterId SMALLINT
	,@IsActive BIT  = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT	
AS
/*
Created: 20180201@Raymond.Le
Task: Create new item of Section
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
		,@SectionId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Section
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Section_CheckExists @CostCenterId = @CostCenterId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].Section(
		Code
		,Name
		,CostCenterId
		,IsActive	
	)	VALUES (
		@Code
		,@Name
		,@CostCenterId
		,@IsActive
	)

	SET @SectionId = SCOPE_IDENTITY()

	--Write Log
	SET @xml = (
			SELECT SectionId AS '@sid'
				,Code AS '@c'
				,Name AS '@n'
				,CostCenterId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Section WITH (NOLOCK)
			WHERE SectionId = @SectionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Section_Create'
		,@ObjectId = @SectionId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END