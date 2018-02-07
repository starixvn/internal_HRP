
-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/07/2018
-- Description:	Update Office
-- =============================================
Create PROCEDURE [dbo].Office_Update @OfficeId SMALLINT
	,@Code VARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Department
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Office_CheckExists @OfficeId = @OfficeId,
								@Code = @Code
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code + ' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT OfficeId AS '@oid'
				,Code AS '@c'
				,Name AS '@n'
				,CityId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Office WITH (NOLOCK)
			WHERE OfficeId = @OfficeId
			FOR XML PATH('r')
				,TYPE
			)

	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Office WITH (ROWLOCK,UPDLOCK)
	SET Code = @Code
		,Name = @Name
		,IsActive = @IsActive
	WHERE OfficeId = @OfficeId

	--Write Log
	SET @xml = (
			SELECT OfficeId AS '@oid'
				,Code AS '@c'
				,Name AS '@n'
				,CityId AS '@cid'
				,IsActive AS '@ia'
			FROM dbo.Office WITH (NOLOCK)
			WHERE OfficeId = @OfficeId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Office_Update'
		,@ObjectId = @OfficeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists
END