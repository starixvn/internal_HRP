
/*
 Created: 20180112@EltonTon
 Task : update payroll - basic salary
 DB: bodb_HRP
 Original:
 */
CREATE PROCEDURE dbo.Employee_UpdatePayrollBasicSalary
	@EmployeeId INT
	,@Salary VARCHAR(MAX)
	,@CurrencyCode VARCHAR(3)
	,@WorkingDays TINYINT
	,@EbankingCurrencyCode VARCHAR(3)
	,@EbankingAmount VARCHAR(MAX)
	,@EbankingAccountId INT
	,@DirectBanking1CurrencyCode VARCHAR(3)
	,@DirectBanking1Amount VARCHAR(MAX)
	,@DirectBanking1AccountId INT
	,@DirectBanking2CurrencyCode VARCHAR(3)
	,@DirectBanking2Amount VARCHAR(MAX)
	,@DirectBanking2AccountId INT
	,@CashCurrencyCode VARCHAR(3)
	,@CashAmount VARCHAR(MAX)
	,@CashAccountId INT
	,@AutoCalculateOn VARCHAR(30)
	,@EffectiveDate DATE
	,@ReasonId TINYINT
	,@UpdatedBy INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logDataOld NVARCHAR(MAX)
		,@logData NVARCHAR(MAX)
		,@xml XML

	SET @startRun = GETDATE()

	IF NOT EXISTS (SELECT 1 FROM dbo.EmployeePayrollBasicSalary WHERE EmployeeId = @EmployeeId)
	BEGIN
		INSERT INTO dbo.EmployeePayrollBasicSalary
		(
			[EmployeeId]
           ,[Salary]
           ,[CurrencyCode]
           ,[WorkingDays]
           ,[EbankingCurrencyCode]
           ,[EbankingAmount]
           ,[EbankingAccountId]
           ,[DirectBanking1CurrencyCode]
           ,[DirectBanking1Amount]
           ,[DirectBanking1AccountId]
           ,[DirectBanking2CurrencyCode]
           ,[DirectBanking2Amount]
           ,[DirectBanking2AccountId]
           ,[CashCurrencyCode]
           ,[CashAmount]
           ,[CashAccountId]
           ,[AutoCalculateOn]
		)
		VALUES
		(
			@EmployeeId
			,@Salary
			,@CurrencyCode
			,@WorkingDays
			,@EbankingCurrencyCode
			,@EbankingAmount
			,@EbankingAccountId
			,@DirectBanking1CurrencyCode
			,@DirectBanking1Amount
			,@DirectBanking1AccountId
			,@DirectBanking2CurrencyCode
			,@DirectBanking2Amount
			,@DirectBanking2AccountId
			,@CashCurrencyCode
			,@CashAmount
			,@CashAccountId
			,@AutoCalculateOn
		)

		INSERT INTO [dbo].[EmployeePayrollBasicSalaryHistory]
        (
			[EmployeeId]
			,[Salary]
            ,[CurrencyCode]
            ,[EffectiveDate]
            ,[ReasonId]
		)
		VALUES
        (
			@EmployeeId
			,@Salary
			,@CurrencyCode
			,@EffectiveDate
			,@ReasonId
		)

		SET @logDataOld = NULL
	END
	ELSE
	BEGIN
		DECLARE @OldSalary VARCHAR(MAX)
		SELECT @OldSalary = Salary
		FROM dbo.EmployeePayrollBasicSalary
		WHERE EmployeeId = @EmployeeId

		SET @xml = (
			SELECT Salary AS '@sa'
				  ,CurrencyCode AS '@cc'
				  ,WorkingDays AS '@wd'
				  ,EbankingCurrencyCode AS '@ecc'
				  ,EbankingAmount AS '@ea'
				  ,EbankingAccountId AS '@eai'
				  ,DirectBanking1CurrencyCode AS '@d1cc'
				  ,DirectBanking1Amount AS '@d1a'
				  ,DirectBanking1AccountId AS '@d1ai'
				  ,DirectBanking2CurrencyCode AS '@d2cc'
				  ,DirectBanking2Amount AS '@d2a'
				  ,DirectBanking2AccountId AS '@d2ai'
				  ,CashCurrencyCode AS '@ccc'
				  ,CashAmount AS '@ca'
				  ,CashAccountId AS '@cai'
				  ,AutoCalculateOn AS '@aco'
			FROM dbo.EmployeePayrollBasicSalary WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			FOR XML PATH('r'), TYPE
			)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		UPDATE [dbo].[EmployeePayrollBasicSalary]
		SET Salary = @Salary
			,CurrencyCode = @CurrencyCode
			,WorkingDays = @WorkingDays
			,EbankingCurrencyCode = @EbankingCurrencyCode
			,EbankingAmount = @EbankingAmount
			,EbankingAccountId  = @EbankingAccountId
			,DirectBanking1CurrencyCode = @DirectBanking1CurrencyCode
			,DirectBanking1Amount = @DirectBanking1Amount
			,DirectBanking1AccountId = @DirectBanking1AccountId
			,DirectBanking2CurrencyCode = @DirectBanking2CurrencyCode
			,DirectBanking2Amount = @DirectBanking2Amount
			,DirectBanking2AccountId = @DirectBanking2AccountId
			,CashCurrencyCode = @CashCurrencyCode
			,CashAmount = @CashAmount
			,CashAccountId = @CashAccountId
			,AutoCalculateOn = @AutoCalculateOn
		WHERE EmployeeId = @EmployeeId

		IF @OldSalary <> @Salary
		BEGIN
			INSERT INTO [dbo].[EmployeePayrollBasicSalaryHistory]
			(
				[EmployeeId]
				,[Salary]
				,[CurrencyCode]
				,[EffectiveDate]
				,[ReasonId]
			)
			VALUES
			(
				@EmployeeId
				,@Salary
				,@CurrencyCode
				,@EffectiveDate
				,@ReasonId
			)
		END

	END

	SET @xml = (
			SELECT Salary AS '@sa'
				  ,CurrencyCode AS '@cc'
				  ,WorkingDays AS '@wd'
				  ,EbankingCurrencyCode AS '@ecc'
				  ,EbankingAmount AS '@ea'
				  ,EbankingAccountId AS '@eai'
				  ,DirectBanking1CurrencyCode AS '@d1cc'
				  ,DirectBanking1Amount AS '@d1a'
				  ,DirectBanking1AccountId AS '@d1ai'
				  ,DirectBanking2CurrencyCode AS '@d2cc'
				  ,DirectBanking2Amount AS '@d2a'
				  ,DirectBanking2AccountId AS '@d2ai'
				  ,CashCurrencyCode AS '@ccc'
				  ,CashAmount AS '@ca'
				  ,CashAccountId AS '@cai'
				  ,AutoCalculateOn AS '@aco'
			FROM dbo.EmployeePayrollBasicSalary WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			FOR XML PATH('r'), TYPE
			)

	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Employee_UpdatePayrollBasicSalary'
		,@ObjectId = @EmployeeId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END