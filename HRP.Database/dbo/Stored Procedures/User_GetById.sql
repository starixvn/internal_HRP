CREATE PROCEDURE [dbo].[User_GetById] @UserId INT
AS
/*
Created: 20171211@EltonTon
Task: get user by id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT u.UserId
		,u.Username
		,u.FullName
		,u.Phone
		,u.Email
		,u.IsActive
		,u.IsSystemAdmin
	FROM dbo.[User] AS u WITH (NOLOCK)
	WHERE u.UserId = @UserId
END