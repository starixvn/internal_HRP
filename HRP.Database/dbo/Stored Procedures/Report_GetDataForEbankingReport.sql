
CREATE PROCEDURE [dbo].[Report_GetDataForEbankingReport]
	@UserId INT
	,@Month SMALLDATETIME
	,@CompanyId INT
	,@PaymentTransactions NVARCHAR(MAX) = ''
AS
/*
Created: 20180406@Hunter.Tran
Task: Get data for E-Banking reports
DB: bodb_HRP
Original:
Revisions:

Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EBankingRefName NVARCHAR(50) = 'PaymentTransactionEbanking'
			,@CashRefName NVARCHAR(50) = 'PaymentTransactionCash'

	-- Check Permission on Company
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, 'EbankingReport') = 0 -- don't have read permission
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
				EbankingCurrencyId
				,EbankingAmount
				,EbankingAccountId
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
			INNER JOIN #MinimalMonthly as mm WITH (NOLOCK) ON MonthlyPayroll.PayrollId = mm.PayrollId
			INNER JOIN dbo.BankAccount as ba WITH (NOLOCK) ON ba.Id = mm.BankAccountId
			INNER JOIN dbo.Currency as cur WITH (NOLOCK) ON mm.BankCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee as e WITH (NOLOCK) ON e.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Employment as ep WITH (NOLOCK) ON ep.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Company as com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource as ref WITH (NOLOCK) ON ref.RefName = @EBankingRefName
			AND ref.ResourceId = ba.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions as spt ON spt.BankTypeId = 1
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
			,ms.CashAmount AS Amount
			,cur.CurrencyCode
		FROM dbo.MonthlyPayroll as mp WITH (NOLOCK)
			INNER JOIN dbo.MonthlySalary as ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
			INNER JOIN dbo.Currency as cur WITH (NOLOCK) ON ms.CashCurrencyId = cur.CurrencyId
			INNER JOIN dbo.Employee as e WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Employment as ep WITH (NOLOCK) ON ep.EmployeeId = mp.EmployeeId
			INNER JOIN dbo.Company as com WITH (NOLOCK) ON ep.CompanyId = com.CompanyId
			INNER JOIN dbo.RefResource as ref WITH (NOLOCK) ON ref.RefName = @CashRefName
			INNER JOIN #SelectedPaymentTransactions as spt ON spt.BankTypeId = 3
			AND ref.ResourceId = spt.PaymentTransactionId
			AND ref.ResourceId = ms.CashAccountId
		WHERE YEAR(mp.[Month]) = YEAR(@month)
			AND MONTH(mp.[Month]) = MONTH(@month)
			AND ep.CompanyId = @CompanyId
		ORDER BY e.Nickname ASC

	-- Company
	SELECT [Name]
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @CompanyId

	-- Get Payment transaction's names that were selected by user
	SELECT ref.[Value]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		INNER JOIN #SelectedPaymentTransactions AS pt WITH (NOLOCK) ON ref.ResourceId = pt.PaymentTransactionId
		AND ref.RefName =
		CASE
			WHEN pt.BankTypeId = 3
			THEN @CashRefName
			ELSE @EBankingRefName
		END

	DROP TABLE #SelectedPaymentTransactions

	DROP TABLE #MinimalMonthly
END