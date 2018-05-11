
CREATE PROCEDURE [dbo].[Employee_GetPayroll] @EmployeeId INT
	,@UserId INT
AS
/*
Created: 20180112@Elton.Ton
Task: Get employee payroll
DB: bodb_HRP
Original:
Updated: 
 ---- 20180417@Elton: dont need to join RefResource for contra co account, remove banktypeid
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @CanRead BIT = 0
		,@CanUpdate BIT = 0
		,@functionKey VARCHAR(50) = 'EmployeePayroll'

	SELECT @CanRead = uf.IsRead
		,@CanUpdate = uf.IsHost
	FROM dbo.UserFunction AS uf WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON e.CompanyId = uf.CompanyId
		AND e.EmployeeId = @EmployeeId
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.FunctionKey = @functionKey
	WHERE uf.UserId = @UserId

	IF @CanRead = 0 RETURN

	SELECT e.EmployeeId
		,e.FullName
		,e.NickName
		,c.Code + em.EmployeeCode AS 'Code'
		,em.CompanyId
		,@CanRead AS CanRead
		,@CanUpdate AS CanUpdate
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
	WHERE e.EmployeeId = @EmployeeId

	SELECT epbs.EmployeeId
		,epbs.CurrencyId
		,epbs.Salary
		,epbs.WorkingDays
		,epbs.EbankingCurrencyId
		,epbs.EbankingAmount
		,epbs.EbankingAccountId
		,rr.Value AS 'EbankingAccountName'
		,epbs.DirectBanking1CurrencyId
		,epbs.DirectBanking1Amount
		,epbs.DirectBanking1AccountId
		,rr1.Value AS 'DirectBanking1AccountName'
		,epbs.DirectBanking2CurrencyId
		,epbs.DirectBanking2Amount
		,epbs.DirectBanking2AccountId
		,rr2.Value AS 'DirectBanking2AccountName'
		,epbs.CashCurrencyId
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
		LEFT JOIN dbo.RefResource AS rr WITH (NOLOCK) ON eba.PaymentTransactionId = rr.ResourceId
		AND rr.RefName = 'PaymentTransactionEbanking'
		LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON epbs.DirectBanking1AccountId = dba1.Id
		LEFT JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON dba1.PaymentTransactionId = rr1.ResourceId
		AND rr1.RefName = 'PaymentTransactionDirectBanking'
		LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON DirectBanking2AccountId = dba2.Id
		LEFT JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON dba2.PaymentTransactionId = rr2.ResourceId
		AND rr2.RefName = 'PaymentTransactionDirectBanking'

	SELECT epba.EmployeeId
		,epba.CurrencyId
		,epba.DirectBanking1CurrencyId
		,epba.DirectBanking1Amount
		,epba.DirectBanking1AccountId
		,rr1.Value AS 'DirectBanking1AccountName'
		,epba.DirectBanking2CurrencyId
		,epba.DirectBanking2Amount
		,epba.DirectBanking2AccountId
		,rr2.Value AS 'DirectBanking2AccountName'
		,epba.CashCurrencyId
		,epba.CashAmount
		,epba.CashAccountId
		,epba.AutoCalculateOn
		,epba.SubsistenceCurrencyId
		,epba.SubsistenceAmount
		,epba.HomePassageCurrencyId
		,epba.HomePassageAmount
		,epba.HousingCurrencyId
		,epba.HousingAmount
		,epba.MobilePhoneCurrencyId
		,epba.MobilePhoneAmount
		,epba.HPICurrencyId
		,epba.HPIAmount
		,epba.DutyMealCurrencyId
		,epba.DutyMealAmount
		,epba.SecondmentCurrencyId
		,epba.SecondmentAmount
		,epba.OthersCurrencyId
		,epba.OthersAmount
	FROM dbo.EmployeePayrollAllowance AS epba WITH (NOLOCK)
		LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON epba.DirectBanking1AccountId = dba1.Id
		LEFT JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON dba1.PaymentTransactionId = rr1.ResourceId
		AND rr1.RefName = 'PaymentTransactionDirectBanking'
		LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON DirectBanking2AccountId = dba2.Id
		LEFT JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON dba2.PaymentTransactionId = rr2.ResourceId
		AND rr2.RefName = 'PaymentTransactionDirectBanking'
	WHERE epba.EmployeeId = @EmployeeId

	SELECT epd.EmployeeId
		,epd.RoomingChargesCurrencyId
		,epd.RoomingChargesAmount
		,epd.FitnessFirstCurrencyId
		,epd.FitnessFirstAmount
	FROM dbo.EmployeePayrollDeduction AS epd WITH (NOLOCK)
	WHERE epd.EmployeeId = @EmployeeId
END