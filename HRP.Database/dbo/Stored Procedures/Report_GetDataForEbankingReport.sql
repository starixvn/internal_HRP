
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

		SELECT Company.Code + Employment.EmployeeCode AS IDNo
			,Employee.Nickname
			,BankAccount.BeneficiaryName
			,BankAccount.BankName
			,BankAccount.BankBranch
			,BankAccount.BankAddress
			,BankAccount.BankSwiftCode
			,BankAccount.AccountNumber
			,RefResource.[Value] AS PaymentTransaction
			,#MinimalMonthly.BankAmount AS Amount
			,Currency.CurrencyCode
		FROM dbo.MonthlyPayroll WITH (NOLOCK)
			INNER JOIN #MinimalMonthly WITH (NOLOCK) ON MonthlyPayroll.PayrollId = #MinimalMonthly.PayrollId
			INNER JOIN dbo.BankAccount WITH (NOLOCK) ON BankAccount.Id = #MinimalMonthly.BankAccountId
			INNER JOIN dbo.Currency WITH (NOLOCK) ON #MinimalMonthly.BankCurrencyId = Currency.CurrencyId
			INNER JOIN dbo.Employee WITH (NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Employment WITH (NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Company WITH (NOLOCK) ON Employment.CompanyId = Company.CompanyId
			INNER JOIN dbo.RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionEbanking'
			AND RefResource.ResourceId = BankAccount.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions ON #SelectedPaymentTransactions.BankTypeId = 1
			AND #SelectedPaymentTransactions.PaymentTransactionId = RefResource.ResourceId
			AND #SelectedPaymentTransactions.BankTypeId = BankAccount.BankType
		WHERE YEAR(MonthlyPayroll.[Month]) = YEAR(@month)
			AND MONTH(MonthlyPayroll.[Month]) = MONTH(@month)
			AND Employment.CompanyId = @CompanyId
		ORDER BY BankAccount.BankName ASC
			,Employee.Nickname ASC

		-- Cash
		SELECT Company.Code + Employment.EmployeeCode AS IDNo
			,Employee.Nickname
			,Employee.FullName AS BeneficiaryName
			,NULL AS BankName
			,NULL AS AccountNumber
			,RefResource.[Value] AS PaymentTransaction
			,MonthlySalary.CashAmount AS Amount
			,Currency.CurrencyCode
		FROM dbo.MonthlyPayroll WITH (NOLOCK)
			INNER JOIN dbo.MonthlySalary WITH (NOLOCK) ON MonthlyPayroll.PayrollId = MonthlySalary.PayrollId
			INNER JOIN dbo.Currency WITH (NOLOCK) ON MonthlySalary.CashCurrencyId = Currency.CurrencyId
			INNER JOIN dbo.Employee WITH (NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Employment WITH (NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Company WITH (NOLOCK) ON Employment.CompanyId = Company.CompanyId
			INNER JOIN dbo.RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionCash'
			INNER JOIN #SelectedPaymentTransactions ON #SelectedPaymentTransactions.BankTypeId = 3
			AND RefResource.ResourceId = #SelectedPaymentTransactions.PaymentTransactionId
			AND RefResource.ResourceId = MonthlySalary.CashAccountId
		WHERE YEAR(MonthlyPayroll.[Month]) = YEAR(@month)
			AND MONTH(MonthlyPayroll.[Month]) = MONTH(@month)
			AND Employment.CompanyId = @CompanyId
		ORDER BY Employee.Nickname ASC

	-- Company
	SELECT [Name]
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @CompanyId

	-- Get Payment transactions that have data and selected by user
	SELECT ref.[Value]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		INNER JOIN #SelectedPaymentTransactions AS pt WITH (NOLOCK) ON ref.ResourceId = pt.PaymentTransactionId
		AND ref.RefName =
		CASE
			WHEN pt.BankTypeId = 3
			THEN 'PaymentTransactionCash'
			ELSE 'PaymentTransactionEbanking'
		END

	DROP TABLE #SelectedPaymentTransactions

	DROP TABLE #MinimalMonthly
END