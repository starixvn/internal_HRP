
CREATE PROCEDURE [dbo].[User_CheckIn] @UserId INT
	,@NetSessionId VARCHAR(30)
	,@IP VARCHAR(50) = ''
	,@Platform VARCHAR(255) = ''
	,@OnUserId INT OUTPUT
AS
/*
Created: 20171221@Tim.Trinh
Task: User check in
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logTime DATETIME = GETDATE()

	UPDATE dbo.OnUser WITH (ROWLOCK, UPDLOCK)
	SET IsKickedOut = 1
	WHERE UserId = @UserId

	INSERT INTO dbo.OnUser (
		UserId
		,IP
		,[Platform]
		,NetSessionId
		,IsKickedOut
		)
	VALUES (
		@UserId
		,@IP
		,@Platform
		,@NetSessionId
		,0
		)

	SET @OnUserId = SCOPE_IDENTITY()

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET LastLoginDate = @logTime
	WHERE UserId = @UserId

	INSERT INTO dbo.LogUserLogin (
		UserId
		,LogTime
		,IP
		,[Platform]
		,ActionTypeId
		)
	VALUES (
		@UserId
		,@logTime
		,@IP
		,@Platform
		,1
		)
END