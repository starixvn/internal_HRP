

CREATE PROCEDURE [dbo].[Employee_UpdatePayroll] @EmployeeId INT
	,@Salary VARCHAR(MAX) = NULL
	,@IsSalaryChanged BIT
	,@CurrencyCode VARCHAR(3) = NULL
	,@WorkingDays TINYINT
	,@EbankingCurrencyCode VARCHAR(3) = NULL
	,@EbankingAmount VARCHAR(MAX) = NULL
	,@EbankingAccountId INT = NULL
	,@DirectBanking1CurrencyCode VARCHAR(3) = NULL
	,@DirectBanking1Amount VARCHAR(MAX) = NULL
	,@DirectBanking1AccountId INT = NULL
	,@DirectBanking1BankTypeId INT = NULL
	,@DirectBanking2CurrencyCode VARCHAR(3) = NULL
	,@DirectBanking2Amount VARCHAR(MAX) = NULL
	,@DirectBanking2AccountId INT = NULL
	,@DirectBanking2BankTypeId INT = NULL
	,@CashCurrencyCode VARCHAR(3) = NULL
	,@CashAmount VARCHAR(MAX) = NULL
	,@CashAccountId INT = NULL
	,@AutoCalculateOn VARCHAR(30) = NULL
	,@EffectiveDate DATE = NULL
	,@ReasonId INT = NULL
	,@AllowanceCurrencyCode VARCHAR(3) = NULL
	,@AllowanceDirectBanking1CurrencyCode VARCHAR(3) = NULL
	,@AllowanceDirectBanking1Amount VARCHAR(MAX) = NULL
	,@AllowanceDirectBanking1AccountId INT = NULL
	,@AllowanceDirectBanking1BankTypeId INT = NULL
	,@AllowanceDirectBanking2CurrencyCode VARCHAR(3) = NULL
	,@AllowanceDirectBanking2Amount VARCHAR(MAX) = NULL
	,@AllowanceDirectBanking2AccountId INT = NULL
	,@AllowanceDirectBanking2BankTypeId INT = NULL
	,@AllowanceCashCurrencyCode VARCHAR(3) = NULL
	,@AllowanceCashAmount VARCHAR(MAX) = NULL
	,@AllowanceCashAccountId INT = NULL
	,@AllowanceAutoCalculateOn VARCHAR(30) = NULL
	,@SubsistenceCurrencyCode VARCHAR(3) = NULL
	,@SubsistenceAmount VARCHAR(MAX) = NULL
	,@HomePassageCurrencyCode VARCHAR(3) = NULL
	,@HomePassageAmount VARCHAR(MAX) = NULL
	,@HousingCurrencyCode VARCHAR(3) = NULL
	,@HousingAmount VARCHAR(MAX) = NULL
	,@MobilePhoneCurrencyCode VARCHAR(3) = NULL
	,@MobilePhoneAmount VARCHAR(MAX) = NULL
	,@HPICurrencyCode VARCHAR(3) = NULL
	,@HPIAmount VARCHAR(MAX) = NULL
	,@DutyMealCurrencyCode VARCHAR(3) = NULL
	,@DutyMealAmount VARCHAR(MAX) = NULL
	,@SecondmentCurrencyCode VARCHAR(3) = NULL
	,@SecondmentAmount VARCHAR(MAX) = NULL
	,@OthersCurrencyCode VARCHAR(3) = NULL
	,@OthersAmount VARCHAR(MAX) = NULL
	,@RoomingChargesCurrencyCode VARCHAR(3) = NULL
	,@RoomingChargesAmount VARCHAR(MAX) = NULL
	,@FitnessFirstCurrencyCode VARCHAR(3) = NULL
	,@FitnessFirstAmount VARCHAR(MAX) = NULL
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180119@EltonTon
Task : Update Employee Payroll
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
 */
