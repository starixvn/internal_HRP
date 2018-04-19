CREATE PROCEDURE [dbo].[Employee_UpdatePayroll] @EmployeeId INT
	,@Salary VARCHAR(MAX) = NULL
	,@IsSalaryChanged BIT
	,@CurrencyId INT = NULL
	,@WorkingDays TINYINT
	,@EbankingCurrencyId INT = NULL
	,@EbankingAmount VARCHAR(MAX) = NULL
	,@EbankingAccountId INT = NULL
	,@DirectBanking1CurrencyId INT = NULL
	,@DirectBanking1Amount VARCHAR(MAX) = NULL
	,@DirectBanking1AccountId INT = NULL
	,@DirectBanking2CurrencyId INT = NULL
	,@DirectBanking2Amount VARCHAR(MAX) = NULL
	,@DirectBanking2AccountId INT = NULL
	,@CashCurrencyId INT = NULL
	,@CashAmount VARCHAR(MAX) = NULL
	,@CashAccountId INT = NULL
	,@AutoCalculateOn VARCHAR(30) = NULL
	,@EffectiveDate DATE = NULL
	,@ReasonId INT = NULL
	,@AllowanceCurrencyId INT = NULL
	,@AllowanceDirectBanking1CurrencyId INT = NULL
	,@AllowanceDirectBanking1Amount VARCHAR(MAX) = NULL
	,@AllowanceDirectBanking1AccountId INT = NULL
	,@AllowanceDirectBanking2CurrencyId INT = NULL
	,@AllowanceDirectBanking2Amount VARCHAR(MAX) = NULL
	,@AllowanceDirectBanking2AccountId INT = NULL
	,@AllowanceCashCurrencyId INT = NULL
	,@AllowanceCashAmount VARCHAR(MAX) = NULL
	,@AllowanceCashAccountId INT = NULL
	,@AllowanceAutoCalculateOn VARCHAR(30) = NULL
	,@SubsistenceCurrencyId INT = NULL
	,@SubsistenceAmount VARCHAR(MAX) = NULL
	,@HomePassageCurrencyId INT = NULL
	,@HomePassageAmount VARCHAR(MAX) = NULL
	,@HousingCurrencyId INT = NULL
	,@HousingAmount VARCHAR(MAX) = NULL
	,@MobilePhoneCurrencyId INT = NULL
	,@MobilePhoneAmount VARCHAR(MAX) = NULL
	,@HPICurrencyId INT = NULL
	,@HPIAmount VARCHAR(MAX) = NULL
	,@DutyMealCurrencyId INT = NULL
	,@DutyMealAmount VARCHAR(MAX) = NULL
	,@SecondmentCurrencyId INT = NULL
	,@SecondmentAmount VARCHAR(MAX) = NULL
	,@OthersCurrencyId INT = NULL
	,@OthersAmount VARCHAR(MAX) = NULL
	,@RoomingChargesCurrencyId INT = NULL
	,@RoomingChargesAmount VARCHAR(MAX) = NULL
	,@FitnessFirstCurrencyId INT = NULL
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
Updated: 
 ---- 20180417@Elton: dont need to join RefResource for contra co account, remove banktypeid
 */
