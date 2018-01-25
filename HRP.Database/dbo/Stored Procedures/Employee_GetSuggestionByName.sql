
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

	SELECT DISTINCT TOP 10 emp.EmployeeId
		,emp.Nickname
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
		LEFT JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON uf.CompanyId = e.CompanyId
	WHERE @UserId = uf.UserId
		AND uf.FunctionId = 5
		AND uf.IsRead = 1
		AND (
			emp.Nickname LIKE '%' + @Name + '%'
			OR emp.FullName LIKE '%' + @Name + '%'
			)
END