SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_GetByTokenResetPassword] @TokenResetPassword VARCHAR(250)
AS
/*
Created: 20171201@Oliver.Dang
Task: Get User by TokenResetPassword.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT UserId
		,Username
		,FullName
		,Email
		,[Password]
		,IsActive
		,IsOTPRegistered
		,SecretKey
	FROM [dbo].[User] WITH (NOLOCK)
	WHERE TokenResetPassword = @TokenResetPassword
END

GO


