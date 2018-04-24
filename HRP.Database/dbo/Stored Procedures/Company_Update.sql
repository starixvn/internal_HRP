
CREATE PROCEDURE [dbo].[Company_Update] @CompanyId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180126@Raymond.Le
Task: Update Company BVI
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

	-- Check exists Company
	IF EXISTS (
			SELECT 1
			FROM dbo.Company WITH (NOLOCK)
			WHERE Code = @Code
				AND CompanyId <> @CompanyId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	-- Check if Company is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Company AS c WITH (NOLOCK)
			WHERE c.CompanyId = @CompanyId
				AND c.IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM (
					SELECT 1 AS i
					FROM dbo.Division WITH (NOLOCK)
					WHERE CompanyId = @CompanyId
					
					UNION
					
					SELECT 1 AS i
					FROM dbo.Employment WITH (NOLOCK)
					WHERE CompanyId = @CompanyId
					) u
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

	SET @xml = (
			SELECT c.CompanyId AS '@cid'
				,c.Code AS '@c'
				,c.Name AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.[Company] AS c WITH (NOLOCK)
			WHERE c.CompanyId = @CompanyId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Company
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE CompanyId = @CompanyId
	--Write Log
	SET @xml = (
			SELECT c.CompanyId AS '@cid'
				,c.Code AS '@c'
				,c.Name AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.[Company] AS c WITH (NOLOCK)
			WHERE c.CompanyId = @CompanyId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Company_Update'
		,@ObjectId = @CompanyId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END