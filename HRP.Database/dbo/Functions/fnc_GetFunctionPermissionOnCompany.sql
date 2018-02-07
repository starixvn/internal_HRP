
CREATE FUNCTION [dbo].[fnc_GetFunctionPermissionOnCompany] (
	@UserId INT
	,@CompanyId SMALLINT
	,@FunctionKey VARCHAR(50)
	)
RETURNS TINYINT
AS
/*
Created: 20180207@Tim.Trinh
Task : get function permission on company
DB: bodb_HRP

Param's Explanation:

Revisions:
*/
BEGIN
	DECLARE @policy TINYINT

	SELECT @policy = (CASE 
			WHEN uf.IsHost = 1 THEN 2
			WHEN uf.IsRead = 1 THEN 1
			ELSE 0 END
			)
	FROM dbo.[Function] AS f WITH (NOLOCK)
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON f.FunctionId = uf.FunctionId
		AND uf.CompanyId = @CompanyId
		INNER JOIN dbo.[User] AS u WITH (NOLOCK) ON uf.UserId = u.UserId
		AND u.IsActive = 1
		AND u.UserId = @UserId
	WHERE f.FunctionKey = @FunctionKey
		AND f.IsActive = 1

	SET @policy = ISNULL(@policy, 0)

	RETURN @policy
END