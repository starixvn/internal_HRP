SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_UpdateTokenOTPRegister] @Username VARCHAR(30)
	,@TokenOTPRegister VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update Token to register OTP.
DB: bodb_HRP
Original:

Revisions:
    20171211@Hunter.Tran: Update TokenOTPRegister column name to match with SP name
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET TokenOTPRegister = @TokenOTPRegister
	WHERE Username = @Username
END

GO


