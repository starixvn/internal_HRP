
CREATE PROCEDURE [dbo].[Payslip_Get] @UserId INT
	,@Month DATE
	,@CompanyId SMALLINT = 0
	,@IsSent BIT = NULL
AS
/*
Created: 20180328@Tim.Trinh
Task: get payslip
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'Payslip'

	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, @functionKey) <> 2
	BEGIN
		RETURN
	END

	SELECT DISTINCT mp.PayrollId
		,c.Code + em.EmployeeCode AS IDNo
		,emp.Nickname
		,emp.FullName
		,c.Code + '/' + di.Code + '/' + dept.Code + '/' + cc.Code + '/' + section.Code AS Section
		,em.Email
		,emp.OverseasPhoneNo AS Phone
		,(CASE 
			WHEN ms.[Status] > ma.[Status] THEN ms.[Status]
			ELSE ma.[Status] END
			) AS PayrollStatus
		,payslip.IsSent
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		AND c.CompanyId = @CompanyId
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON em.DivisionId = di.DivisionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON em.DepartmentId = dept.DepartmentId
		INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON em.CostCenterId = cc.CostCenterId
		INNER JOIN dbo.Section AS section WITH (NOLOCK) ON em.SectionId = section.SectionId
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON emp.EmployeeId = mp.EmployeeId
		AND MONTH(mp.[Month]) = MONTH(@Month)
		AND YEAR(mp.[Month]) = YEAR(@Month)
		INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
		INNER JOIN dbo.MonthlyAllowance AS ma WITH (NOLOCK) ON mp.PayrollId = ma.PayrollId
		LEFT JOIN dbo.Payslip AS payslip WITH (NOLOCK) ON mp.PayrollId = payslip.PayrollId
	WHERE @IsSent IS NULL
		OR @IsSent = payslip.IsSent
END