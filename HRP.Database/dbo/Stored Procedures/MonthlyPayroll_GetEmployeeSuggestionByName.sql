
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetEmployeeSuggestionByName] @Name VARCHAR(70) = ''
	,@UserId INT
	,@CompanyId SMALLINT
AS
/*
Created: 20180308@Tim.Trinh
Task: get suggestion by nickname or fullname
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'

	SELECT DISTINCT TOP 10 emp.EmployeeId
		,emp.Nickname
		,emp.FullName
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
		AND e.CompanyId = @CompanyId
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON uf.CompanyId = e.CompanyId
		AND uf.UserId = @UserId
		AND uf.IsRead = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.FunctionKey = @functionKey
	WHERE emp.Nickname LIKE '%' + ISNULL(@Name, '') + '%'
		OR emp.FullName LIKE '%' + ISNULL(@Name, '') + '%'
END