
CREATE PROCEDURE [dbo].[City_Create] @Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@CountryId SMALLINT
	,@IsActive BIT = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180206@Raymond.Le
Task: Create new item of City
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
		,@CityId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists City									
	IF EXISTS (
			SELECT 1
			FROM dbo.City WITH (NOLOCK)
			WHERE Code = @Code
				AND CountryId = @CountryId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].City (
		Code
		,Name
		,IsActive
		,CountryId
		)
	VALUES (
		@Code
		,@Name
		,@IsActive
		,@CountryId
		)

	SET @CityId = SCOPE_IDENTITY()
	--Write Log
	SET @xml = (
			SELECT CityId AS '@ciid'
				,Code AS '@c'
				,Name AS '@n'
				,CountryId AS '@coid'
				,IsActive AS '@ia'
			FROM dbo.City WITH (NOLOCK)
			WHERE CityId = @CityId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'City_Create'
		,@ObjectId = @CityId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END