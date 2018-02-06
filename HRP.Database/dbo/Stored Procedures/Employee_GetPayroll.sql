

CREATE PROCEDURE [dbo].[Employee_GetPayroll] @EmployeeId INT
	,@UserId INT
	,@CanRead BIT OUT
	,@CanUpdate BIT OUT
AS
/*
Created: 20180112@Elton.Ton
Task: Get employee payroll
DB: bodb_HRP
Original:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT @CanRead = uf.IsRead
		,@CanUpdate = uf.IsHost
	FROM dbo.UserFunction uf WITH (NOLOCK)
		INNER JOIN dbo.Employment e WITH (NOLOCK) ON e.CompanyId = uf.CompanyId
		AND e.EmployeeId = @EmployeeId
	WHERE uf.UserId = @UserId
		AND uf.FunctionId = 7

	IF @CanRead = 0 RETURN

	SELECT e.EmployeeId
		,e.FullName
		,e.NickName
		,em.CompanyCode + em.EmployeeCode AS 'Code'
		,em.CompanyId
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
	WHERE e.EmployeeId = @EmployeeId

	SELECT epbs.EmployeeId
		,epbs.CurrencyCode
		,epbs.Salary
		,epbs.WorkingDays
		,epbs.EbankingCurrencyCode
		,epbs.EbankingAmount
		,epbs.EbankingAccountId
		,eba.AccountName AS 'EbankingAccountName'
		,epbs.DirectBanking1CurrencyCode
		,epbs.DirectBanking1Amount
		,epbs.DirectBanking1AccountId
		,epbs.DirectBanking1BankTypeId
		,ISNULL(dba1.AccountName, rr1.Value) AS 'DirectBanking1AccountName'
		,epbs.DirectBanking2CurrencyCode
		,epbs.DirectBanking2Amount
		,epbs.DirectBanking2AccountId
		,epbs.DirectBanking2BankTypeId
		,ISNULL(dba2.AccountName, rr2.Value) AS 'DirectBanking2AccountName'
		,epbs.CashCurrencyCode
		,epbs.CashAmount
		,epbs.CashAccountId
		,epbs.AutoCalculateOn
		,epbsh.EffectiveDate
		,epbsh.ReasonId
	FROM dbo.EmployeePayrollBasicSalary AS epbs WITH (NOLOCK)
		INNER JOIN (
		SELECT TOP 1 EmployeeId
			,EffectiveDate
			,ReasonId
		FROM dbo.EmployeePayrollBasicSalaryHistory WITH (NOLOCK)
		WHERE EmployeeId = @EmployeeId
		ORDER BY HistoryId DESC
		) AS epbsh ON epbs.EmployeeId = epbsh.EmployeeId
		LEFT JOIN dbo.BankAccount AS eba WITH (NOLOCK) ON epbs.EbankingAccountId = eba.Id
		LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON epbs.DirectBanking1AccountId = dba1.Id
		AND epbs.DirectBanking1BankTypeId <> 3
		LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON epbs.DirectBanking2AccountId = dba2.Id
		AND epbs.DirectBanking2BankTypeId <> 3
		LEFT JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON epbs.DirectBanking1AccountId = rr1.ResourceId
		AND epbs.DirectBanking1BankTypeId = 3
		AND rr1.RefName = 'ContraCoAccount'
		LEFT JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON epbs.DirectBanking2AccountId = rr1.ResourceId
		AND epbs.DirectBanking2BankTypeId = 3
		AND rr2.RefName = 'ContraCoAccount'

	SELECT epba.EmployeeId
		,epba.CurrencyCode
		,epba.DirectBanking1CurrencyCode
		,epba.DirectBanking1Amount
		,epba.DirectBanking1AccountId
		,epba.DirectBanking1BankTypeId
		,ISNULL(dba1.AccountName, rr1.Value) AS 'DirectBanking1AccountName'
		,epba.DirectBanking2CurrencyCode
		,epba.DirectBanking2Amount
		,epba.DirectBanking2AccountId
		,epba.DirectBanking2BankTypeId
		,ISNULL(dba2.AccountName, rr2.Value) AS 'DirectBanking2AccountName'
		,epba.CashCurrencyCode
		,epba.CashAmount
		,epba.CashAccountId
		,epba.AutoCalculateOn
		,epba.SubsistenceCurrencyCode
		,epba.SubsistenceAmount
		,epba.HomePassageCurrencyCode
		,epba.HomePassageAmount
		,epba.HousingCurrencyCode
		,epba.HousingAmount
		,epba.MobilePhoneCurrencyCode
		,epba.MobilePhoneAmount
		,epba.HPICurrencyCode
		,epba.HPIAmount
		,epba.DutyMealCurrencyCode
		,epba.DutyMealAmount
		,epba.SecondmentCurrencyCode
		,epba.SecondmentAmount
		,epba.OthersCurrencyCode
		,epba.OthersAmount
	FROM dbo.EmployeePayrollAllowance AS epba WITH (NOLOCK)
		LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON epba.DirectBanking1AccountId = dba1.Id
		AND epba.DirectBanking1BankTypeId <> 3
		LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON epba.DirectBanking2AccountId = dba2.Id
		AND epba.DirectBanking2BankTypeId <> 3
		LEFT JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON epba.DirectBanking1AccountId = rr1.ResourceId
		AND epba.DirectBanking1BankTypeId = 3
		AND rr1.RefName = 'ContraCoAccount'
		LEFT JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON epba.DirectBanking2AccountId = rr1.ResourceId
		AND epba.DirectBanking2BankTypeId = 3
		AND rr2.RefName = 'ContraCoAccount'
	WHERE epba.EmployeeId = @EmployeeId

	SELECT epd.EmployeeId
		,epd.RoomingChargesCurrencyCode
		,epd.RoomingChargesAmount
		,epd.FitnessFirstCurrencyCode
		,epd.FitnessFirstAmount
	FROM dbo.EmployeePayrollDeduction AS epd WITH (NOLOCK)
	WHERE epd.EmployeeId = @EmployeeId
END