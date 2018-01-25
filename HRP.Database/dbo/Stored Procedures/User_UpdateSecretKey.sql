
CREATE PROCEDURE [dbo].[User_UpdateSecretKey] @Username VARCHAR(30)
	,@SecretKey VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171211@Hunter.Tran
Task: Update user's secret key to verify with OTP
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET SecretKey = @SecretKey
	WHERE Username = @Username
END