
CREATE PROCEDURE [dbo].[CostCenter_Create]
	@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@DepartmentId SMALLINT
	,@IsActive BIT  = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT	
AS
/*
Created: 20180201@Raymond.Le
Task: Create new item of CostCenter
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
		,@CostCenterId SMALLINT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Cost Center
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.CostCenter_CheckExists @DepartmentId = @DepartmentId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].CostCenter(
		Code
		,Name
		,DepartmentId
		,IsActive	
	)	VALUES (
		@Code
		,@Name
		,@DepartmentId
		,@IsActive
	)

	SET @CostCenterId = SCOPE_IDENTITY()

	--Write Log
	SET @xml = (
			SELECT CostCenterId AS '@cid'
				,Code AS '@c'
				,Name AS '@n'
				,DepartmentId AS '@did'
				,IsActive AS '@ia'
			FROM dbo.CostCenter WITH (NOLOCK)
			WHERE CostCenterId = @CostCenterId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'CostCenter_Create'
		,@ObjectId = @CostCenterId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END