
CREATE PROCEDURE [dbo].[Department_Update] @DepartmentId SMALLINT
	,@Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Update Department
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
		,@DivisionId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Department
	SELECT TOP 1 @DivisionId = DivisionId
	FROM dbo.Department WITH (NOLOCK)
	WHERE DepartmentId = @DepartmentId

	IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE Code = @Code
				AND DivisionId = @DivisionId
				AND DepartmentId <> @DepartmentId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	-- Check if Department is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
				AND IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM (
					SELECT 1 AS i
					FROM dbo.CostCenter WITH (NOLOCK)
					WHERE DepartmentId = @DepartmentId
					
					UNION
					
					SELECT 1 AS i
					FROM dbo.Employment WITH (NOLOCK)
					WHERE DepartmentId = @DepartmentId
					) u
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

	SET @xml = (
			SELECT DepartmentId AS '@deid'
				,Code AS '@c'
				,Name AS '@n'
				,DivisionId AS '@diid'
				,IsActive AS '@ia'
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Department	WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE DepartmentId = @DepartmentId

	--Write Log
	SET @xml = (
			SELECT DepartmentId AS '@deid'
				,Code AS '@c'
				,Name AS '@n'
				,DivisionId AS '@diid'
				,IsActive AS '@ia'
			FROM dbo.Department WITH (NOLOCK)
			WHERE DepartmentId = @DepartmentId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Department_Update'
		,@ObjectId = @DepartmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END