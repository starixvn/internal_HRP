
CREATE PROCEDURE [dbo].[User_UpdateTokenResetPassword] @Username VARCHAR(30)
	,@TokenResetPassword VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update Token to reset password.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@userId INT
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @startRun = GETDATE()
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.TokenResetPassword AS '@t'
			FROM dbo.[User] u WITH (NOLOCK)
			WHERE u.Username = @Username
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @userId = (
		SELECT u.UserId AS '@u'
			FROM dbo.[User] u WITH (NOLOCK)
			WHERE u.Username = @Username
	)

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET TokenResetPassword = @TokenResetPassword
	WHERE Username = @Username

	-- write log after update
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.TokenResetPassword AS '@t'
			FROM dbo.[User] AS u WITH (NOLOCK)
			WHERE u.Username = @Username
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @userId
		,@SPName = 'User_UpdateTokenResetPassword'
		,@ObjectId = @UserId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END