
CREATE PROCEDURE [dbo].Currency_Update @CurrencyId SMALLINT
	,@CurrencyCode NVARCHAR(50)
	,@CurrencyName NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180222@Raymond.Le
Task: Update Currency
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

	CREATE TABLE #tblCheckExists (
		[Count] SMALLINT
	)

	SET @startRun = GETDATE()

	-- Check exists Currency
	INSERT INTO #tblCheckExists(
		[Count]
	)
	EXEC dbo.Currency_CheckExists @CurrencyId = @CurrencyId,
								@CurrencyCode = @CurrencyCode
									
	IF EXISTS (SELECT 1 FROM #tblCheckExists)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @CurrencyCode + ' has already existed!'

		RETURN
	END

	SET @xml = (
			SELECT c.CurrencyId AS '@cid'
				,c.CurrencyCode AS '@c'
				,c.CurrencyName AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.Currency AS c WITH (NOLOCK)
			WHERE c.CurrencyId = @CurrencyId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Currency WITH (ROWLOCK,UPDLOCK)
	SET CurrencyCode = @CurrencyCode
		,CurrencyName = @CurrencyName
		,IsActive = @IsActive
	WHERE CurrencyId = @CurrencyId

	--Write Log
	SET @xml = (
			SELECT c.CurrencyId AS '@cid'
				,c.CurrencyCode AS '@c'
				,c.CurrencyName AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.Currency AS c WITH (NOLOCK)
			WHERE c.CurrencyId = @CurrencyId
			FOR XML PATH('r')
				,TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Currency_Update'
		,@ObjectId = @CurrencyId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCheckExists 
END