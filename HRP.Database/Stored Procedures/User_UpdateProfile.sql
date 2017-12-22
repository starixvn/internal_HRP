SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_UpdateProfile] @UserId INT
	,@FullName VARCHAR(70)
	,@Phone VARCHAR(20)
	,@Email VARCHAR(250)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update User profile.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET FullName = @FullName
		,Phone = @Phone
		,Email = @Email
	WHERE UserId = @UserId
END

GO