BEGIN
	SET NOCOUNT ON

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@companyId SMALLINT
		,@functionKey VARCHAR(50) = 'EmployeePayroll'

	SET @startRun = GETDATE()

	SELECT @companyId = e.CompanyId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
	WHERE emp.EmployeeId = @EmployeeId

	-- check permission
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UpdatedBy, @companyId, @functionKey) <> 2
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
					,epbs.CurrencyId AS '@epbscc'
					,epbs.Salary AS '@epbss'
					,epbs.WorkingDays AS '@epbswd'
					,epbs.EbankingCurrencyId AS '@epbsecc'
					,epbs.EbankingAmount AS '@epbsea'
					,epbs.EbankingAccountId AS '@epbseai'
					,epbs.DirectBanking1CurrencyId AS '@epbsdb1cc'
					,epbs.DirectBanking1Amount AS '@epbsdb1a'
					,epbs.DirectBanking1AccountId AS '@epbsdb1ai'
					,epbs.DirectBanking2CurrencyId AS '@epbsdb2cc'
					,epbs.DirectBanking2Amount AS '@epbsdb2a'
					,epbs.DirectBanking2AccountId AS '@epbsdb2ai'
					,epbs.CashCurrencyId AS '@epbsccc'
					,epbs.CashAmount AS '@epbsca'
					,epbs.CashAccountId AS '@epbscai'
					,epbs.AutoCalculateOn AS '@epbsaco'
					,epbsh.EffectiveDate AS '@epbsed'
					,epbsh.ReasonId AS '@epbsri'
					,epa.CurrencyId AS '@epacc'
					,epa.DirectBanking1CurrencyId AS '@epadb1cc'
					,epa.DirectBanking1Amount AS '@epadb1a'
					,epa.DirectBanking1AccountId AS '@epadb1ai'
					,epa.DirectBanking2CurrencyId AS '@epadb2cc'
					,epa.DirectBanking2Amount AS '@epadb2a'
					,epa.DirectBanking2AccountId AS '@epadb2ai'
					,epa.CashCurrencyId AS '@epaccc'
					,epa.CashAmount AS '@epaca'
					,epa.CashAccountId AS '@epacai'
					,epa.AutoCalculateOn AS '@epaaco'
					,epa.SubsistenceCurrencyId AS '@epasscc'
					,epa.SubsistenceAmount AS '@epassa'
					,epa.HomePassageCurrencyId AS '@epahpcc'
					,epa.HomePassageAmount AS '@epahpa'
					,epa.HousingCurrencyId AS '@epahcc'
					,epa.HousingAmount AS '@epaha'
					,epa.MobilePhoneCurrencyId AS '@epampcc'
					,epa.MobilePhoneAmount AS '@epampa'
					,epa.HPICurrencyId AS '@epahpicc'
					,epa.HPIAmount AS '@epahpia'
					,epa.DutyMealCurrencyId AS '@epadmcc'
					,epa.DutyMealAmount AS '@epadma'
					,epa.SecondmentCurrencyId AS '@epascc'
					,epa.SecondmentAmount AS '@epasa'
					,epa.OthersCurrencyId AS '@epaocc'
					,epa.OthersAmount AS '@epaoa'
					,epd.RoomingChargesCurrencyId AS '@epdrccc'
					,epd.RoomingChargesAmount AS '@epdrca'
					,epd.FitnessFirstCurrencyId AS '@epdffcc'
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
				,[CurrencyId]
				,[EffectiveDate]
				,[ReasonId]
				)
			VALUES (
				@EmployeeId
				,@Salary
				,@CurrencyId
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
				,CurrencyId = @CurrencyId
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
			,[CurrencyId] = @CurrencyId
			,[WorkingDays] = @WorkingDays
			,[EbankingCurrencyId] = @EbankingCurrencyId
			,[EbankingAmount] = @EbankingAmount
			,[EbankingAccountId] = @EbankingAccountId
			,[DirectBanking1CurrencyId] = @DirectBanking1CurrencyId
			,[DirectBanking1Amount] = @DirectBanking1Amount
			,[DirectBanking1AccountId] = @DirectBanking1AccountId
			,[DirectBanking2CurrencyId] = @DirectBanking2CurrencyId
			,[DirectBanking2Amount] = @DirectBanking2Amount
			,[DirectBanking2AccountId] = @DirectBanking2AccountId
			,[CashCurrencyId] = @CashCurrencyId
			,[CashAmount] = @CashAmount
			,[CashAccountId] = @CashAccountId
			,[AutoCalculateOn] = @AutoCalculateOn
		WHERE EmployeeId = @EmployeeId

		UPDATE [dbo].[EmployeePayrollAllowance] WITH (ROWLOCK, UPDLOCK)
		SET [CurrencyId] = @AllowanceCurrencyId
			,[DirectBanking1CurrencyId] = @AllowanceDirectBanking1CurrencyId
			,[DirectBanking1Amount] = @AllowanceDirectBanking1Amount
			,[DirectBanking1AccountId] = @AllowanceDirectBanking1AccountId
			,[DirectBanking2CurrencyId] = @AllowanceDirectBanking2CurrencyId
			,[DirectBanking2Amount] = @AllowanceDirectBanking2Amount
			,[DirectBanking2AccountId] = @AllowanceDirectBanking2AccountId
			,[CashCurrencyId] = @AllowanceCashCurrencyId
			,[CashAmount] = @AllowanceCashAmount
			,[CashAccountId] = @AllowanceCashAccountId
			,[AutoCalculateOn] = @AllowanceAutoCalculateOn
			,[SubsistenceCurrencyId] = @SubsistenceCurrencyId
			,[SubsistenceAmount] = @SubsistenceAmount
			,[HomePassageCurrencyId] = @HomePassageCurrencyId
			,[HomePassageAmount] = @HomePassageAmount
			,[HousingCurrencyId] = @HousingCurrencyId
			,[HousingAmount] = @HousingAmount
			,[MobilePhoneCurrencyId] = @MobilePhoneCurrencyId
			,[MobilePhoneAmount] = @MobilePhoneAmount
			,[HPICurrencyId] = @HPICurrencyId
			,[HPIAmount] = @HPIAmount
			,[DutyMealCurrencyId] = @DutyMealCurrencyId
			,[DutyMealAmount] = @DutyMealAmount
			,[SecondmentCurrencyId] = @SecondmentCurrencyId
			,[SecondmentAmount] = @SecondmentAmount
			,[OthersCurrencyId] = @OthersCurrencyId
			,[OthersAmount] = @OthersAmount
		WHERE EmployeeId = @EmployeeId

		UPDATE [dbo].[EmployeePayrollDeduction] WITH (ROWLOCK, UPDLOCK)
		SET [RoomingChargesCurrencyId] = @RoomingChargesCurrencyId
			,[RoomingChargesAmount] = @RoomingChargesAmount
			,[FitnessFirstCurrencyId] = @FitnessFirstCurrencyId
			,[FitnessFirstAmount] = @FitnessFirstAmount
		WHERE EmployeeId = @EmployeeId
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[EmployeePayrollBasicSalary] (
			[EmployeeId]
			,[Salary]
			,[CurrencyId]
			,[WorkingDays]
			,[EbankingCurrencyId]
			,[EbankingAmount]
			,[EbankingAccountId]
			,[DirectBanking1CurrencyId]
			,[DirectBanking1Amount]
			,[DirectBanking1AccountId]
			,[DirectBanking2CurrencyId]
			,[DirectBanking2Amount]
			,[DirectBanking2AccountId]
			,[CashCurrencyId]
			,[CashAmount]
			,[CashAccountId]
			,[AutoCalculateOn]
			)
		VALUES (
			@EmployeeId
			,@Salary
			,@CurrencyId
			,@WorkingDays
			,@EbankingCurrencyId
			,@EbankingAmount
			,@EbankingAccountId
			,@DirectBanking1CurrencyId
			,@DirectBanking1Amount
			,@DirectBanking1AccountId
			,@DirectBanking2CurrencyId
			,@DirectBanking2Amount
			,@DirectBanking2AccountId
			,@CashCurrencyId
			,@CashAmount
			,@CashAccountId
			,@AutoCalculateOn
			)

		INSERT INTO [dbo].[EmployeePayrollBasicSalaryHistory] (
			[EmployeeId]
			,[Salary]
			,[CurrencyId]
			,[EffectiveDate]
			,[ReasonId]
			)
		VALUES (
			@EmployeeId
			,@Salary
			,@CurrencyId
			,@EffectiveDate
			,@ReasonId
			)

		INSERT INTO [dbo].[EmployeePayrollAllowance] (
			[EmployeeId]
			,[CurrencyId]
			,[DirectBanking1CurrencyId]
			,[DirectBanking1Amount]
			,[DirectBanking1AccountId]
			,[DirectBanking2CurrencyId]
			,[DirectBanking2Amount]
			,[DirectBanking2AccountId]
			,[CashCurrencyId]
			,[CashAmount]
			,[CashAccountId]
			,[AutoCalculateOn]
			,[SubsistenceCurrencyId]
			,[SubsistenceAmount]
			,[HomePassageCurrencyId]
			,[HomePassageAmount]
			,[HousingCurrencyId]
			,[HousingAmount]
			,[MobilePhoneCurrencyId]
			,[MobilePhoneAmount]
			,[HPICurrencyId]
			,[HPIAmount]
			,[DutyMealCurrencyId]
			,[DutyMealAmount]
			,[SecondmentCurrencyId]
			,[SecondmentAmount]
			,[OthersCurrencyId]
			,[OthersAmount]
			)
		VALUES (
			@EmployeeId
			,@AllowanceCurrencyId
			,@AllowanceDirectBanking1CurrencyId
			,@AllowanceDirectBanking1Amount
			,@AllowanceDirectBanking1AccountId
			,@AllowanceDirectBanking2CurrencyId
			,@AllowanceDirectBanking2Amount
			,@AllowanceDirectBanking2AccountId
			,@AllowanceCashCurrencyId
			,@AllowanceCashAmount
			,@AllowanceCashAccountId
			,@AllowanceAutoCalculateOn
			,@SubsistenceCurrencyId
			,@SubsistenceAmount
			,@HomePassageCurrencyId
			,@HomePassageAmount
			,@HousingCurrencyId
			,@HousingAmount
			,@MobilePhoneCurrencyId
			,@MobilePhoneAmount
			,@HPICurrencyId
			,@HPIAmount
			,@DutyMealCurrencyId
			,@DutyMealAmount
			,@SecondmentCurrencyId
			,@SecondmentAmount
			,@OthersCurrencyId
			,@OthersAmount
			)

		INSERT INTO [dbo].[EmployeePayrollDeduction] (
			[EmployeeId]
			,[RoomingChargesCurrencyId]
			,[RoomingChargesAmount]
			,[FitnessFirstCurrencyId]
			,[FitnessFirstAmount]
			)
		VALUES (
			@EmployeeId
			,@RoomingChargesCurrencyId
			,@RoomingChargesAmount
			,@FitnessFirstCurrencyId
			,@FitnessFirstAmount
			)
	END

	SET @xml = (
			SELECT epbs.EmployeeId AS '@ei'
				,epbs.CurrencyId AS '@epbscc'
				,epbs.Salary AS '@epbss'
				,epbs.WorkingDays AS '@epbswd'
				,epbs.EbankingCurrencyId AS '@epbsecc'
				,epbs.EbankingAmount AS '@epbsea'
				,epbs.EbankingAccountId AS '@epbseai'
				,epbs.DirectBanking1CurrencyId AS '@epbsdb1cc'
				,epbs.DirectBanking1Amount AS '@epbsdb1a'
				,epbs.DirectBanking1AccountId AS '@epbsdb1ai'
				,epbs.DirectBanking2CurrencyId AS '@epbsdb2cc'
				,epbs.DirectBanking2Amount AS '@epbsdb2a'
				,epbs.DirectBanking2AccountId AS '@epbsdb2ai'
				,epbs.CashCurrencyId AS '@epbsccc'
				,epbs.CashAmount AS '@epbsca'
				,epbs.CashAccountId AS '@epbscai'
				,epbs.AutoCalculateOn AS '@epbsaco'
				,epbsh.EffectiveDate AS '@epbsed'
				,epbsh.ReasonId AS '@epbsri'
				,epa.CurrencyId AS '@epacc'
				,epa.DirectBanking1CurrencyId AS '@epadb1cc'
				,epa.DirectBanking1Amount AS '@epadb1a'
				,epa.DirectBanking1AccountId AS '@epadb1ai'
				,epa.DirectBanking2CurrencyId AS '@epadb2cc'
				,epa.DirectBanking2Amount AS '@epadb2a'
				,epa.DirectBanking2AccountId AS '@epadb2ai'
				,epa.CashCurrencyId AS '@epaccc'
				,epa.CashAmount AS '@epaca'
				,epa.CashAccountId AS '@epacai'
				,epa.AutoCalculateOn AS '@epaaco'
				,epa.SubsistenceCurrencyId AS '@epasscc'
				,epa.SubsistenceAmount AS '@epassa'
				,epa.HomePassageCurrencyId AS '@epahpcc'
				,epa.HomePassageAmount AS '@epahpa'
				,epa.HousingCurrencyId AS '@epahcc'
				,epa.HousingAmount AS '@epaha'
				,epa.MobilePhoneCurrencyId AS '@epampcc'
				,epa.MobilePhoneAmount AS '@epampa'
				,epa.HPICurrencyId AS '@epahpicc'
				,epa.HPIAmount AS '@epahpia'
				,epa.DutyMealCurrencyId AS '@epadmcc'
				,epa.DutyMealAmount AS '@epadma'
				,epa.SecondmentCurrencyId AS '@epascc'
				,epa.SecondmentAmount AS '@epasa'
				,epa.OthersCurrencyId AS '@epaocc'
				,epa.OthersAmount AS '@epaoa'
				,epd.RoomingChargesCurrencyId AS '@epdrccc'
				,epd.RoomingChargesAmount AS '@epdrca'
				,epd.FitnessFirstCurrencyId AS '@epdffcc'
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

GO

