
CREATE PROCEDURE [dbo].[Report_GetDataForDirectBankingReport]
	@UserId INT
	,@Month SMALLDATETIME
	,@CompanyId INT
	,@PaymentType INT
	,@PaymentTransactions NVARCHAR(MAX) = ''
AS
/*
Created: 20180406@Hunter.Tran
Task: Get data for Direct Banking reports
DB: bodb_HRP
Original:
Revisions:

Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DirectBankingRefName NVARCHAR(50) = 'PaymentTransactionDirectBanking'
			,@CashRefName NVARCHAR(50) = 'PaymentTransactionCash'

	-- Check Permission on Company
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, 'DirectBankingReport') = 0 -- don't have read permission
	BEGIN
		RETURN
	END

	-- split selected payment transaction
	CREATE TABLE #SelectedPaymentTransactions (
		BankTypeId INT
		,PaymentTransactionId INT
		)

	CREATE TABLE #PaymentTransactionValues (Pair NVARCHAR(5))

	INSERT INTO #PaymentTransactionValues (Pair)
	SELECT val
	FROM dbo.strSplit(@PaymentTransactions, ',')

	INSERT INTO #SelectedPaymentTransactions (
		BankTypeId
		,PaymentTransactionId
		)
	SELECT SUBSTRING(Pair, 1, 1)
		,SUBSTRING(Pair, 3, LEN(Pair) - 2)
	FROM #PaymentTransactionValues
	WHERE LEN(Pair) = 3

	DROP TABLE #PaymentTransactionValues

	CREATE TABLE #MinimalMonthly (
		PayrollId INT
		,BankCurrencyId INT
		,BankAmount NVARCHAR(MAX)
		,BankAccountId INT
		)

	-- get bank in data
	IF @PaymentType = 1 --BasicSalary
	BEGIN
		INSERT INTO #MinimalMonthly (
			PayrollId
			,BankCurrencyId
			,BankAmount
			,BankAccountId
			)
		SELECT PayrollId
			,BankCurrencyId
			,BankAmount
			,BankAccountId
		FROM dbo.MonthlySalary WITH (NOLOCK)
			CROSS APPLY (
			VALUES (
				DirectBanking1CurrencyId
				,DirectBanking1Amount
				,DirectBanking1AccountId
				)
				,(
				DirectBanking2CurrencyId
				,DirectBanking2Amount
				,DirectBanking2AccountId
				)
			) c(BankCurrencyId, BankAmount, BankAccountId)
		WHERE BankCurrencyId IS NOT NULL

		SELECT com.Code + ep.EmployeeCode AS IDNo
			,e.Nickname
			,ba.BeneficiaryName
			,ba.BankName
			,ba.BankBranch
			,ba.BankAddress
			,ba.BankSwiftCode
			,ba.AccountNumber
			,ref.[Value] AS PaymentTransaction
			,mm.BankAmount AS Amount
			,cur.CurrencyCode
		FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
			INNER JOIN #MinimalMonthly AS mm WITH (NOLOCK) ON mp.PayrollId = mm.PayrollId
			INNER JOIN dbo.BankAccount AS ba WITH (NOLOCK) ON ba.Id = mm.BankAccountId
			INNER JOIN dbo.Currency AS cur WITH (NOLOCK) ON mm.BankCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON ep.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Company AS com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ref.RefName = @DirectBankingRefName
			AND ref.ResourceId = ba.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions AS spt ON spt.BankTypeId = 2
			AND spt.PaymentTransactionId = ref.ResourceId
			AND spt.BankTypeId = ba.BankType
		WHERE YEAR(mp.[Month]) = YEAR(@month) 
			AND MONTH(mp.[Month]) = MONTH(@month)
			AND ep.CompanyId = @CompanyId
		ORDER BY ba.BankName ASC
			,e.Nickname ASC

		-- Cash
		SELECT com.Code + ep.EmployeeCode AS IDNo
			,e.Nickname
			,e.FullName AS BeneficiaryName
			,NULL AS BankName
			,NULL AS AccountNumber
			,ref.[Value] AS PaymentTransaction
			,ms.CashAmount AS Amount
			,cur.CurrencyCode
		FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
			INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
			INNER JOIN dbo.Currency AS cur WITH (NOLOCK) ON ms.CashCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON ep.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Company AS com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ref.RefName = @CashRefName
			INNER JOIN #SelectedPaymentTransactions AS spt ON spt.BankTypeId = 3
			AND ref.ResourceId = spt.PaymentTransactionId
			AND ref.ResourceId = ms.CashAccountId
		WHERE YEAR(mp.[Month]) = YEAR(@month)
			AND MONTH(mp.[Month]) = MONTH(@month)
			AND ep.CompanyId = @CompanyId
		ORDER BY e.Nickname ASC
	END
	ELSE -- Allowances
	BEGIN
		INSERT INTO #MinimalMonthly (
			PayrollId
			,BankCurrencyId
			,BankAmount
			,BankAccountId
			)
		SELECT PayrollId
			,BankCurrencyId
			,BankAmount
			,BankAccountId
		FROM dbo.MonthlyAllowance
			CROSS APPLY (
			VALUES (
				DirectBanking1CurrencyId
				,DirectBanking1Amount
				,DirectBanking1AccountId
				)
				,(
				DirectBanking2CurrencyId
				,DirectBanking2Amount
				,DirectBanking2AccountId
				)
			) c(BankCurrencyId, BankAmount, BankAccountId)
		WHERE BankCurrencyId IS NOT NULL

		SELECT com.Code + ep.EmployeeCode AS IDNo
			,e.Nickname
			,ba.BeneficiaryName
			,ba.BankName
			,ba.BankBranch
			,ba.BankAddress
			,ba.BankSwiftCode
			,ba.AccountNumber
			,ref.[Value] AS PaymentTransaction
			,mm.BankAmount AS Amount
			,cur.CurrencyCode
		FROM dbo.MonthlyPayroll WITH (NOLOCK)
			INNER JOIN #MinimalMonthly AS mm WITH (NOLOCK) ON MonthlyPayroll.PayrollId = mm.PayrollId
			INNER JOIN dbo.BankAccount AS ba WITH (NOLOCK) ON ba.Id = mm.BankAccountId
			INNER JOIN dbo.Currency AS cur WITH (NOLOCK) ON mm.BankCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON ep.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Company AS com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ref.RefName = @DirectBankingRefName
			AND ref.ResourceId = ba.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions AS spt ON spt.BankTypeId = 2
			AND spt.PaymentTransactionId = ref.ResourceId
			AND spt.BankTypeId = ba.BankType
		WHERE YEAR(MonthlyPayroll.[Month]) = YEAR(@month)
			AND MONTH(MonthlyPayroll.[Month]) = MONTH(@month)
			AND ep.CompanyId = @CompanyId
		ORDER BY ba.BankName ASC
			,e.Nickname ASC

		-- Cash
		SELECT com.Code + ep.EmployeeCode AS IDNo
			,e.Nickname
			,e.FullName AS BeneficiaryName
			,NULL AS BankName
			,NULL AS AccountNumber
			,ref.[Value] AS PaymentTransaction
			,ma.CashAmount AS Amount
			,cur.CurrencyCode
		FROM dbo.MonthlyPayroll AS mm WITH (NOLOCK)
			INNER JOIN dbo.MonthlyAllowance as ma WITH (NOLOCK) ON mm.PayrollId = ma.PayrollId
			INNER JOIN dbo.Currency AS cur WITH (NOLOCK) ON ma.CashCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = mm.EmployeeId
			INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON ep.EmployeeId = mm.EmployeeId
			INNER JOIN dbo.Company AS com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource AS ref WITH (NOLOCK) ON ref.RefName = @CashRefName
			INNER JOIN #SelectedPaymentTransactions AS spt ON spt.BankTypeId = 3
			AND ref.ResourceId = spt.PaymentTransactionId
			AND ref.ResourceId = ma.CashAccountId
		WHERE YEAR(mm.[Month]) = YEAR(@month)
			AND MONTH(mm.[Month]) = MONTH(@month)
			AND ep.CompanyId = @CompanyId
		ORDER BY e.Nickname ASC
	END

	-- Company
	SELECT [Name]
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @CompanyId

	-- Get Payment transaction's names that were selected by user
	SELECT ref.[Value]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		INNER JOIN #SelectedPaymentTransactions AS spt WITH (NOLOCK) ON ref.ResourceId = spt.PaymentTransactionId
		AND ref.RefName =
		CASE
			WHEN spt.BankTypeId = 3
			THEN @CashRefName
			ELSE @DirectBankingRefName
		END

	DROP TABLE #SelectedPaymentTransactions

	DROP TABLE #MinimalMonthly
END