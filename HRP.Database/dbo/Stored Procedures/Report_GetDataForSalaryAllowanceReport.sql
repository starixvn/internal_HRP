
CREATE PROCEDURE [dbo].[Report_GetDataForSalaryAllowanceReport] @CompanyId SMALLINT
	,@Month DATE
AS
/*
Created: 20180419@Raymond.Le
Task: Get data for Salary & Allowance Report
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblEmployeeInfo (
		EmployeeId INT
		,PayrollId INT
		)

	INSERT INTO #tblEmployeeInfo (
		EmployeeId
		,PayrollId
		)
	SELECT ep.EmployeeId
		,mp.PayrollId
	FROM dbo.Employment AS ep WITH (NOLOCK)
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON ep.EmployeeId = mp.EmployeeId
	WHERE mp.[Month] = @Month
		AND ep.CompanyId = @CompanyId

	-- Get employee basic info
	SELECT ei.EmployeeId
		,co.Code AS CompanyCode
		,di.Code AS DivisionCode
		,de.Code AS DepartmentCode
		,cc.Code AS CostCenterCode
		,se.Code AS SectionCode
		,ep.EmployeeCode
		,e.FullName
		,e.Nickname
	FROM #tblEmployeeInfo AS ei WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = ei.EmployeeId
	INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON ep.EmployeeId = ei.EmployeeId
	LEFT JOIN dbo.Company AS co WITH (NOLOCK) ON co.CompanyId = ep.CompanyId
	LEFT JOIN dbo.Division AS di WITH (NOLOCK) ON di.DivisionId = ep.DivisionId
	LEFT JOIN dbo.Department AS de WITH (NOLOCK) ON de.DepartmentId = ep.DepartmentId
	LEFT JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON cc.CostCenterId = ep.CostCenterId
	LEFT JOIN dbo.Section AS se WITH (NOLOCK) ON se.SectionId = ep.SectionId

	-- Get monthly Basic salary
	SELECT ei.EmployeeId
		,bs.CurrencyId
		,bs.Salary
	FROM #tblEmployeeInfo AS ei WITH (NOLOCK)
	LEFT JOIN dbo.MonthlyBasicSalary AS bs WITH (NOLOCK) ON bs.PayrollId = ei.PayrollId

	-- Get monthly salary
	SELECT ei.EmployeeId
		,s.CurrencyId
		,s.Salary
		,s.EbankingCurrencyId
		,s.EbankingAmount
		,s.DirectBanking1CurrencyId
		,s.DirectBanking1Amount
		,s.DirectBanking2CurrencyId
		,s.DirectBanking2Amount
		,s.CashCurrencyId
		,s.CashAmount
	FROM #tblEmployeeInfo AS ei WITH (NOLOCK)
	LEFT JOIN dbo.MonthlySalary AS s WITH (NOLOCK) ON s.PayrollId = ei.PayrollId

	-- Get basic Allowances
	SELECT ei.EmployeeId
		,ba.CurrencyId
		,c.CurrencyCode
		,ba.TotalAmount
		,ba.SubsistenceCurrencyId
		,ba.SecondmentAmount
		,ba.HomePassageCurrencyId
		,ba.HomePassageAmount
		,ba.HousingCurrencyId
		,ba.HousingAmount
		,ba.MobilePhoneCurrencyId
		,ba.MobilePhoneAmount
		,ba.HPICurrencyId
		,ba.HPIAmount
		,ba.DutyMealCurrencyId
		,ba.DutyMealAmount
		,ba.SecondmentCurrencyId
		,ba.SecondmentAmount
		,ba.OthersCurrencyId
		,ba.OthersAmount
		,ba.RoomingChargesCurrencyId
		,ba.RoomingChargesAmount
		,ba.FitnessFirstCurrencyId
		,ba.FitnessFirstAmount
	FROM #tblEmployeeInfo AS ei WITH (NOLOCK)
	LEFT JOIN dbo.MonthlyBasicAllowance AS ba WITH (NOLOCK) ON ba.PayrollId = ei.PayrollId
	LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON ba.CurrencyId = c.CurrencyId

	-- Get Allowances
	SELECT ei.EmployeeId
		,a.CurrencyId
		,a.TotalAmount
		,a.[DSTNetSalaryCurrencyId]
		,a.[DSTNetSalaryAmount]
		,a.[TotalNetAmount]
	FROM #tblEmployeeInfo AS ei WITH (NOLOCK)
	LEFT JOIN dbo.MonthlyAllowance AS a WITH (NOLOCK) ON a.PayrollId = ei.PayrollId

	-- Get currency exchange Rate
	SELECT r.ExchangeRate
		,r.FromCurrencyId
		,r.ToCurrencyId
	FROM dbo.CurrencyExchangeRate AS r WITH (NOLOCK)
	WHERE r.ExchangeRateMonth = @Month

	DROP TABLE #tblEmployeeInfo
END