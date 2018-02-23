
CREATE PROCEDURE [dbo].[Employee_GetBasicInfoById] 
	@EmployeeId INT
AS
/*
Created: 20180212@Quinn.Vo
Task: Get Employee basic by EmpId
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
		,e.Nickname
		,e.FullName
		,epm.EmployeeCode
		,epm.CompanyId
		,e.MailingAddress

	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.Employment AS epm WITH (NOLOCK) ON e.EmployeeId = epm.EmployeeId
	WHERE e.EmployeeId = @EmployeeId
END