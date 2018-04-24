
CREATE PROCEDURE [dbo].[Country_Update] @CountryId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Update Country
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

	SET @startRun = GETDATE()

	-- Check exists Country
	IF EXISTS (
			SELECT 1
			FROM dbo.Country WITH (NOLOCK)
			WHERE Code = @Code
				AND CountryId <> @CountryId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	-- Check if Country is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Country WITH (NOLOCK)
			WHERE CountryId = @CountryId
				AND IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM (
					SELECT 1 AS i
					FROM dbo.City WITH (NOLOCK)
					WHERE CountryId = @CountryId
					
					UNION
					
					SELECT 1 AS i
					FROM dbo.Employment WITH (NOLOCK)
					WHERE CountryId = @CountryId
					) u
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

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
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Country
	WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE CountryId = @CountryId

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

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Country_Update'
		,@ObjectId = @CountryId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END