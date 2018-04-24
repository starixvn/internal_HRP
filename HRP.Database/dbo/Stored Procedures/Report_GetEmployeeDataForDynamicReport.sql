CREATE PROCEDURE [dbo].[Report_GetEmployeeDataForDynamicReport] 
	@Month DATE
	,@CompanyId INT
AS
/*
Created: 20180423@Elton.Ton
Task: Get report with selected column
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
	,e.FullName
	,e.Nickname
	,d.Name AS 'Division'
	,dm.Name AS 'Department'
	,cc.Name AS 'CostCenter'
	,s.Name AS 'Section'
	,mbs.AfterAdjustmentAmount AS 'ActualBasicSalaryAmount'
	,mbsc.CurrencyCode AS 'ActualBasicSalaryCurrency'
	,mbs.Salary AS 'BasicSalaryAmount'
	,mbsc.CurrencyCode AS 'BasicSalaryCurrency'
	,ms.AdditionAmount AS 'BasicSalaryAdditionAmount'
	,CASE WHEN ms.AdditionAmount IS NOT NULL AND ms.AdditionAmount <> '' THEN msc.CurrencyCode
	ELSE NULL END AS 'BasicSalaryAdditionCurrency'
	,ms.DeductionAmount AS 'BasicSalaryDeductionAmount'
	,CASE WHEN ms.DeductionAmount IS NOT NULL AND ms.DeductionAmount <> '' THEN msc.CurrencyCode
	ELSE NULL END AS 'BasicSalaryDeductionCurrency'
	,ms.Salary AS 'NetAmount'
	,msc.CurrencyCode AS 'NetCurrency'
	,mba.SubsistenceAmount
	,sstc.CurrencyCode AS 'SubsistenceCurrency'
	,mba.HousingAmount
	,hc.CurrencyCode AS 'HousingCurrency'
	,mba.HomePassageAmount
	,hpc.CurrencyCode AS 'HomePassageCurrency'
	,mba.DutyMealAmount
	,dmc.CurrencyCode AS 'DutyMealCurrency'
	,mba.HPIAmount
	,hpic.CurrencyCode AS 'HPICurrency'
	,mba.MobilePhoneAmount
	,mpc.CurrencyCode AS 'MobilePhoneCurrency'
	,mba.OthersAmount
	,otc.CurrencyCode AS 'OthersCurrency'
	,mba.SecondmentAmount
	,smc.CurrencyCode AS 'SecondmentCurrency'
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
	INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
	INNER JOIN dbo.Division AS d WITH (NOLOCK) on em.DivisionId = d.DivisionId
	INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) on em.CostCenterId = cc.CostCenterId
	INNER JOIN dbo.Department AS dm WITH (NOLOCK) ON em.DepartmentId = dm.DepartmentId
	INNER JOIN dbo.Section AS s WITH (NOLOCK) ON em.SectionId = s.SectionId

	INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
	LEFT JOIN dbo.Currency AS msc WITH (NOLOCK) ON ms.CurrencyId = msc.CurrencyId

	INNER JOIN dbo.MonthlyBasicSalary AS mbs WITH (NOLOCK) ON mp.PayrollId = mbs.PayrollId
	LEFT JOIN dbo.Currency AS mbsc WITH (NOLOCK) ON mbs.CurrencyId = mbsc.CurrencyId

	INNER JOIN dbo.MonthlyBasicAllowance AS mba WITH (NOLOCK) ON mp.PayrollId = mba.PayrollId
	LEFT JOIN dbo.Currency AS sstc WITH (NOLOCK) ON mba.SubsistenceCurrencyId = sstc.CurrencyId
	LEFT JOIN dbo.Currency AS hc WITH (NOLOCK) ON mba.HousingCurrencyId = hc.CurrencyId
	LEFT JOIN dbo.Currency AS hpc WITH (NOLOCK) ON mba.HomePassageCurrencyId = hpc.CurrencyId
	LEFT JOIN dbo.Currency AS dmc WITH (NOLOCK) ON mba.DutyMealCurrencyId = dmc.CurrencyId
	LEFT JOIN dbo.Currency AS hpic WITH (NOLOCK) ON mba.HPICurrencyId = hpic.CurrencyId
	LEFT JOIN dbo.Currency AS mpc WITH (NOLOCK) ON mba.MobilePhoneCurrencyId = mpc.CurrencyId
	LEFT JOIN dbo.Currency AS otc WITH (NOLOCK) ON mba.OthersCurrencyId = otc.CurrencyId
	LEFT JOIN dbo.Currency AS smc WITH (NOLOCK) ON mba.SecondmentCurrencyId = smc.CurrencyId


	WHERE em.CompanyId = @CompanyId AND YEAR(mp.Month) = YEAR(@Month) AND MONTH(mp.Month) = MONTH(@Month)
END