
CREATE PROCEDURE [dbo].[Department_Create] @Code NVARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@DivisionId SMALLINT
	,@IsActive BIT = 0
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180207@Raymond.Le
Task: Create new item of Department
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
		,@DepartmentId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Department									
	IF EXISTS (
			SELECT 1
			FROM dbo.Department WITH (NOLOCK)
			WHERE Code = @Code
				AND DivisionId = @DivisionId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @Code + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].Department (
		Code
		,Name
		,DivisionId
		,IsActive
		)
	VALUES (
		@Code
		,@Name
		,@DivisionId
		,@IsActive
		)

	SET @DepartmentId = SCOPE_IDENTITY()
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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Department_Create'
		,@ObjectId = @DepartmentId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END