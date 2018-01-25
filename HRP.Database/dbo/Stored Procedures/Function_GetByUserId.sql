
CREATE PROCEDURE [dbo].[Function_GetByUserId] @UserId INT
AS
/*
Created: 20171205@Tim.Trinh
Task: Get all function permission of a user
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT uf.FunctionId
		,f.[Name]
		,f.FunctionKey
		,f.IsReadOnly
		,f.ParentId
		,uf.CompanyId
		,uf.IsRead
		,uf.IsHost
	FROM dbo.UserFunction AS uf WITH (NOLOCK)
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
	WHERE uf.UserId = @UserId
		AND f.IsActive = 1
END