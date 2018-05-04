
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetSalary] @UserId INT
	,@PayrollId INT
AS
/*
Created: 20180307@Tim.Trinh
Task: get data to update salary
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
Update:
20180418@Elton: remove DirectbankingBankTypeId
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'
		,@companyId SMALLINT
		,@policy TINYINT
		,@month DATE
		,@contraCoBankAccountType TINYINT = 3

	SELECT @companyId = em.CompanyId
		,@month = mp.[Month]
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON mp.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
	WHERE mp.PayrollId = @PayrollId

	-- Check permission
	SELECT @policy = dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @companyId, @functionKey)

	IF @policy = 0
	BEGIN
		RETURN
	END

	SELECT mbs.PayrollId
		,mbs.CurrencyId
		,cur.CurrencyCode
		,mbs.Salary
		,mbs.WorkingDays
		,mbs.EbankingCurrencyId
		,ecur.CurrencyCode AS EBankingCurrencyCode
		,mbs.EbankingAmount
		,mbs.DirectBanking1CurrencyId
		,db1cur.CurrencyCode AS DirectBanking1CurrencyCode
		,mbs.DirectBanking1Amount
		,mbs.DirectBanking2CurrencyId
		,db2cur.CurrencyCode AS DirectBanking2CurrencyCode
		,mbs.DirectBanking2Amount
		,mbs.CashCurrencyId
		,cashcur.CurrencyCode AS CashCurrencyCode
		,mbs.CashAmount
		,mbs.AfterAdjustmentAmount
		,mbs.AfterAdjustmentReason
		,mbs.AutoCalculateOn
	FROM dbo.MonthlyBasicSalary AS mbs WITH (NOLOCK)
		LEFT JOIN dbo.Currency AS cur WITH (NOLOCK) ON mbs.CurrencyId = cur.CurrencyId
		LEFT JOIN dbo.Currency AS ecur WITH (NOLOCK) ON mbs.EbankingCurrencyId = ecur.CurrencyId
		LEFT JOIN dbo.Currency AS db1cur WITH (NOLOCK) ON mbs.DirectBanking1CurrencyId = db1cur.CurrencyId
		LEFT JOIN dbo.Currency AS db2cur WITH (NOLOCK) ON mbs.DirectBanking2CurrencyId = db2cur.CurrencyId
		LEFT JOIN dbo.Currency AS cashcur WITH (NOLOCK) ON mbs.CashCurrencyId = cashcur.CurrencyId
	WHERE mbs.PayrollId = @PayrollId

	SELECT PayrollId
		,ms.CurrencyId
		,c.CurrencyCode
		,Salary
		,EbankingCurrencyId
		,EbankingAmount
		,EbankingAccountId
		,refeb.Value AS EbankingAccountName
		,DirectBanking1CurrencyId
		,DirectBanking1Amount
		,DirectBanking1AccountId
		,refdb1.Value AS DirectBanking1AccountName
		,DirectBanking2CurrencyId
		,DirectBanking2Amount
		,DirectBanking2AccountId
		,refdb2.Value AS DirectBanking2AccountName
		,CashCurrencyId
		,CashAmount
		,CashAccountId
		,AdditionAmount
		,DeductionAmount
		,AutoCalculateOn
		,[Status]
	FROM dbo.MonthlySalary AS ms WITH (NOLOCK)
		LEFT JOIN dbo.BankAccount AS eba WITH (NOLOCK) ON ms.EbankingAccountId = eba.Id
		LEFT JOIN dbo.RefResource AS refeb WITH (NOLOCK) ON eba.PaymentTransactionId = refeb.ResourceId
		AND refeb.RefName = 'PaymentTransactionEbanking'
		LEFT JOIN dbo.BankAccount AS badb1 WITH (NOLOCK) ON ms.DirectBanking1AccountId = badb1.Id
		LEFT JOIN dbo.RefResource AS refdb1 WITH (NOLOCK) ON badb1.PaymentTransactionId = refdb1.ResourceId
		AND refdb1.RefName = 'PaymentTransactionDirectBanking'
		LEFT JOIN dbo.BankAccount AS badb2 WITH (NOLOCK) ON ms.DirectBanking2AccountId = badb2.Id
		LEFT JOIN dbo.RefResource AS refdb2 WITH (NOLOCK) ON badb2.PaymentTransactionId = refdb2.ResourceId
		AND refdb2.RefName = 'PaymentTransactionDirectBanking'
		LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON ms.CurrencyId = c.CurrencyId
	WHERE ms.PayrollId = @PayrollId

	SELECT msa.PayrollId
		,ref.ResourceId AS [Type]
		,msa.UnitAmount
		,msa.NoOfDays
		,msa.NetSalary
		,msa.[Description]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		LEFT JOIN dbo.MonthlySalaryAddition AS msa WITH (NOLOCK) ON ref.ResourceId = msa.[Type]
		AND msa.PayrollId = @PayrollId
	WHERE ref.RefName = 'MonthlySalaryAdditionType'
	ORDER BY ref.ResourceId

	SELECT msd.PayrollId
		,ref.ResourceId AS [Type]
		,msd.UnitAmount
		,msd.NoOfDays
		,msd.NetSalary
		,msd.[Description]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		LEFT JOIN dbo.MonthlySalaryDeduction AS msd WITH (NOLOCK) ON ref.ResourceId = msd.[Type]
		AND msd.PayrollId = @PayrollId
	WHERE ref.RefName = 'MonthlySalaryDeductionType'
	ORDER BY ref.ResourceId
END