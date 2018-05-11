
CREATE PROCEDURE [dbo].[Employee_GetSuggestionByName] @Name VARCHAR(70)
	,@UserId INT
AS
/*
Created: 20180115@Oliver.Dang
Task: get suggestion by nickname or fullname
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'EmployeeManagement'

	SELECT DISTINCT TOP 10 emp.EmployeeId
		,emp.Nickname
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON uf.CompanyId = e.CompanyId
		AND uf.IsRead = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.FunctionKey = @functionKey
	WHERE uf.UserId = @UserId
		AND (
			emp.Nickname LIKE '%' + @Name + '%'
			OR emp.FullName LIKE '%' + @Name + '%'
			)
END