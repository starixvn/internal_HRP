-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/22/2018
-- Description:	Create new item of Company BVI field
-- =============================================
CREATE PROCEDURE [dbo].[Company_Create] 
	@Code VARCHAR(50)
	,@Name NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML
		,@CompanyId SMALLINT

	SET @startRun = GETDATE()

	IF EXISTS (SELECT 1 FROM dbo.Company WHERE Code = @Code)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ' + @Code +' has already existed!'
		RETURN
	END

	INSERT INTO [dbo].Company (
		Code
		,Name
		,IsActive	
	)	VALUES (
		@Code
		,@Name
		,@IsActive
	)

	SET @CompanyId = SCOPE_IDENTITY()
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

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Company_Create'
		,@ObjectId = @CompanyId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
	 
END