GO

CREATE PROCEDURE [dbo].[User_UpdateFilePassword] 
	@UserId INT
	,@Password VARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update User File password.
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
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.FilePassword AS '@fp'
			FROM dbo.[User] u WITH (NOLOCK)
			WHERE u.UserId = @UserId
			FOR XML PATH('r'), TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')


	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET [FilePassword] = @Password
	WHERE UserId = @UserId

	-- write log after update
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.FilePassword AS '@fp'
			FROM dbo.[User] AS u WITH (NOLOCK)
			WHERE u.UserId = @UserId
			FOR XML PATH('r'), TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UserId
		,@SPName = 'User_UpdateFilePassword'
		,@ObjectId = @UserId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END
