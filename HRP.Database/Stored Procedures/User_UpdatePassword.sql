SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_UpdatePassword] @UserId INT
	,@Password VARCHAR(250)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update User password.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET [Password] = @Password
		,TokenResetPassword = NULL
	WHERE UserId = @UserId
END

GO


