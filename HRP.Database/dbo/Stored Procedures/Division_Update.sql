
CREATE PROCEDURE [dbo].[Division_Update] @DivisionId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180131@Raymond.Le
Task: Update Division
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
		,@CompanyId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Division
	SELECT TOP 1 @CompanyId = CompanyId
	FROM dbo.Division WITH (NOLOCK)
	WHERE DivisionId = @DivisionId

	IF EXISTS (
			SELECT 1
			FROM dbo.Division WITH (NOLOCK)
			WHERE Code = @Code
				AND CompanyId = @CompanyId
				AND DivisionId <> @DivisionId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	-- Check if Division is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Division WITH (NOLOCK)
			WHERE DivisionId = @DivisionId
				AND IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM (
					SELECT 1 AS i
					FROM dbo.Department WITH (NOLOCK)
					WHERE DivisionId = @DivisionId
					
					UNION
					
					SELECT 1 AS i
					FROM dbo.Employment WITH (NOLOCK)
					WHERE DivisionId = @DivisionId
					) u
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

	SET @xml = (
			SELECT DivisionId AS '@did'
				,Code AS '@c'
				,Name AS '@n'
				,IsActive AS '@ia'
				,CompanyId AS '@cid'
			FROM dbo.Division WITH (NOLOCK)
			WHERE DivisionId = @DivisionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Division WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE DivisionId = @DivisionId

	--Write Log
	SET @xml = (
			SELECT DivisionId AS '@did'
				,Code AS '@c'
				,Name AS '@n'
				,IsActive AS '@ia'
				,CompanyId AS '@cid'
			FROM dbo.Division WITH (NOLOCK)
			WHERE DivisionId = @DivisionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Division_Update'
		,@ObjectId = @DivisionId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END