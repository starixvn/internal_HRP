
CREATE PROCEDURE [dbo].[MonthlyPayroll_Get] @UserId INT
	,@Month DATE
	,@CompanyId SMALLINT
	,@CostCenterIds VARCHAR(MAX) = NULL
	,@EmployeeId INT = NULL
	,@Employee NVARCHAR(70) = NULL
	,@IDNo VARCHAR(30) = NULL
	,@WorkingDays TINYINT = NULL
AS
/*
Created: 20180305@Tim.Trinh
Task: get monthly payroll
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'
		,@startDate DATE
		,@dueDate DATE
		,@today DATE = GETDATE()
		,@readPolicy TINYINT = 1

	CREATE TABLE #tblCostCenterId (Id SMALLINT)

	CREATE TABLE #tblMonthlyPayroll (
		PayrollId INT
		,CompanyId SMALLINT
		,EmployeeId INT
		,IDNo NVARCHAR(100)
		,Nickname VARCHAR(30)
		,FullName NVARCHAR(70)
		,CostCenter NVARCHAR(50)
		,WorkingDays TINYINT
		,Policy TINYINT
		,[Status] TINYINT
		)

	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@Month)

	IF @CostCenterIds IS NULL
	BEGIN
		INSERT INTO #tblCostCenterId (Id)
		SELECT DISTINCT cc.CostCenterId
		FROM dbo.CostCenter AS cc WITH (NOLOCK)
			INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON cc.DepartmentId = dept.DepartmentId
			AND dept.IsActive = 1
			INNER JOIN dbo.Division AS div WITH (NOLOCK) ON dept.DivisionId = div.DivisionId
			AND div.IsActive = 1
			INNER JOIN dbo.Company AS c WITH (NOLOCK) ON div.CompanyId = c.CompanyId
			AND c.CompanyId = @CompanyId
			AND c.IsActive = 1
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
			AND uf.UserId = @UserId
			AND uf.IsRead = 1
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @functionKey
			AND f.IsActive = 1
		WHERE cc.IsActive = 1
	END
	ELSE
	BEGIN
		INSERT INTO #tblCostCenterId (Id)
		SELECT val
		FROM dbo.strSplit(@CostCenterIds, ',')
	END

	INSERT INTO #tblMonthlyPayroll (
		PayrollId
		,CompanyId
		,EmployeeId
		,IDNo
		,Nickname
		,FullName
		,CostCenter
		,WorkingDays
		,Policy
		,[Status]
		)
	SELECT mp.PayrollId
		,c.CompanyId
		,emp.EmployeeId
		,c.Code + em.EmployeeCode AS IDNo
		,emp.Nickname
		,emp.FullName
		,cc.Code AS CostCenter
		,mbs.WorkingDays
		,(CASE 
			WHEN @today BETWEEN @startDate
					AND @dueDate THEN policy.Policy
			ELSE @readPolicy END
			) AS Policy
		,CASE 
			WHEN ms.[Status] > ma.[Status] THEN ms.[Status]
			ELSE ma.[Status]
			END AS [Status]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		AND c.CompanyId = @CompanyId
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON c.CompanyId = di.CompanyId
		AND em.DivisionId = di.DivisionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON di.DivisionId = dept.DivisionId
		AND em.DepartmentId = dept.DepartmentId
		INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON dept.DepartmentId = cc.DepartmentId
		AND em.CostCenterId = cc.CostCenterId
		INNER JOIN #tblCostCenterId AS tbl WITH (NOLOCK) ON cc.CostCenterId = tbl.Id
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON emp.EmployeeId = mp.EmployeeId
		AND MONTH(mp.[Month]) = MONTH(@Month)
		AND YEAR(mp.[Month]) = YEAR(@Month)
		INNER JOIN dbo.MonthlyBasicSalary AS mbs WITH (NOLOCK) ON mp.PayrollId = mbs.PayrollId
		AND ISNULL(@WorkingDays, mbs.WorkingDays) = mbs.WorkingDays
		INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
		INNER JOIN dbo.MonthlyAllowance AS ma WITH (NOLOCK) ON mp.PayrollId = ma.PayrollId
		INNER JOIN (
		SELECT uf.CompanyId
			,(CASE 
				WHEN uf.IsHost = 1 THEN 2
				WHEN uf.IsRead = 1 THEN 1
				ELSE 0 END
				) AS Policy
		FROM dbo.UserFunction AS uf WITH (NOLOCK)
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @functionKey
			AND f.IsActive = 1
		WHERE uf.UserId = @UserId
			AND uf.CompanyId = @CompanyId
		) AS policy ON c.CompanyId = policy.CompanyId
	WHERE (
			(
				@EmployeeId IS NOT NULL
				AND @EmployeeId = emp.EmployeeId
				)
			OR (
				@EmployeeId IS NULL
				AND (
					emp.FullName LIKE '%' + ISNULL(@Employee, '') + '%'
					OR emp.Nickname LIKE '%' + ISNULL(@Employee, '') + '%'
					)
				)
			)
		AND c.Code + em.EmployeeCode LIKE '%' + ISNULL(@IDNo, '') + '%'

	SELECT PayrollId
		,CompanyId
		,EmployeeId
		,IDNo
		,Nickname
		,FullName
		,CostCenter
		,WorkingDays
		,Policy
		,[Status]
	FROM #tblMonthlyPayroll WITH (NOLOCK)
	ORDER BY IDNo

	SELECT mp.PayrollId
		,ms.CurrencyId
		,ms.Salary
		,ms.EbankingCurrencyId
		,ec.CurrencyCode AS EbankingCurrencyCode
		,ms.EbankingAmount
		,ms.DirectBanking1CurrencyId
		,db1c.CurrencyCode AS DirectBanking1CurrencyCode
		,ms.DirectBanking1Amount
		,ms.DirectBanking2CurrencyId
		,db2c.CurrencyCode AS DirectBanking2CurrencyCode
		,ms.DirectBanking2Amount
		,ms.CashCurrencyId
		,cashc.CurrencyCode AS CashCurrencyCode
		,ms.CashAmount
		,ms.AutoCalculateOn
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN #tblMonthlyPayroll AS tbl WITH (NOLOCK) ON mp.PayrollId = tbl.PayrollId
		INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
		LEFT JOIN dbo.Currency AS ec WITH (NOLOCK) ON ms.EbankingCurrencyId = ec.CurrencyId
		LEFT JOIN dbo.Currency AS db1c WITH (NOLOCK) ON ms.DirectBanking1CurrencyId = db1c.CurrencyId
		LEFT JOIN dbo.Currency AS db2c WITH (NOLOCK) ON ms.DirectBanking2CurrencyId = db2c.CurrencyId
		LEFT JOIN dbo.Currency AS cashc WITH (NOLOCK) ON ms.CashCurrencyId = cashc.CurrencyId

	SELECT mp.PayrollId
		,ma.CurrencyId
		,ma.TotalAmount AS Salary
		,ma.DirectBanking1CurrencyId
		,db1c.CurrencyCode AS DirectBanking1CurrencyCode
		,ma.DirectBanking1Amount
		,ma.DirectBanking2CurrencyId
		,db2c.CurrencyCode AS DirectBanking2CurrencyCode
		,ma.DirectBanking2Amount
		,ma.CashCurrencyId
		,cashc.CurrencyCode AS CashCurrencyCode
		,ma.CashAmount
		,ma.AutoCalculateOn
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN #tblMonthlyPayroll AS tbl WITH (NOLOCK) ON mp.PayrollId = tbl.PayrollId
		INNER JOIN dbo.MonthlyAllowance AS ma WITH (NOLOCK) ON mp.PayrollId = ma.PayrollId
		LEFT JOIN dbo.Currency AS db1c WITH (NOLOCK) ON ma.DirectBanking1CurrencyId = db1c.CurrencyId
		LEFT JOIN dbo.Currency AS db2c WITH (NOLOCK) ON ma.DirectBanking2CurrencyId = db2c.CurrencyId
		LEFT JOIN dbo.Currency AS cashc WITH (NOLOCK) ON ma.CashCurrencyId = cashc.CurrencyId

	DROP TABLE #tblMonthlyPayroll

	DROP TABLE #tblCostCenterId
END