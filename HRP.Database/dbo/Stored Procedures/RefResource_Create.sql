
CREATE PROCEDURE [dbo].[RefResource_Create] @RefName VARCHAR(100)
	,@Value NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180208@Raymond.Le
Task: Create new item of RefResource
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
		,@RefResourceId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists RefResource
	IF EXISTS (
			SELECT 1
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefName = @RefName
				AND Value = @Value
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Value + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].RefResource (
		RefName
		,ResourceId
		,Value
		,[Description]
		,IsActive
		)
	VALUES (
		@RefName
		,ISNULL((
				SELECT MAX(r.ResourceId)
				FROM dbo.RefResource AS r WITH (NOLOCK)
				WHERE r.RefName = @RefName
				), 0) + 1
		,@Value
		,@Description
		,@IsActive
		)

	SET @RefResourceId = SCOPE_IDENTITY()
	--Write Log
	SET @xml = (
			SELECT RefResourceId AS '@rrid'
				,RefName AS '@rn'
				,ResourceId AS '@reid'
				,Value AS '@v'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefResourceId = @RefResourceId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'RefResource_Create'
		,@ObjectId = @RefResourceId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END