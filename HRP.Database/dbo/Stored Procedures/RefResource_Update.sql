
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/08/2018
-- Description:	Update RefResource
-- =============================================
CREATE PROCEDURE [dbo].[RefResource_Update] 
	@RefName VARCHAR(100)
	,@ResourceId SMALLINT
	,@Value NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
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
		,@RefResourceId INT

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists RefResource
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.RefResource_CheckExists @RefName = @RefName
		,@ResourceId = @ResourceId
		,@Value = @Value

	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Value + ' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT RefResourceId AS '@rrid'
				,RefName AS '@rn'
				,ResourceId AS '@reid'
				,Value AS '@v'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefName = @RefName
				AND ResourceId = @ResourceId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].RefResource WITH (ROWLOCK,UPDLOCK)
	SET Value = @Value
		,[Description] = @Description
		,IsActive = @IsActive
		,@RefResourceId = RefResourceId
	WHERE RefName = @RefName
		AND ResourceId = @ResourceId

	--Write Log
	SET @xml = (
			SELECT RefResourceId AS '@rrid'
				,RefName AS '@rn'
				,ResourceId AS '@reid'
				,Value AS '@v'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefName = @RefName
				AND ResourceId = @ResourceId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'RefResource_Update'
		,@ObjectId = @RefResourceId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists 
END