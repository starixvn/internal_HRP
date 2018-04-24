
CREATE PROCEDURE [dbo].[Currency_Update] @CurrencyId SMALLINT
	,@CurrencyCode NVARCHAR(50)
	,@CurrencyName NVARCHAR(200) = ''
	,@IsActive BIT = 0
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180222@Raymond.Le
Task: Update Currency
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML
		,@month DATE
		,@isUsedInEPA BIT
		,@isUsedInEPB BIT
		,@isUsedInEPD BIT
		,@isUsedInMP BIT

	CREATE TABLE #tblCurrentDuration (
		[Month] DATE
		,StartDate DATE
		,DueDate DATE
		)

	SET @startRun = GETDATE()

	-- Check exists Currency
	IF EXISTS (
			SELECT 1
			FROM dbo.Currency WITH (NOLOCK)
			WHERE CurrencyCode = @CurrencyCode
				AND CurrencyId <> @CurrencyId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'The ''' + @CurrencyCode + ''' has already existed!'

		RETURN
	END

	-- Check if Currency is using when change to InActive
	IF EXISTS (
			SELECT 1
			FROM dbo.Currency AS c WITH (NOLOCK)
			WHERE c.CurrencyId = @CurrencyId
				AND c.IsActive = 1
				AND @IsActive = 0
			)
	BEGIN
		INSERT INTO #tblCurrentDuration
		EXEC dbo.[PaymentProcess_GetCurrentDuration]

		SELECT @month = [Month]
		FROM #tblCurrentDuration

		SELECT @isUsedInEPA = 1
		FROM dbo.EmployeePayrollAllowance AS ea WITH (NOLOCK)
		WHERE ea.CurrencyId = @CurrencyId
			OR ea.DirectBanking1CurrencyId = @CurrencyId
			OR ea.DirectBanking2CurrencyId = @CurrencyId
			OR ea.CashCurrencyId = @CurrencyId
			OR ea.SubsistenceCurrencyId = @CurrencyId
			OR ea.HomePassageCurrencyId = @CurrencyId
			OR ea.HousingCurrencyId = @CurrencyId
			OR ea.MobilePhoneCurrencyId = @CurrencyId
			OR ea.HPICurrencyId = @CurrencyId
			OR ea.DutyMealCurrencyId = @CurrencyId
			OR ea.SecondmentCurrencyId = @CurrencyId
			OR ea.OthersCurrencyId = @CurrencyId

		SELECT @isUsedInEPB = 1
		FROM dbo.EmployeePayrollBasicSalary WITH (NOLOCK)
		WHERE CurrencyId = @CurrencyId
			OR EbankingCurrencyId = @CurrencyId
			OR DirectBanking1CurrencyId = @CurrencyId
			OR DirectBanking2CurrencyId = @CurrencyId
			OR CashCurrencyId = @CurrencyId

		SELECT @isUsedInEPD = 1
		FROM dbo.EmployeePayrollDeduction WITH (NOLOCK)
		WHERE RoomingChargesCurrencyId = @CurrencyId
			OR FitnessFirstCurrencyId = @CurrencyId

		SELECT @isUsedInMP = 1
		FROM dbo.MonthlyPayroll AS p WITH (NOLOCK)
		LEFT JOIN dbo.MonthlyAllowance AS a WITH (NOLOCK) ON p.PayrollId = a.PayrollId
		LEFT JOIN dbo.MonthlyAllowanceAddition AS aa WITH (NOLOCK) ON p.PayrollId = aa.PayrollId
		LEFT JOIN dbo.MonthlyAllowanceDeduction AS ad WITH (NOLOCK) ON p.PayrollId = ad.PayrollId
		LEFT JOIN dbo.MonthlyBasicAllowance AS ba WITH (NOLOCK) ON p.PayrollId = ba.PayrollId
		LEFT JOIN dbo.MonthlyBasicSalary AS bs WITH (NOLOCK) ON p.PayrollId = bs.PayrollId
		LEFT JOIN dbo.MonthlyDSTNetSalary AS ds WITH (NOLOCK) ON p.EmployeeId = ds.EmployeeId
			AND p.[Month] = ds.[Month]
		LEFT JOIN dbo.MonthlySalary AS s WITH (NOLOCK) ON p.PayrollId = s.PayrollId
		WHERE p.[Month] = @month
			AND (
				a.CurrencyId = @CurrencyId
				OR a.DirectBanking1CurrencyId = @CurrencyId
				OR a.DirectBanking2CurrencyId = @CurrencyId
				OR a.CashCurrencyId = @CurrencyId
				OR a.DSTNetSalaryCurrencyId = @CurrencyId
				OR aa.CurrencyId = @CurrencyId
				OR ad.CurrencyId = @CurrencyId
				OR ba.CurrencyId = @CurrencyId
				OR ba.DirectBanking1CurrencyId = @CurrencyId
				OR ba.DirectBanking2CurrencyId = @CurrencyId
				OR ba.CashCurrencyId = @CurrencyId
				OR ba.[SubsistenceCurrencyId] = @CurrencyId
				OR ba.[HomePassageCurrencyId] = @CurrencyId
				OR ba.[HousingCurrencyId] = @CurrencyId
				OR ba.[MobilePhoneCurrencyId] = @CurrencyId
				OR ba.[HPICurrencyId] = @CurrencyId
				OR ba.[DutyMealCurrencyId] = @CurrencyId
				OR ba.[SecondmentCurrencyId] = @CurrencyId
				OR ba.[OthersCurrencyId] = @CurrencyId
				OR ba.[RoomingChargesCurrencyId] = @CurrencyId
				OR ba.[FitnessFirstCurrencyId] = @CurrencyId
				OR bs.[CurrencyId] = @CurrencyId
				OR bs.[EbankingCurrencyId] = @CurrencyId
				OR bs.[DirectBanking1CurrencyId] = @CurrencyId
				OR bs.[DirectBanking2CurrencyId] = @CurrencyId
				OR bs.[CashCurrencyId] = @CurrencyId
				OR ds.CurrencyId = @CurrencyId
				OR s.[CurrencyId] = @CurrencyId
				OR s.[EbankingCurrencyId] = @CurrencyId
				OR s.[DirectBanking1CurrencyId] = @CurrencyId
				OR s.[DirectBanking2CurrencyId] = @CurrencyId
				OR s.[CashCurrencyId] = @CurrencyId
				)

		IF (
				@isUsedInEPA = 1
				OR @isUsedInEPB = 1
				OR @isUsedInEPD = 1
				OR @isUsedInMP = 1
				)
		BEGIN
			SET @ErrCode = 1
			SET @ErrMsg = 'You can not inactive an item which is in used'

			RETURN
		END
	END

	SET @xml = (
			SELECT c.CurrencyId AS '@cid'
				,c.CurrencyCode AS '@c'
				,c.CurrencyName AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.Currency AS c WITH (NOLOCK)
			WHERE c.CurrencyId = @CurrencyId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].Currency
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET CurrencyCode = @CurrencyCode
		,CurrencyName = @CurrencyName
		,IsActive = @IsActive
	WHERE CurrencyId = @CurrencyId
	--Write Log
	SET @xml = (
			SELECT c.CurrencyId AS '@cid'
				,c.CurrencyCode AS '@c'
				,c.CurrencyName AS '@n'
				,c.IsActive AS '@ia'
			FROM dbo.Currency AS c WITH (NOLOCK)
			WHERE c.CurrencyId = @CurrencyId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Currency_Update'
		,@ObjectId = @CurrencyId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration

	DROP TABLE #tblCurrentDuration
END