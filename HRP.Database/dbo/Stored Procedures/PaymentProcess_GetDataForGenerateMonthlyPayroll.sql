
CREATE PROCEDURE [dbo].[PaymentProcess_GetDataForGenerateMonthlyPayroll] @CompanyId SMALLINT
	,@Month DATE
	,@EmployeeId INT = NULL
AS
/*
Created: 20180316@Raymond.Le
Task: Get data for generate monthly payroll
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @FirstDayOfWorkingMonth DATE
			,@LastDayOfWorkingMonth DATE

	CREATE TABLE #tblMonthlyPayroll (
		EmployeeId INT
		,[Month] DATE
		,SalaryEffectiveDate DATE
		,SalaryChangeReasonId TINYINT
		,GroupStartDate DATE
		,GroupEndDate DATE
		)

	CREATE TABLE #tblCurrentSalary (
		[EmployeeId] INT
		,[EffectiveDate] DATE
		,[ReasonId] TINYINT
		)

	CREATE TABLE #tblLastMonthSalary (
		EmployeeId INT
		,OldCurrencyId INT
		,OldSalary NVARCHAR(MAX)
		)

	----------------------------
	SET @FirstDayOfWorkingMonth = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month), 0)
	SET @LastDayOfWorkingMonth = DATEADD(s,-1,DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month) + 1,0))

	-- Get Current Salary History id
	INSERT INTO #tblCurrentSalary (
		EmployeeId
		,EffectiveDate
		,ReasonId
		)
	SELECT h.EmployeeId
		,h.EffectiveDate
		,h.ReasonId
	FROM dbo.EmployeePayrollBasicSalaryHistory AS h WITH (NOLOCK)
	WHERE h.HistoryId = (
			SELECT MAX(HistoryId)
			FROM dbo.EmployeePayrollBasicSalaryHistory AS h1 WITH (NOLOCK)
			WHERE h1.EmployeeId = h.EmployeeId
			)
		AND h.EmployeeId = ISNULL(@EmployeeId, h.EmployeeId)

	-- Get monthly payroll
	INSERT INTO #tblMonthlyPayroll (
		EmployeeId
		,[Month]
		,SalaryEffectiveDate
		,SalaryChangeReasonId
		,GroupStartDate
		,GroupEndDate
		)
	SELECT e.EmployeeId
		,@FirstDayOfWorkingMonth AS [Month]
		,s.EffectiveDate AS SalaryEffectiveDate
		,s.ReasonId AS SalaryChangeReasonId
		,e.GroupStartDate
		,e.GroupEndDate
	FROM dbo.Employment AS e WITH (NOLOCK)
		LEFT JOIN #tblCurrentSalary AS s WITH (NOLOCK) ON e.EmployeeId = s.EmployeeId
	WHERE e.CompanyId = @CompanyId
		AND (
			(e.GroupEndDate IS NULL)
			OR (
				e.GroupEndDate IS NOT NULL
				AND e.GroupEndDate > @FirstDayOfWorkingMonth
				)
			)
		AND e.GroupStartDate IS NOT NULL
		AND e.GroupStartDate <= @LastDayOfWorkingMonth
		AND e.EmployeeId = ISNULL(@EmployeeId, e.EmployeeId)

	-- Get last month salary
	INSERT INTO #tblLastMonthSalary (
		EmployeeId
		,OldCurrencyId
		,OldSalary
		)
	SELECT tmp.EmployeeId
		,NULL
		,NULL
	FROM #tblMonthlyPayroll AS tmp WITH (NOLOCK)

	UPDATE ls
	SET ls.OldCurrencyId = s.CurrencyId
		,ls.OldSalary = s.Salary
	FROM #tblLastMonthSalary AS ls
	LEFT JOIN dbo.Employment AS e WITH (NOLOCK) ON ls.EmployeeId = e.EmployeeId
	LEFT JOIN dbo.MonthlyPayroll AS p WITH (NOLOCK) ON p.EmployeeId = ls.EmployeeId
	LEFT JOIN dbo.MonthlyBasicSalary AS s WITH (NOLOCK) ON p.PayrollId = s.PayrollId
	WHERE p.[Month] = DATEADD(MONTH, - 1, @FirstDayOfWorkingMonth)
		AND p.EmployeeId = ISNULL(@EmployeeId, p.EmployeeId)
		AND e.GroupStartDate < @FirstDayOfWorkingMonth

	-- get old salary for new employee
	UPDATE ls
	SET ls.OldCurrencyId = h.CurrencyId
		,ls.OldSalary = h.Salary
	FROM #tblLastMonthSalary AS ls
		LEFT JOIN dbo.EmployeePayrollBasicSalaryHistory AS h WITH (NOLOCK) ON ls.EmployeeId = h.EmployeeId
	WHERE ls.OldSalary IS NULL
		AND h.HistoryId = (
			SELECT MAX(HistoryId)
			FROM dbo.EmployeePayrollBasicSalaryHistory AS h1 WITH (NOLOCK)
			WHERE h1.EmployeeId = h.EmployeeId
					AND h1.EffectiveDate IS NULL
					AND h1.ReasonId IS NULL
			)

	------------RESULT--------------
	-- Get monthly payroll 
	SELECT EmployeeId
		,[Month]
		,SalaryEffectiveDate
		,SalaryChangeReasonId
		,GroupStartDate
		,GroupEndDate
	FROM #tblMonthlyPayroll

	-- Get MonthlyAttendance
	SELECT a.EmployeeId
		,a.[Month]
		,a.ActualWorkingDay
	FROM dbo.MonthlyAttendance AS a WITH (NOLOCK)
		INNER JOIN #tblMonthlyPayroll AS p WITH (NOLOCK) ON a.EmployeeId = p.EmployeeId
														AND a.[Month] = p.[Month]

	-- Get MonthlyDSTNetSalary
	SELECT d.EmployeeId
		,d.[Month]
		,d.CurrencyId
		,d.NetSalaryReceived
	FROM dbo.MonthlyDSTNetSalary AS d WITH (NOLOCK)
		INNER JOIN #tblMonthlyPayroll AS p WITH (NOLOCK) ON d.EmployeeId = p.EmployeeId
														AND d.[Month] = p.[Month]

	-- Get MonthlyBasicSalary
	SELECT p.[EmployeeId]
		,bs.[CurrencyId]
		,c1.CurrencyCode AS CurrencyCode
		,[Salary]
		,os.OldCurrencyId
		,os.OldSalary
		,p.SalaryEffectiveDate AS EffectiveDate
		,p.SalaryChangeReasonId AS SalaryChangeReason
		,[WorkingDays]
		,[EbankingCurrencyId]
		,c2.CurrencyCode AS EbankingCurrencyCode
		,[EbankingAmount]
		,[EbankingAccountId]
		,[DirectBanking1CurrencyId]
		,c3.CurrencyCode AS DirectBanking1CurrencyCode
		,[DirectBanking1Amount]
		,[DirectBanking1AccountId]
		,[DirectBanking2CurrencyId]
		,c4.CurrencyCode AS DirectBanking2CurrencyCode
		,[DirectBanking2Amount]
		,[DirectBanking2AccountId]
		,[CashCurrencyId]
		,c5.CurrencyCode AS CashCurrencyCode
		,[CashAmount]
		,[CashAccountId]
		,[AutoCalculateOn]
	FROM dbo.EmployeePayrollBasicSalary AS bs WITH (NOLOCK)
		LEFT JOIN #tblLastMonthSalary AS os WITH (NOLOCK) ON bs.EmployeeId = os.EmployeeId
		INNER JOIN #tblMonthlyPayroll AS p WITH (NOLOCK) ON bs.EmployeeId = p.EmployeeId
		LEFT JOIN dbo.Currency AS c1 WITH (NOLOCK) ON bs.CurrencyId = c1.CurrencyId
		LEFT JOIN dbo.Currency AS c2 WITH (NOLOCK) ON bs.[EbankingCurrencyId] = c2.CurrencyId
		LEFT JOIN dbo.Currency AS c3 WITH (NOLOCK) ON bs.[DirectBanking1CurrencyId] = c3.CurrencyId
		LEFT JOIN dbo.Currency AS c4 WITH (NOLOCK) ON bs.[DirectBanking2CurrencyId] = c4.CurrencyId
		LEFT JOIN dbo.Currency AS c5 WITH (NOLOCK) ON bs.[CashCurrencyId] = c5.CurrencyId

	-- Get MonthlyBasicAllowance
	SELECT p.[EmployeeId]
		,ba.[CurrencyId]
		,c1.CurrencyCode
		,[DirectBanking1CurrencyId]
		,c2.CurrencyCode AS DirectBanking1CurrencyCode
		,[DirectBanking1Amount]
		,[DirectBanking1AccountId]
		,[DirectBanking2CurrencyId]
		,c3.CurrencyCode AS DirectBanking2CurrencyCode
		,[DirectBanking2Amount]
		,[DirectBanking2AccountId]
		,[CashCurrencyId]
		,c4.CurrencyCode AS CashCurrencyCode
		,[CashAmount]
		,[CashAccountId]
		,[AutoCalculateOn]
		,[SubsistenceCurrencyId]
		,[SubsistenceAmount]
		,[HomePassageCurrencyId]
		,[HomePassageAmount]
		,[HousingCurrencyId]
		,[HousingAmount]
		,[MobilePhoneCurrencyId]
		,[MobilePhoneAmount]
		,[HPICurrencyId]
		,[HPIAmount]
		,[DutyMealCurrencyId]
		,[DutyMealAmount]
		,[SecondmentCurrencyId]
		,[SecondmentAmount]
		,[OthersCurrencyId]
		,[OthersAmount]
		,d.RoomingChargesCurrencyId
		,d.RoomingChargesAmount
		,d.FitnessFirstCurrencyId
		,d.FitnessFirstAmount
	FROM [dbo].[EmployeePayrollAllowance] AS ba WITH (NOLOCK)
		LEFT JOIN dbo.EmployeePayrollDeduction AS d WITH (NOLOCK) ON ba.EmployeeId = d.EmployeeId
		INNER JOIN #tblMonthlyPayroll AS p WITH (NOLOCK) ON ba.EmployeeId = p.EmployeeId
		LEFT JOIN dbo.Currency AS c1 WITH (NOLOCK) ON ba.CurrencyId = c1.CurrencyId 
		LEFT JOIN dbo.Currency AS c2 WITH (NOLOCK) ON ba.[DirectBanking1CurrencyId] = c2.CurrencyId
		LEFT JOIN dbo.Currency AS c3 WITH (NOLOCK) ON ba.[DirectBanking2CurrencyId] = c3.CurrencyId
		LEFT JOIN dbo.Currency AS c4 WITH (NOLOCK) ON ba.[CashCurrencyId] = c4.CurrencyId

	-- Get currency exchange Rate
	SELECT r.ExchangeRate
		,r.FromCurrencyId
		,r.ToCurrencyId
	FROM dbo.CurrencyExchangeRate AS r WITH (NOLOCK)
	WHERE r.ExchangeRateMonth = @FirstDayOfWorkingMonth

	DROP TABLE #tblMonthlyPayroll

	DROP TABLE #tblCurrentSalary

	DROP TABLE #tblLastMonthSalary
END