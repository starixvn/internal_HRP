
CREATE PROCEDURE [dbo].[City_Update] @CityId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180206@Raymond.Le
Task: Update city
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
		,@CountryId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists City
	SELECT TOP 1 @CountryId = CountryId
	FROM dbo.City WITH (NOLOCK)
	WHERE CityId = @CityId

	IF EXISTS (
			SELECT 1
			FROM dbo.City WITH (NOLOCK)
			WHERE Code = @Code
				AND CountryId = @CountryId
				AND CityId <> @CityId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

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
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].City
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE CityId = @CityId
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

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'City_Update'
		,@ObjectId = @CityId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

END