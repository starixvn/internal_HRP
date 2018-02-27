
CREATE PROCEDURE [dbo].[Office_Update] @OfficeId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Update Office
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
		,@CityId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Office
	SELECT TOP 1 @CityId = CityId
	FROM dbo.Office WITH (NOLOCK)
	WHERE OfficeId = @OfficeId

	IF EXISTS (
			SELECT 1
			FROM dbo.Office WITH (NOLOCK)
			WHERE Code = @Code
				AND CityId = @CityId
				AND OfficeId <> @OfficeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

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
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Office	WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE OfficeId = @OfficeId

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

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Office_Update'
		,@ObjectId = @OfficeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END