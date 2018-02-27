
CREATE PROCEDURE [dbo].[CostCenter_Update] @CostCenterId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180201@Raymond.Le
Task: Update Cost Center
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
		,@DepartmentId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Cost Center
	SELECT TOP 1 @DepartmentId = DepartmentId
	FROM dbo.CostCenter WITH (NOLOCK)
	WHERE CostCenterId = @CostCenterId

	IF EXISTS (
			SELECT 1
			FROM dbo.CostCenter WITH (NOLOCK)
			WHERE Code = @Code
				AND DepartmentId = @DepartmentId
				AND CostCenterId <> @CostCenterId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

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

	UPDATE [dbo].CostCenter
	WITH (ROWLOCK,UPDLOCK)
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
END