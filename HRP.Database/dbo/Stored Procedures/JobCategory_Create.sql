
CREATE PROCEDURE [dbo].[JobCategory_Create] @Name NVARCHAR(50)
	,@Description NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Create new item of JobCategory field
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
		,@JobCategoryId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists JobCategory						
	IF EXISTS (
			SELECT 1
			FROM dbo.JobCategory WITH (NOLOCK)
			WHERE Name = @Name
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Name + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].JobCategory (
		Name
		,[Description]
		,IsActive
		)
	VALUES (
		@Name
		,@Description
		,@IsActive
		)

	SET @JobCategoryId = SCOPE_IDENTITY()

	--Write Log
	SET @xml = (
			SELECT JobCategoryId AS '@jid'
				,Name AS '@n'
				,[Description] AS '@d'
				,IsActive AS '@ia'
			FROM dbo.JobCategory WITH (NOLOCK)
			WHERE JobCategoryId = @JobCategoryId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'JobCategory_Create'
		,@ObjectId = @JobCategoryId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END