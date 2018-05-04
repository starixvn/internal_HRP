
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
			INNER JOIN dbo.RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionDirectBanking'
			AND RefResource.ResourceId = BankAccount.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions ON #SelectedPaymentTransactions.BankTypeId = 2
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
			INNER JOIN dbo.RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionDirectBanking'
			AND RefResource.ResourceId = BankAccount.PaymentTransactionId
			INNER JOIN #SelectedPaymentTransactions ON #SelectedPaymentTransactions.BankTypeId = 2
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
			,MonthlyAllowance.CashAmount AS Amount
			,Currency.CurrencyCode
		FROM dbo.MonthlyPayroll WITH (NOLOCK)
			INNER JOIN dbo.MonthlyAllowance WITH (NOLOCK) ON MonthlyPayroll.PayrollId = MonthlyAllowance.PayrollId
			INNER JOIN dbo.Currency WITH (NOLOCK) ON MonthlyAllowance.CashCurrencyId = Currency.CurrencyId
			INNER JOIN dbo.Employee WITH (NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Employment WITH (NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN dbo.Company WITH (NOLOCK) ON Employment.CompanyId = Company.CompanyId
			INNER JOIN dbo.RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionCash'
			INNER JOIN #SelectedPaymentTransactions ON #SelectedPaymentTransactions.BankTypeId = 3
			AND RefResource.ResourceId = #SelectedPaymentTransactions.PaymentTransactionId
			AND RefResource.ResourceId = MonthlyAllowance.CashAccountId
		WHERE YEAR(MonthlyPayroll.[Month]) = YEAR(@month)
			AND MONTH(MonthlyPayroll.[Month]) = MONTH(@month)
			AND Employment.CompanyId = @CompanyId
		ORDER BY Employee.Nickname ASC
	END

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
			THEN 'PaymentTransactionCash'
			ELSE 'PaymentTransactionDirectBanking'
		END

	DROP TABLE #SelectedPaymentTransactions

	DROP TABLE #MinimalMonthly
END