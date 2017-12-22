SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


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

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET TokenResetPassword = @TokenResetPassword
	WHERE Username = @Username
END

GO


