
CREATE PROCEDURE [dbo].[User_GetUsers] @IsActive BIT = NULL
AS
/*
Created: 20171205@EltonTon
Task : Get Users based on status
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
		,u.CreatedDate
		,u.Email
		,cb.Username AS CreatedBy
		,u.IsActive
		,u.IsSystemAdmin
	FROM [dbo].[User] AS u WITH (NOLOCK)
		INNER JOIN dbo.[User] AS cb WITH (NOLOCK) ON u.CreatedBy = cb.UserId
	WHERE ISNULL(@IsActive, u.IsActive) = u.IsActive
	ORDER BY u.Username
END