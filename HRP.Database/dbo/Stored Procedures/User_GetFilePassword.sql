GO
CREATE PROCEDURE [dbo].[User_GetFilePassword]
	@UserId INT
AS
/*
Created: 20180627@EltonTon
Task: get file password by id
DB: bodb_HRP
Original:
*/
BEGIN
	SET NOCOUNT ON

	SELECT u.FilePassword
	FROM dbo.[User] AS u WITH (NOLOCK)
	WHERE u.UserId = @UserId
END
