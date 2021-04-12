
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
		,@unsatisfied TINYINT = 2
		,@firstDayOfWorkingMonth DATE
		,@startDate DATE
		,@dueDate DATE
		,@today DATE = GETDATE()
		,@previousMonth DATE
		,@readPolicy TINYINT = 1

	SET @firstDayOfWorkingMonth = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month), 0)
	SET @previousMonth = DATEADD(MONTH, -1, @today)

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
		,IsGenerated BIT
		,HasSettingSalary BIT
		,HasSettingAllowance BIT
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
		SELECT DISTINCT cc.CostCenterId
		FROM dbo.strSplit(@CostCenterIds, ',') AS tbl
			INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON ISNUMERIC(tbl.val) = 1
			AND tbl.val = cc.CostCenterId
			AND cc.IsActive = 1
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
		,IsGenerated
		,HasSettingSalary
		,HasSettingAllowance
		,Policy
		,[Status]
		)
	SELECT mp.PayrollId
		,c.CompanyId
		,emp.EmployeeId
		,c.Code + employment.EmployeeCode AS IDNo
		,emp.Nickname
		,emp.FullName
		,cc.Code AS CostCenter
		,mbs.WorkingDays
		,(CASE 
			WHEN mp.PayrollId IS NULL THEN 0
			ELSE 1 END
			) AS IsGenerated
		,(CASE 
			WHEN ISNULL(ms.EbankingCurrencyId, '') = ''
				AND ISNULL(ms.EbankingAmount, '') = ''
				AND ISNULL(ms.DirectBanking1CurrencyId, '') = ''
				AND ISNULL(ms.DirectBanking1Amount, '') = ''
				AND ISNULL(ms.DirectBanking2CurrencyId, '') = ''
				AND ISNULL(ms.DirectBanking2Amount, '') = ''
				AND ISNULL(ms.CashCurrencyId, '') = ''
				AND ISNULL(ms.CashAmount, '') = ''
				AND ISNULL(ms.CurrencyId, '') = ''
				AND ISNULL(ms.Salary, '') = '' THEN 0
			ELSE 1 END
			) AS HasSettingSalary
		,(CASE 
			WHEN ISNULL(ma.DirectBanking1CurrencyId, '') = ''
				AND ISNULL(ma.DirectBanking1Amount, '') = ''
				AND ISNULL(ma.DirectBanking2CurrencyId, '') = ''
				AND ISNULL(ma.DirectBanking2Amount, '') = ''
				AND ISNULL(ma.CashCurrencyId, '') = ''
				AND ISNULL(ma.CashAmount, '') = ''
				AND ISNULL(ma.CurrencyId, '') = ''
				AND ISNULL(ma.TotalAmount, '') = '' THEN 0
			ELSE 1 END
			) AS HasSettingAllowance
		,(CASE 
			WHEN (@previousMonth BETWEEN @startDate AND @dueDate)
				OR (@today BETWEEN @startDate AND @dueDate) THEN policy.Policy
			ELSE @readPolicy END
			) AS Policy
		,(CASE 
			WHEN ISNULL(ms.[Status], @unsatisfied) > ISNULL(ma.[Status], @unsatisfied) THEN ISNULL(ms.[Status], @unsatisfied)
			ELSE ISNULL(ma.[Status], @unsatisfied) END
			) AS [Status]
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS employment WITH (NOLOCK) ON emp.EmployeeId = employment.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON employment.CompanyId = c.CompanyId
		AND c.CompanyId = @CompanyId
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON c.CompanyId = di.CompanyId
		AND employment.DivisionId = di.DivisionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON di.DivisionId = dept.DivisionId
		AND employment.DepartmentId = dept.DepartmentId
		INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON dept.DepartmentId = cc.DepartmentId
		AND employment.CostCenterId = cc.CostCenterId
		INNER JOIN #tblCostCenterId AS tbl WITH (NOLOCK) ON cc.CostCenterId = tbl.Id
		LEFT JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON emp.EmployeeId = mp.EmployeeId
		AND MONTH(mp.[Month]) = MONTH(@Month)
		AND YEAR(mp.[Month]) = YEAR(@Month)
		LEFT JOIN dbo.MonthlyBasicSalary AS mbs WITH (NOLOCK) ON mp.PayrollId = mbs.PayrollId
		AND ISNULL(@WorkingDays, mbs.WorkingDays) = mbs.WorkingDays
		LEFT JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
		LEFT JOIN dbo.MonthlyAllowance AS ma WITH (NOLOCK) ON mp.PayrollId = ma.PayrollId
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
		AND c.Code + employment.EmployeeCode LIKE '%' + ISNULL(@IDNo, '') + '%'
		AND employment.GroupStartDate IS NOT NULL
		AND employment.GroupStartDate <= @dueDate
		AND (
			employment.GroupEndDate IS NULL
			OR employment.GroupEndDate > @firstDayOfWorkingMonth
			)

	SELECT PayrollId
		,CompanyId
		,EmployeeId
		,IDNo
		,Nickname
		,FullName
		,CostCenter
		,WorkingDays
		,IsGenerated
		,HasSettingSalary
		,HasSettingAllowance
		,Policy
		,[Status]
	FROM #tblMonthlyPayroll WITH (NOLOCK)
	ORDER BY IsGenerated
		,HasSettingSalary
		,HasSettingAllowance
		,IDNo

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
