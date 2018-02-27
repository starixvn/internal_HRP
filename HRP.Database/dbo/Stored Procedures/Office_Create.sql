
CREATE PROCEDURE [dbo].[Office_Create] @Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@CityId SMALLINT
	,@IsActive BIT = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Create new item of Office
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
		,@OfficeId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Office
	IF EXISTS (
			SELECT 1
			FROM dbo.Office WITH (NOLOCK)
			WHERE Code = @Code
				AND CityId = @CityId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].Office (
		Code
		,Name
		,CityId
		,IsActive
		)
	VALUES (
		@Code
		,@Name
		,@CityId
		,@IsActive
		)

	SET @OfficeId = SCOPE_IDENTITY()

	--Write Log
	SET @xml = (
			SELECT OfficeId AS '@oid'
				,Code AS '@c'
				,Name AS '@n'
				,CityId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Office WITH (NOLOCK)
			WHERE OfficeId = @OfficeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Office_Create'
		,@ObjectId = @OfficeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END