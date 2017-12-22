SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_GetByUsername] @Username VARCHAR(30)
AS
/*
Created: 20171201@Oliver.Dang
Task: Get User by UserId.
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
		,Phone
		,[Password]
		,IsActive
		,IsOTPRegistered
		,SecretKey
		,IsSystemAdmin
	FROM [dbo].[User] WITH (NOLOCK)
	WHERE Username = @Username
END

GO


