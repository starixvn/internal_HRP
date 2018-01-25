
CREATE PROCEDURE [dbo].[User_Logout] @OnUserId INT
	,@IP VARCHAR(50) = ''
	,@Platform VARCHAR(255) = ''
AS
/*
Created: 20171221@Tim.Trinh
Task: User logout
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @userId INT = 0
		,@logTime DATETIME = GETDATE()

	SELECT @userId = UserId
	FROM dbo.OnUser WITH (NOLOCK)
	WHERE Id = @OnUserId

	DELETE
	FROM dbo.OnUser
	WHERE Id = @OnUserID

	IF @userId > 0
	BEGIN
		INSERT INTO dbo.LogUserLogin (
			UserId
			,LogTime
			,IP
			,[Platform]
			,ActionTypeId
			)
		VALUES (
			@userId
			,@logTime
			,@IP
			,@Platform
			,2
			)
	END
END