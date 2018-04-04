
CREATE PROCEDURE [dbo].[Employee_GetSuggestionByIDNo] @IDNo VARCHAR(30)
AS
/*
Created: 20180105@Tim.Trinh
Task: get suggestion
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 10 emp.EmployeeId
		,emp.NickName
		,emp.FullName
		,(c.Code + em.EmployeeCode) AS IDNo
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
	WHERE c.Code + em.EmployeeCode LIKE '%' + ISNULL(@IDNo, '') + '%'
END