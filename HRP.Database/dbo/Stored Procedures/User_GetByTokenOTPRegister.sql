
CREATE PROCEDURE [dbo].[User_GetByTokenOTPRegister] @TokenOTPRegister VARCHAR(250)
AS
/*
Created: 20171201@Oliver.Dang
Task: Get User by TokenResetPassword.
DB: bodb_HRP
Original:

Revisions:
    20171211@Hunter.Tran: Update param name to match with table column name
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
	WHERE TokenOTPRegister = @TokenOTPRegister
END