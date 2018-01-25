
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
		,(em.CompanyCode + em.EmployeeCode) AS IDNo
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
	WHERE em.CompanyCode + em.EmployeeCode LIKE '%' + @IDNo + '%'
END