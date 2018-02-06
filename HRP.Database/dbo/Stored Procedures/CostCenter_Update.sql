
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Update Cost Center
-- =============================================
Create PROCEDURE [dbo].[CostCenter_Update] @CostCenterId SMALLINT
	,@Code VARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
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
	EXEC dbo.CostCenter_CheckExists @CostCenterId = @CostCenterId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code + ' has already existed!'

		RETURN
	END

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

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].CostCenter WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE CostCenterId = @CostCenterId

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

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'CostCenter_Update'
		,@ObjectId = @CostCenterId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END