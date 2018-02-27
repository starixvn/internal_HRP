
CREATE PROCEDURE [dbo].[Country_Create] @Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Create new item of Country field
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
		,@CountryId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Country
	IF EXISTS (
			SELECT 1
			FROM dbo.Country WITH (NOLOCK)
			WHERE Code = @Code
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].Country (
		Code
		,Name
		,IsActive
		)
	VALUES (
		@Code
		,@Name
		,@IsActive
		)

	SET @CountryId = SCOPE_IDENTITY()
	--Write Log
	SET @xml = (
			SELECT CountryId AS '@cid'
				,Code AS '@c'
				,Name AS '@n'
				,IsActive AS '@ia'
			FROM dbo.Country WITH (NOLOCK)
			WHERE CountryId = @CountryId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Country_Create'
		,@ObjectId = @CountryId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END