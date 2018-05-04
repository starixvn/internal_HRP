CREATE PROCEDURE [dbo].[Report_GetEmployeeDataForDynamicReport] 
	@EmployeeStatusIds VARCHAR(200)
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
	,epbs.Salary
	,epbs.CurrencyId
	,bsc.CurrencyCode AS 'BasicSalaryCurrency'
	,epbs.EbankingCurrencyId
	,epbs.EbankingAmount
	,epbs.DirectBanking1CurrencyId
	,epbs.DirectBanking1Amount
	,epbs.DirectBanking2CurrencyId
	,epbs.DirectBanking2Amount
	,epbs.CashCurrencyId
	,epbs.CashAmount
	,epbs.AutoCalculateOn
	,epa.CurrencyId
	,epa.DirectBanking1CurrencyId
	,epa.DirectBanking1Amount
	,epa.DirectBanking2CurrencyId
	,epa.DirectBanking2Amount
	,epa.CashCurrencyId
	,epa.CashAmount
	,epa.AutoCalculateOn
	,epa.SubsistenceAmount
	,epa.SubsistenceCurrencyId
	,sstc.CurrencyCode AS 'SubsistenceCurrency'
	,epa.HousingAmount
	,epa.HousingCurrencyId
	,hc.CurrencyCode AS 'HousingCurrency'
	,epa.HomePassageAmount
	,epa.HomePassageCurrencyId
	,hpc.CurrencyCode AS 'HomePassageCurrency'
	,epa.DutyMealAmount
	,epa.DutyMealCurrencyId
	,dmc.CurrencyCode AS 'DutyMealCurrency'
	,epa.HPIAmount
	,epa.HPICurrencyId
	,hpic.CurrencyCode AS 'HPICurrency'
	,epa.MobilePhoneAmount
	,epa.MobilePhoneCurrencyId
	,mpc.CurrencyCode AS 'MobilePhoneCurrency'
	,epa.OthersAmount
	,epa.OthersCurrencyId
	,otc.CurrencyCode AS 'OthersCurrency'
	,epa.SecondmentAmount
	,epa.SecondmentCurrencyId
	,smc.CurrencyCode AS 'SecondmentCurrency'
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
	INNER JOIN dbo.Division AS d WITH (NOLOCK) on em.DivisionId = d.DivisionId
	INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) on em.CostCenterId = cc.CostCenterId
	INNER JOIN dbo.Department AS dm WITH (NOLOCK) ON em.DepartmentId = dm.DepartmentId
	INNER JOIN dbo.Section AS s WITH (NOLOCK) ON em.SectionId = s.SectionId

	LEFT JOIN dbo.EmployeePayrollBasicSalary AS epbs WITH (NOLOCK) ON e.EmployeeId = epbs.EmployeeId
	LEFT JOIN dbo.Currency AS bsc WITH (NOLOCK) ON epbs.CurrencyId = bsc.CurrencyId

	LEFT JOIN dbo.EmployeePayrollAllowance AS epa WITH (NOLOCK) ON e.EmployeeId = epa.EmployeeId
	LEFT JOIN dbo.Currency AS sstc WITH (NOLOCK) ON epa.SubsistenceCurrencyId = sstc.CurrencyId
	LEFT JOIN dbo.Currency AS hc WITH (NOLOCK) ON epa.HousingCurrencyId = hc.CurrencyId
	LEFT JOIN dbo.Currency AS hpc WITH (NOLOCK) ON epa.HomePassageCurrencyId = hpc.CurrencyId
	LEFT JOIN dbo.Currency AS dmc WITH (NOLOCK) ON epa.DutyMealCurrencyId = dmc.CurrencyId
	LEFT JOIN dbo.Currency AS hpic WITH (NOLOCK) ON epa.HPICurrencyId = hpic.CurrencyId
	LEFT JOIN dbo.Currency AS mpc WITH (NOLOCK) ON epa.MobilePhoneCurrencyId = mpc.CurrencyId
	LEFT JOIN dbo.Currency AS otc WITH (NOLOCK) ON epa.OthersCurrencyId = otc.CurrencyId
	LEFT JOIN dbo.Currency AS smc WITH (NOLOCK) ON epa.SecondmentCurrencyId = smc.CurrencyId

	WHERE em.CompanyId = @CompanyId 
	AND EXISTS (
			SELECT 1
			FROM dbo.strSplit('1,2', ',') AS s
			WHERE em.EmploymentStatus = s.val
		)
END