
CREATE PROCEDURE [dbo].[Payslip_GetDetails] @UserId INT
	,@CompanyId SMALLINT = 0
	,@PayrollIds VARCHAR(MAX) = NULL
AS
/*
Created: 20180328@Tim.Trinh
Task: get payslip details
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'Payslip'

	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, @functionKey) = 0
	BEGIN
		RETURN
	END

	CREATE TABLE #tblPayrollId (Id SMALLINT)

	INSERT INTO #tblPayrollId (Id)
	SELECT val
	FROM dbo.strSplit(@PayrollIds, ',')

	SELECT mp.PayrollId
		,mp.[Month]
		,emp.FullName
		,emp.Nickname
		,c.Code + em.EmployeeCode AS IDNo
		,em.Email
		,(CASE 
			WHEN ISNULL(c.[Name], '') <> '' THEN c.[Name]
			ELSE c.Code END
			) AS CompanyName
		,em.PDFPassword
		,mbs.CurrencyId AS SalaryCurrencyId
		,scur.CurrencyCode AS SalaryCurrencyCode
		,mbs.AfterAdjustmentAmount AS SalaryAmount
		,mba.CurrencyId AS AllowanceCurrencyId
		,acur.CurrencyCode AS AllowanceCurrencyCode
		,mba.AfterAdjustmentAmount AS AllowanceAmount
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.MonthlyBasicSalary AS mbs WITH (NOLOCK) ON mp.PayrollId = mbs.PayrollId
		INNER JOIN dbo.MonthlyBasicAllowance AS mba WITH (NOLOCK) ON mp.PayrollId = mba.PayrollId
		INNER JOIN #tblPayrollId AS tbl WITH (NOLOCK) ON mp.PayrollId = tbl.Id
		LEFT JOIN dbo.Currency AS scur WITH (NOLOCK) ON mbs.CurrencyId = scur.CurrencyId
		LEFT JOIN dbo.Currency AS acur WITH (NOLOCK) ON mba.CurrencyId = acur.CurrencyId
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON mp.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		AND c.CompanyId = @CompanyId
	ORDER BY mp.PayrollId

	SELECT mp.PayrollId
		,[Type]
		,NetSalary
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.MonthlySalaryAddition AS msa WITH (NOLOCK) ON mp.PayrollId = msa.PayrollId
		INNER JOIN #tblPayrollId AS tbl WITH (NOLOCK) ON msa.PayrollId = tbl.Id
	ORDER BY mp.PayrollId
		,msa.[Type]

	SELECT mp.PayrollId
		,[Type]
		,NetSalary
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.MonthlySalaryDeduction AS msd WITH (NOLOCK) ON mp.PayrollId = msd.PayrollId
		INNER JOIN #tblPayrollId AS tbl WITH (NOLOCK) ON msd.PayrollId = tbl.Id
	ORDER BY mp.PayrollId
		,msd.[Type]

	SELECT mp.PayrollId
		,[Type]
		,maa.NetAllowance
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.MonthlyAllowanceAddition AS maa WITH (NOLOCK) ON mp.PayrollId = maa.PayrollId
		INNER JOIN #tblPayrollId AS tbl WITH (NOLOCK) ON maa.PayrollId = tbl.Id
	ORDER BY mp.PayrollId
		,maa.[Type]

	SELECT mp.PayrollId
		,[Type]
		,mad.NetAllowance
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.MonthlyAllowanceDeduction AS mad WITH (NOLOCK) ON mp.PayrollId = mad.PayrollId
		INNER JOIN #tblPayrollId AS tbl WITH (NOLOCK) ON mad.PayrollId = tbl.Id
	ORDER BY mp.PayrollId
		,mad.[Type]

	DROP TABLE #tblPayrollId
END