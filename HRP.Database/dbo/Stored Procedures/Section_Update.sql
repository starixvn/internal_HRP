
CREATE PROCEDURE [dbo].[Section_Update] @SectionId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180201@Raymond.Le
Task: Update Section
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
		,@CostCenterId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Section	
	SELECT TOP 1 @CostCenterId = CostCenterId
	FROM dbo.Section WITH (NOLOCK)
	WHERE SectionId = @SectionId

	IF EXISTS (
			SELECT 1
			FROM dbo.Section WITH (NOLOCK)
			WHERE Code = @Code
				AND CostCenterId = @CostCenterId
				AND SectionId <> @SectionId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	-- Check if Section is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Section WITH (NOLOCK)
			WHERE SectionId = @SectionId
				AND IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.Employment WITH (NOLOCK)
				WHERE SectionId = @SectionId
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

	SET @xml = (
			SELECT SectionId AS '@sid'
				,Code AS '@c'
				,Name AS '@n'
				,CostCenterId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Section WITH (NOLOCK)
			WHERE SectionId = @SectionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Section WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE SectionId = @SectionId

	--Write Log
	SET @xml = (
			SELECT SectionId AS '@sid'
				,Code AS '@c'
				,Name AS '@n'
				,CostCenterId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Section WITH (NOLOCK)
			WHERE SectionId = @SectionId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Section_Update'
		,@ObjectId = @SectionId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END