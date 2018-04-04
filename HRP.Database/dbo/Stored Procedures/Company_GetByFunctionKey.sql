
CREATE PROCEDURE [dbo].[Company_GetByFunctionKey] @UserId INT
	,@FunctionKey VARCHAR(50)
AS
/*
Created: 20180328@Tim.Trinh
Task: get company by function key
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT c.CompanyId
		,c.Code
		,c.[Name]
		,c.IsActive
	FROM dbo.Company AS c WITH (NOLOCK)
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
		AND uf.UserId = @UserId
		AND uf.IsRead = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.FunctionKey = @FunctionKey
	WHERE c.IsActive = 1
END