BEGIN
	SET NOCOUNT ON

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @startRun = GETDATE()

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.UserFunction AS uf WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON e.CompanyId = uf.CompanyId
				AND e.EmployeeId = @EmployeeId
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
				AND f.IsActive = 1
			WHERE uf.UserId = @UpdatedBy
				AND uf.FunctionId = 7
				AND uf.IsHost = 1
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission to update this employee payroll'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeePayrollBasicSalary WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			)
	BEGIN
		SET @xml = (
				SELECT epbs.EmployeeId AS '@ei'
					,epbs.CurrencyCode AS '@epbscc'
					,epbs.Salary AS '@epbss'
					,epbs.WorkingDays AS '@epbswd'
					,epbs.EbankingCurrencyCode AS '@epbsecc'
					,epbs.EbankingAmount AS '@epbsea'
					,epbs.EbankingAccountId AS '@epbseai'
					,epbs.DirectBanking1CurrencyCode AS '@epbsdb1cc'
					,epbs.DirectBanking1Amount AS '@epbsdb1a'
					,epbs.DirectBanking1AccountId AS '@epbsdb1ai'
					,epbs.DirectBanking1BankTypeId AS '@epbsdb1bti'
					,epbs.DirectBanking2CurrencyCode AS '@epbsdb2cc'
					,epbs.DirectBanking2Amount AS '@epbsdb2a'
					,epbs.DirectBanking2AccountId AS '@epbsdb2ai'
					,epbs.DirectBanking2BankTypeId AS '@epbsdb2bti'
					,epbs.CashCurrencyCode AS '@epbsccc'
					,epbs.CashAmount AS '@epbsca'
					,epbs.CashAccountId AS '@epbscai'
					,epbs.AutoCalculateOn AS '@epbsaco'
					,epbsh.EffectiveDate AS '@epbsed'
					,epbsh.ReasonId AS '@epbsri'
					,epa.CurrencyCode AS '@epacc'
					,epa.DirectBanking1CurrencyCode AS '@epadb1cc'
					,epa.DirectBanking1Amount AS '@epadb1a'
					,epa.DirectBanking1AccountId AS '@epadb1ai'
					,epa.DirectBanking1BankTypeId AS '@epadb1bti'
					,epa.DirectBanking2CurrencyCode AS '@epadb2cc'
					,epa.DirectBanking2Amount AS '@epadb2a'
					,epa.DirectBanking2AccountId AS '@epadb2ai'
					,epa.DirectBanking2BankTypeId AS '@epadb2bti'
					,epa.CashCurrencyCode AS '@epaccc'
					,epa.CashAmount AS '@epaca'
					,epa.CashAccountId AS '@epacai'
					,epa.AutoCalculateOn AS '@epaaco'
					,epa.SubsistenceCurrencyCode AS '@epasscc'
					,epa.SubsistenceAmount AS '@epassa'
					,epa.HomePassageCurrencyCode AS '@epahpcc'
					,epa.HomePassageAmount AS '@epahpa'
					,epa.HousingCurrencyCode AS '@epahcc'
					,epa.HousingAmount AS '@epaha'
					,epa.MobilePhoneCurrencyCode AS '@epampcc'
					,epa.MobilePhoneAmount AS '@epampa'
					,epa.HPICurrencyCode AS '@epahpicc'
					,epa.HPIAmount AS '@epahpia'
					,epa.DutyMealCurrencyCode AS '@epadmcc'
					,epa.DutyMealAmount AS '@epadma'
					,epa.SecondmentCurrencyCode AS '@epascc'
					,epa.SecondmentAmount AS '@epasa'
					,epa.OthersCurrencyCode AS '@epaocc'
					,epa.OthersAmount AS '@epaoa'
					,epd.RoomingChargesCurrencyCode AS '@epdrccc'
					,epd.RoomingChargesAmount AS '@epdrca'
					,epd.FitnessFirstCurrencyCode AS '@epdffcc'
					,epd.FitnessFirstAmount AS '@epdffa'
				FROM dbo.EmployeePayrollBasicSalary AS epbs WITH (NOLOCK)
					INNER JOIN (
					SELECT TOP 1 EmployeeId
						,EffectiveDate
						,ReasonId
					FROM dbo.EmployeePayrollBasicSalaryHistory WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
					ORDER BY HistoryId DESC
					) AS epbsh ON epbs.EmployeeId = epbsh.EmployeeId
					INNER JOIN dbo.EmployeePayrollAllowance AS epa WITH (NOLOCK) ON epbs.EmployeeId = epa.EmployeeId
					INNER JOIN dbo.EmployeePayrollDeduction AS epd WITH (NOLOCK) ON epbs.EmployeeId = epd.EmployeeId
				WHERE epbs.EmployeeId = @EmployeeId
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		IF @IsSalaryChanged = 1
		BEGIN
			INSERT INTO [dbo].[EmployeePayrollBasicSalaryHistory] (
				[EmployeeId]
				,[Salary]
				,[CurrencyCode]
				,[EffectiveDate]
				,[ReasonId]
				)
			VALUES (
				@EmployeeId
				,@Salary
				,@CurrencyCode
				,@EffectiveDate
				,@ReasonId
				)
		END
		ELSE
		BEGIN
			UPDATE dbo.EmployeePayrollBasicSalaryHistory WITH (ROWLOCK, UPDLOCK)
			SET ReasonId = @ReasonId
				,EffectiveDate = @EffectiveDate
				,Salary = @Salary
				,CurrencyCode = @CurrencyCode
			WHERE EmployeeId = @EmployeeId
				AND HistoryId = (
					SELECT TOP 1 HistoryId
					FROM dbo.EmployeePayrollBasicSalaryHistory AS epbsh WITH (NOLOCK)
					WHERE epbsh.EmployeeId = @EmployeeId
					ORDER BY epbsh.HistoryId DESC
					)
		END

		UPDATE [dbo].[EmployeePayrollBasicSalary] WITH (ROWLOCK, UPDLOCK)
		SET [Salary] = @Salary
			,[CurrencyCode] = @CurrencyCode
			,[WorkingDays] = @WorkingDays
			,[EbankingCurrencyCode] = @EbankingCurrencyCode
			,[EbankingAmount] = @EbankingAmount
			,[EbankingAccountId] = @EbankingAccountId
			,[DirectBanking1CurrencyCode] = @DirectBanking1CurrencyCode
			,[DirectBanking1Amount] = @DirectBanking1Amount
			,[DirectBanking1AccountId] = @DirectBanking1AccountId
			,[DirectBanking2CurrencyCode] = @DirectBanking2CurrencyCode
			,[DirectBanking2Amount] = @DirectBanking2Amount
			,[DirectBanking2AccountId] = @DirectBanking2AccountId
			,[CashCurrencyCode] = @CashCurrencyCode
			,[CashAmount] = @CashAmount
			,[CashAccountId] = @CashAccountId
			,[AutoCalculateOn] = @AutoCalculateOn
			,[DirectBanking1BankTypeId] = @DirectBanking1BankTypeId
			,[DirectBanking2BankTypeId] = @DirectBanking2BankTypeId
		WHERE EmployeeId = @EmployeeId

		UPDATE [dbo].[EmployeePayrollAllowance] WITH (ROWLOCK, UPDLOCK)
		SET [CurrencyCode] = @AllowanceCurrencyCode
			,[DirectBanking1CurrencyCode] = @AllowanceDirectBanking1CurrencyCode
			,[DirectBanking1Amount] = @AllowanceDirectBanking1Amount
			,[DirectBanking1AccountId] = @AllowanceDirectBanking1AccountId
			,[DirectBanking2CurrencyCode] = @AllowanceDirectBanking2CurrencyCode
			,[DirectBanking2Amount] = @AllowanceDirectBanking2Amount
			,[DirectBanking2AccountId] = @AllowanceDirectBanking2AccountId
			,[CashCurrencyCode] = @CashCurrencyCode
			,[CashAmount] = @CashAmount
			,[CashAccountId] = @CashAccountId
			,[AutoCalculateOn] = @AllowanceAutoCalculateOn
			,[SubsistenceCurrencyCode] = @SubsistenceCurrencyCode
			,[SubsistenceAmount] = @SubsistenceAmount
			,[HomePassageCurrencyCode] = @HomePassageCurrencyCode
			,[HomePassageAmount] = @HomePassageAmount
			,[HousingCurrencyCode] = @HousingCurrencyCode
			,[HousingAmount] = @HousingAmount
			,[MobilePhoneCurrencyCode] = @MobilePhoneCurrencyCode
			,[MobilePhoneAmount] = @MobilePhoneAmount
			,[HPICurrencyCode] = @HPICurrencyCode
			,[HPIAmount] = @HPIAmount
			,[DutyMealCurrencyCode] = @DutyMealCurrencyCode
			,[DutyMealAmount] = @DutyMealAmount
			,[SecondmentCurrencyCode] = @SecondmentCurrencyCode
			,[SecondmentAmount] = @SecondmentAmount
			,[OthersCurrencyCode] = @OthersCurrencyCode
			,[OthersAmount] = @OthersAmount
			,[DirectBanking1BankTypeId] = @AllowanceDirectBanking1BankTypeId
			,[DirectBanking2BankTypeId] = @AllowanceDirectBanking2BankTypeId
		WHERE EmployeeId = @EmployeeId

		UPDATE [dbo].[EmployeePayrollDeduction] WITH (ROWLOCK, UPDLOCK)
		SET [RoomingChargesCurrencyCode] = @RoomingChargesCurrencyCode
			,[RoomingChargesAmount] = @RoomingChargesAmount
			,[FitnessFirstCurrencyCode] = @FitnessFirstCurrencyCode
			,[FitnessFirstAmount] = @FitnessFirstAmount
		WHERE EmployeeId = @EmployeeId
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[EmployeePayrollBasicSalary] (
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
			,[DirectBanking1BankTypeId]
			,[DirectBanking2BankTypeId]
			)
		VALUES (
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
			,@DirectBanking1BankTypeId
			,@DirectBanking2BankTypeId
			)

		INSERT INTO [dbo].[EmployeePayrollBasicSalaryHistory] (
			[EmployeeId]
			,[Salary]
			,[CurrencyCode]
			,[EffectiveDate]
			,[ReasonId]
			)
		VALUES (
			@EmployeeId
			,@Salary
			,@CurrencyCode
			,@EffectiveDate
			,@ReasonId
			)

		INSERT INTO [dbo].[EmployeePayrollAllowance] (
			[EmployeeId]
			,[CurrencyCode]
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
			,[SubsistenceCurrencyCode]
			,[SubsistenceAmount]
			,[HomePassageCurrencyCode]
			,[HomePassageAmount]
			,[HousingCurrencyCode]
			,[HousingAmount]
			,[MobilePhoneCurrencyCode]
			,[MobilePhoneAmount]
			,[HPICurrencyCode]
			,[HPIAmount]
			,[DutyMealCurrencyCode]
			,[DutyMealAmount]
			,[SecondmentCurrencyCode]
			,[SecondmentAmount]
			,[OthersCurrencyCode]
			,[OthersAmount]
			,[DirectBanking1BankTypeId]
			,[DirectBanking2BankTypeId]
			)
		VALUES (
			@EmployeeId
			,@AllowanceCurrencyCode
			,@AllowanceDirectBanking1CurrencyCode
			,@AllowanceDirectBanking1Amount
			,@AllowanceDirectBanking1AccountId
			,@AllowanceDirectBanking2CurrencyCode
			,@AllowanceDirectBanking2Amount
			,@AllowanceDirectBanking2AccountId
			,@AllowanceCashCurrencyCode
			,@AllowanceCashAmount
			,@AllowanceCashAccountId
			,@AllowanceAutoCalculateOn
			,@SubsistenceCurrencyCode
			,@SubsistenceAmount
			,@HomePassageCurrencyCode
			,@HomePassageAmount
			,@HousingCurrencyCode
			,@HousingAmount
			,@MobilePhoneCurrencyCode
			,@MobilePhoneAmount
			,@HPICurrencyCode
			,@HPIAmount
			,@DutyMealCurrencyCode
			,@DutyMealAmount
			,@SecondmentCurrencyCode
			,@SecondmentAmount
			,@OthersCurrencyCode
			,@OthersAmount
			,@AllowanceDirectBanking1BankTypeId
			,@AllowanceDirectBanking2BankTypeId
			)

		INSERT INTO [dbo].[EmployeePayrollDeduction] (
			[EmployeeId]
			,[RoomingChargesCurrencyCode]
			,[RoomingChargesAmount]
			,[FitnessFirstCurrencyCode]
			,[FitnessFirstAmount]
			)
		VALUES (
			@EmployeeId
			,@RoomingChargesCurrencyCode
			,@RoomingChargesAmount
			,@FitnessFirstCurrencyCode
			,@FitnessFirstAmount
			)
	END

	SET @xml = (
			SELECT epbs.EmployeeId AS '@ei'
				,epbs.CurrencyCode AS '@epbscc'
				,epbs.Salary AS '@epbss'
				,epbs.WorkingDays AS '@epbswd'
				,epbs.EbankingCurrencyCode AS '@epbsecc'
				,epbs.EbankingAmount AS '@epbsea'
				,epbs.EbankingAccountId AS '@epbseai'
				,epbs.DirectBanking1CurrencyCode AS '@epbsdb1cc'
				,epbs.DirectBanking1Amount AS '@epbsdb1a'
				,epbs.DirectBanking1AccountId AS '@epbsdb1ai'
				,epbs.DirectBanking1BankTypeId AS '@epbsdb1bti'
				,epbs.DirectBanking2CurrencyCode AS '@epbsdb2cc'
				,epbs.DirectBanking2Amount AS '@epbsdb2a'
				,epbs.DirectBanking2AccountId AS '@epbsdb2ai'
				,epbs.DirectBanking2BankTypeId AS '@epbsdb2bti'
				,epbs.CashCurrencyCode AS '@epbsccc'
				,epbs.CashAmount AS '@epbsca'
				,epbs.CashAccountId AS '@epbscai'
				,epbs.AutoCalculateOn AS '@epbsaco'
				,epbsh.EffectiveDate AS '@epbsed'
				,epbsh.ReasonId AS '@epbsri'
				,epa.CurrencyCode AS '@epacc'
				,epa.DirectBanking1CurrencyCode AS '@epadb1cc'
				,epa.DirectBanking1Amount AS '@epadb1a'
				,epa.DirectBanking1AccountId AS '@epadb1ai'
				,epa.DirectBanking1BankTypeId AS '@epadb1bti'
				,epa.DirectBanking2CurrencyCode AS '@epadb2cc'
				,epa.DirectBanking2Amount AS '@epadb2a'
				,epa.DirectBanking2AccountId AS '@epadb2ai'
				,epa.DirectBanking2BankTypeId AS '@epadb2bti'
				,epa.CashCurrencyCode AS '@epaccc'
				,epa.CashAmount AS '@epaca'
				,epa.CashAccountId AS '@epacai'
				,epa.AutoCalculateOn AS '@epaaco'
				,epa.SubsistenceCurrencyCode AS '@epasscc'
				,epa.SubsistenceAmount AS '@epassa'
				,epa.HomePassageCurrencyCode AS '@epahpcc'
				,epa.HomePassageAmount AS '@epahpa'
				,epa.HousingCurrencyCode AS '@epahcc'
				,epa.HousingAmount AS '@epaha'
				,epa.MobilePhoneCurrencyCode AS '@epampcc'
				,epa.MobilePhoneAmount AS '@epampa'
				,epa.HPICurrencyCode AS '@epahpicc'
				,epa.HPIAmount AS '@epahpia'
				,epa.DutyMealCurrencyCode AS '@epadmcc'
				,epa.DutyMealAmount AS '@epadma'
				,epa.SecondmentCurrencyCode AS '@epascc'
				,epa.SecondmentAmount AS '@epasa'
				,epa.OthersCurrencyCode AS '@epaocc'
				,epa.OthersAmount AS '@epaoa'
				,epd.RoomingChargesCurrencyCode AS '@epdrccc'
				,epd.RoomingChargesAmount AS '@epdrca'
				,epd.FitnessFirstCurrencyCode AS '@epdffcc'
				,epd.FitnessFirstAmount AS '@epdffa'
			FROM dbo.EmployeePayrollBasicSalary AS epbs WITH (NOLOCK)
				INNER JOIN (
				SELECT TOP 1 EmployeeId
					,EffectiveDate
					,ReasonId
				FROM dbo.EmployeePayrollBasicSalaryHistory WITH (NOLOCK)
				WHERE EmployeeId = @EmployeeId
				ORDER BY HistoryId DESC
				) AS epbsh ON epbs.EmployeeId = epbsh.EmployeeId
				INNER JOIN dbo.EmployeePayrollAllowance AS epa WITH (NOLOCK) ON epbs.EmployeeId = epa.EmployeeId
				INNER JOIN dbo.EmployeePayrollDeduction AS epd WITH (NOLOCK) ON epbs.EmployeeId = epd.EmployeeId
			WHERE epbs.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Employee_UpdatePayroll'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END