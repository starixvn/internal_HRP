
CREATE PROCEDURE [dbo].[Currency_Create] @CurrencyCode NVARCHAR(50)
	,@CurrencyName NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180222@Raymond.Le
Task: Create Currency
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
		,@CurrencyId SMALLINT

	SET @startRun = GETDATE()

	-- Check exists Currency
	IF EXISTS (
			SELECT 1
			FROM dbo.Currency WITH (NOLOCK)
			WHERE CurrencyCode = @CurrencyCode
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @CurrencyCode + ''' has already existed!'

		RETURN
	END

	INSERT INTO [dbo].Currency (
		CurrencyCode
		,CurrencyName
		,IsActive
		,[Order]
		)
	VALUES (
		@CurrencyCode
		,@CurrencyName
		,@IsActive
		,ISNULL((
				SELECT MAX(c.[Order])
				FROM dbo.Currency AS c WITH (NOLOCK)
				), 0) + 1
		)

	SET @CurrencyId = SCOPE_IDENTITY()
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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Currency_Create'
		,@ObjectId = @CurrencyId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END