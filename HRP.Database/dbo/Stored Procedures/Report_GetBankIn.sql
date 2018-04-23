
CREATE PROCEDURE [dbo].[Report_GetBankIn]
	@Month smalldatetime,
	@CompanyId int,
	@PaymentType int,
	@PaymentTransactions nvarchar(max)
AS
/*
Created: 20180406@Hunter.Tran
Task: Get data for Bank-In reports
DB: bodb_HRP
Original:
Revisions:

Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	-- split selected payment transaction
	CREATE TABLE #SelectedPaymentTransactions (BankTypeId INT, PaymentTransactionId INT)
	
	CREATE TABLE #PaymentTransactionValues (Pair NVARCHAR(5))
	INSERT INTO #PaymentTransactionValues (Pair)
	SELECT *
	FROM dbo.SplitString(@PaymentTransactions, ',')

	INSERT INTO #SelectedPaymentTransactions (BankTypeId, PaymentTransactionId)
	SELECT SUBSTRING(Pair, 1, 1), SUBSTRING(Pair, 3,LEN(Pair) -2)
	FROM #PaymentTransactionValues

	DROP TABLE #PaymentTransactionValues

	CREATE TABLE #MinimalMonthly (PayrollId INT, BankCurrencyId INT, BankAmount NVARCHAR(MAX), BankAccountId INT)

	-- get bank in data
	IF @PaymentType = 1 --BasicSalary
	BEGIN

		INSERT INTO #MinimalMonthly (PayrollId, BankCurrencyId, BankAmount, BankAccountId)
		select PayrollId, BankCurrencyId, BankAmount, BankAccountId
		from MonthlySalary
		cross apply
		(
			values
				(EbankingCurrencyId, EbankingAmount, EbankingAccountId),
				(DirectBanking1CurrencyId, DirectBanking1Amount, DirectBanking1AccountId),
				(DirectBanking2CurrencyId, DirectBanking2Amount, DirectBanking2AccountId)
		) c (BankCurrencyId, BankAmount, BankAccountId)
		where BankCurrencyId is not null

		SELECT Employee.Nickname,
		    BankAccount.BeneficiaryName,
		    BankAccount.BankName,
		    BankAccount.AccountNumber,
			RefResource.[Value] AS PaymentTransaction,
		    #MinimalMonthly.BankAmount AS Amount,
		    Currency.CurrencyCode
		FROM MonthlyPayroll WITH(NOLOCK)
		    INNER JOIN #MinimalMonthly WITH(NOLOCK) ON MonthlyPayroll.PayrollId = #MinimalMonthly.PayrollId
		    INNER JOIN BankAccount WITH(NOLOCK) ON BankAccount.Id = #MinimalMonthly.BankAccountId
		    INNER JOIN Currency WITH(NOLOCK) ON #MinimalMonthly.BankCurrencyId = Currency.CurrencyId
		    INNER JOIN Employee WITH(NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
		    INNER JOIN Employment WITH(NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN RefResource WITH (NOLOCK) ON 
			CASE
				WHEN BankAccount.BankType = 1 THEN 'PaymentTransactionEbanking'
				ELSE 'PaymentTransactionDirectBanking'
			END = RefResource.RefName AND RefResource.ResourceId = BankAccount.PaymentTransactionId
		WHERE	YEAR(MonthlyPayroll.Month) = YEAR(@month)
		    AND MONTH(MonthlyPayroll.Month) = MONTH(@month)
		    AND Employment.CompanyId = @CompanyId
		ORDER BY BankAccount.BankName ASC,
				 Employee.Nickname ASC

		-- Cash
		SELECT Employee.Nickname,
		    Employee.FullName as BeneficiaryName,
		    'Cash' as BankName,
		    RefResource.[Value] as AccountNumber,
			RefResource.[Value] AS PaymentTransaction,
		    MonthlySalary.CashAmount as Amount,
		    Currency.CurrencyCode
		FROM MonthlyPayroll WITH(NOLOCK)
		    INNER JOIN MonthlySalary WITH(NOLOCK) ON MonthlyPayroll.PayrollId = MonthlySalary.PayrollId
		    INNER JOIN Currency WITH(NOLOCK) ON MonthlySalary.CashCurrencyId = Currency.CurrencyId
		    INNER JOIN Employee WITH(NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
		    INNER JOIN Employment WITH(NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionCash'
		WHERE	YEAR(MonthlyPayroll.Month) = YEAR(@month)
		    AND MONTH(MonthlyPayroll.Month) = MONTH(@month)
		    AND Employment.CompanyId = @CompanyId
		ORDER BY Employee.Nickname ASC

	END
	ELSE -- Allowances
	BEGIN
		INSERT INTO #MinimalMonthly (PayrollId, BankCurrencyId, BankAmount, BankAccountId)
		select PayrollId, BankCurrencyId, BankAmount, BankAccountId
		from MonthlyAllowance
		cross apply
		(
			values
				(DirectBanking1CurrencyId, DirectBanking1Amount, DirectBanking1AccountId),
				(DirectBanking2CurrencyId, DirectBanking2Amount, DirectBanking2AccountId)
		) c (BankCurrencyId, BankAmount, BankAccountId)
		where BankCurrencyId is not null

		SELECT Employee.Nickname,
		    BankAccount.BeneficiaryName,
		    BankAccount.BankName,
		    BankAccount.AccountNumber,
			RefResource.[Value] AS PaymentTransaction,
		    #MinimalMonthly.BankAmount AS Amount,
		    Currency.CurrencyCode
		FROM MonthlyPayroll WITH(NOLOCK)
		    INNER JOIN #MinimalMonthly WITH(NOLOCK) ON MonthlyPayroll.PayrollId = #MinimalMonthly.PayrollId
		    INNER JOIN BankAccount WITH(NOLOCK) ON BankAccount.Id = #MinimalMonthly.BankAccountId
		    INNER JOIN Currency WITH(NOLOCK) ON #MinimalMonthly.BankCurrencyId = Currency.CurrencyId
		    INNER JOIN Employee WITH(NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
		    INNER JOIN Employment WITH(NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN RefResource WITH (NOLOCK) ON 
			CASE
				WHEN BankAccount.BankType = 1 THEN 'PaymentTransactionEbanking'
				ELSE 'PaymentTransactionDirectBanking'
			END = RefResource.RefName AND RefResource.ResourceId = BankAccount.PaymentTransactionId
		WHERE	YEAR(MonthlyPayroll.Month) = YEAR(@month)
		    AND MONTH(MonthlyPayroll.Month) = MONTH(@month)
		    AND Employment.CompanyId = @CompanyId
		ORDER BY BankAccount.BankName ASC,
				 Employee.Nickname ASC

		-- Cash
		SELECT Employee.Nickname,
		    Employee.FullName as BeneficiaryName,
		    'Cash' as BankName,
		    RefResource.[Value] as AccountNumber,
			RefResource.[Value] AS PaymentTransaction,
		    MonthlyAllowance.CashAmount as Amount,
		    Currency.CurrencyCode
		FROM MonthlyPayroll WITH(NOLOCK)
		    INNER JOIN MonthlyAllowance WITH(NOLOCK) ON MonthlyPayroll.PayrollId = MonthlyAllowance.PayrollId
		    INNER JOIN Currency WITH(NOLOCK) ON MonthlyAllowance.CashCurrencyId = Currency.CurrencyId
		    INNER JOIN Employee WITH(NOLOCK) ON Employee.EmployeeId = MonthlyPayroll.EmployeeId
		    INNER JOIN Employment WITH(NOLOCK) ON Employment.EmployeeId = MonthlyPayroll.EmployeeId
			INNER JOIN RefResource WITH (NOLOCK) ON RefResource.RefName = 'PaymentTransactionCash'
		WHERE	YEAR(MonthlyPayroll.Month) = YEAR(@month)
		    AND MONTH(MonthlyPayroll.Month) = MONTH(@month)
		    AND Employment.CompanyId = @CompanyId
		ORDER BY Employee.Nickname ASC
	END

	-- Company
	SELECT [Company].[Name]
	FROM Company
	WHERE CompanyId = @CompanyId
	
	SELECT RefResource.[Value]
	FROM RefResource 
	INNER JOIN #SelectedPaymentTransactions ON RefResource.ResourceId = #SelectedPaymentTransactions.PaymentTransactionId
	AND
		CASE
			WHEN #SelectedPaymentTransactions.BankTypeId = 1 THEN 'PaymentTransactionEbanking'
			WHEN #SelectedPaymentTransactions.BankTypeId = 2 THEN 'PaymentTransactionDirectBanking'
			ELSE 'PaymentTransactionDirectCash'
		END = RefResource.RefName
	
	DROP TABLE #SelectedPaymentTransactions
	DROP TABLE #MinimalMonthly

END