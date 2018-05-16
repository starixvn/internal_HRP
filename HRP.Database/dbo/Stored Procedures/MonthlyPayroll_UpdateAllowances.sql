
CREATE PROCEDURE [dbo].[MonthlyPayroll_UpdateAllowances] @PayrollId INT
	,@CurrencyId INT = NULL
	,@TotalAmount VARCHAR(MAX) = NULL
	,@DirectBanking1CurrencyId INT = NULL
	,@DirectBanking1Amount VARCHAR(MAX) = NULL
	,@DirectBanking1AccountId INT = NULL
	,@DirectBanking2CurrencyId INT = NULL
	,@DirectBanking2Amount VARCHAR(MAX) = NULL
	,@DirectBanking2AccountId INT = NULL
	,@CashCurrencyId INT = NULL
	,@CashAmount VARCHAR(MAX) = NULL
	,@CashAccountId INT = NULL
	,@DSTNetSalaryCurrencyId INT = NULL
	,@DSTNetSalaryAmount VARCHAR(MAX) = NULL
	,@AutoCalculateOn VARCHAR(30)
	,@TotalNetAmount VARCHAR(MAX) = NULL
	,@Status TINYINT = NULL
	,@AdditionAmount VARCHAR(MAX) = NULL
	,@DeductionAmount VARCHAR(MAX) = NULL
	,@AllowanceAddition VARCHAR(MAX)
	,@AllowanceDeduction VARCHAR(MAX)
	,@UserId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180305@EltonTon
Task : payment process - save allowances
DB: bodb_HRP
Original:
Updated: 
 ---- 20180417@Elton: remove banktypeid
 */
BEGIN
	SET NOCOUNT ON

	DECLARE @companyId INT

	SELECT @companyId = e.CompanyId
	FROM dbo.Employment AS e WITH (NOLOCK)
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
		AND mp.PayrollId = @PayrollId

	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @companyId, 'PaymentProcess') <> 2
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You dont have permission to update this employee payroll'

		RETURN
	END

	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idocAddition INT
		,@idocDeduction INT

	CREATE TABLE #addition (
		PayrollId INT
		,[Type] TINYINT
		,NoOfDays DECIMAL(6, 2) NULL
		,UnitAmount VARCHAR(MAX)
		,TotalAmount VARCHAR(MAX)
		,NetAllowance VARCHAR(MAX)
		,CurrencyId INT
		)

	CREATE TABLE #deduction (
		PayrollId INT
		,[Type] TINYINT
		,NoOfDays DECIMAL(6, 2) NULL
		,UnitAmount VARCHAR(MAX)
		,TotalAmount VARCHAR(MAX)
		,NetAllowance VARCHAR(MAX)
		,CurrencyId INT
		)

	EXEC sp_xml_preparedocument @idocAddition OUTPUT
		,@AllowanceAddition

	EXEC sp_xml_preparedocument @idocDeduction OUTPUT
		,@AllowanceDeduction

	INSERT INTO #addition (
		PayrollId
		,[Type]
		,NoOfDays
		,UnitAmount
		,TotalAmount
		,NetAllowance
		,CurrencyId
		)
	SELECT @PayrollId
		,[Type]
		,CASE 
			WHEN ISNUMERIC(NoOfDays) <> 1 THEN NULL
			ELSE NoOfDays
			END
		,UnitAmount
		,TotalAmount
		,NetAllowance
		,CurrencyId
	FROM OPENXML(@idocAddition, '/Root/Row', 1) WITH ([Type] TINYINT, NoOfDays VARCHAR(20), UnitAmount VARCHAR(MAX), TotalAmount VARCHAR(MAX), NetAllowance NVARCHAR(MAX), CurrencyId TINYINT) AS data

	INSERT INTO #deduction (
		PayrollId
		,[Type]
		,NoOfDays
		,UnitAmount
		,TotalAmount
		,NetAllowance
		,CurrencyId
		)
	SELECT @PayrollId
		,[Type]
		,CASE 
			WHEN ISNUMERIC(NoOfDays) <> 1 THEN NULL
			ELSE NoOfDays
			END
		,UnitAmount
		,TotalAmount
		,NetAllowance
		,CurrencyId
	FROM OPENXML(@idocDeduction, '/Root/Row', 1) WITH ([Type] TINYINT, NoOfDays VARCHAR(20), UnitAmount VARCHAR(MAX), TotalAmount VARCHAR(MAX), NetAllowance NVARCHAR(MAX), CurrencyId TINYINT) AS data

	SET @xml = (
			SELECT [PayrollId] AS '@pi'
				,[CurrencyId] AS '@ci'
				,[TotalAmount] AS '@ta'
				,[DirectBanking1CurrencyId] AS '@db1ci'
				,[DirectBanking1Amount] AS '@db1a'
				,[DirectBanking1AccountId] AS '@db1ai'
				,[DirectBanking2CurrencyId] AS '@db2ci'
				,[DirectBanking2Amount] AS '@db2a'
				,[DirectBanking2AccountId] AS '@db2ai'
				,[CashCurrencyId] AS '@cci'
				,[CashAmount] AS '@ca'
				,[CashAccountId] AS '@cai'
				,[DSTNetSalaryCurrencyId] AS '@dnsci'
				,[DSTNetSalaryAmount] AS '@dnsa'
				,[AutoCalculateOn] AS '@aco'
				,[TotalNetAmount] AS '@tna'
				,[Status] AS '@st'
				,[AdditionAmount] AS '@aa'
				,[DeductionAmount] AS '@da'
				,(
					SELECT maa.[Type] AS '@t'
						,maa.UnitAmount AS '@ua'
						,maa.NoOfDays AS '@nod'
						,maa.TotalAmount AS '@ta'
						,maa.NetAllowance AS '@na'
						,maa.CurrencyId AS '@ci'
					FROM dbo.MonthlyAllowanceAddition AS maa WITH (NOLOCK)
					WHERE maa.PayrollId = @PayrollId
					FOR XML PATH('maa')
						,TYPE
					) AS 'maas'
				,(
					SELECT mad.[Type] AS '@t'
						,mad.UnitAmount AS '@ua'
						,mad.NoOfDays AS '@nod'
						,mad.TotalAmount AS '@ta'
						,mad.NetAllowance AS '@na'
						,mad.CurrencyId AS '@ci'
					FROM dbo.MonthlyAllowanceDeduction AS mad WITH (NOLOCK)
					WHERE mad.PayrollId = @PayrollId
					FOR XML PATH('mad')
						,TYPE
					) AS 'mads'
			FROM [dbo].[MonthlyAllowance] WITH (NOLOCK)
			WHERE PayrollId = @PayrollId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE ma
	SET [CurrencyId] = @CurrencyId
		,[TotalAmount] = @TotalAmount
		,[DirectBanking1CurrencyId] = @DirectBanking1CurrencyId
		,[DirectBanking1Amount] = @DirectBanking1Amount
		,[DirectBanking1AccountId] = @DirectBanking1AccountId
		,[DirectBanking2CurrencyId] = @DirectBanking2CurrencyId
		,[DirectBanking2Amount] = @DirectBanking2Amount
		,[DirectBanking2AccountId] = @DirectBanking2AccountId
		,[CashCurrencyId] = @CashCurrencyId
		,[CashAmount] = @CashAmount
		,[DSTNetSalaryCurrencyId] = @DSTNetSalaryCurrencyId
		,[DSTNetSalaryAmount] = @DSTNetSalaryAmount
		,[AutoCalculateOn] = @AutoCalculateOn
		,[TotalNetAmount] = @TotalNetAmount
		,[Status] = @Status
		,[AdditionAmount] = @AdditionAmount
		,[DeductionAmount] = @DeductionAmount
	FROM [dbo].[MonthlyAllowance] AS ma WITH (ROWLOCK, UPDLOCK)
	WHERE ma.PayrollId = @PayrollId

	UPDATE maa
	SET maa.UnitAmount = a.UnitAmount
		,maa.NoOfDays = a.NoOfDays
		,maa.TotalAmount = a.TotalAmount
		,maa.NetAllowance = a.NetAllowance
		,maa.CurrencyId = a.CurrencyId
	FROM dbo.MonthlyAllowanceAddition AS maa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #addition AS a ON maa.PayrollId = a.PayrollId
		AND maa.[Type] = a.[Type]

	UPDATE mad
	SET mad.UnitAmount = d.UnitAmount
		,mad.NoOfDays = d.NoOfDays
		,mad.TotalAmount = d.TotalAmount
		,mad.NetAllowance = d.NetAllowance
		,mad.CurrencyId = d.CurrencyId
	FROM dbo.MonthlyAllowanceDeduction AS mad WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #deduction AS d ON mad.PayrollId = d.PayrollId
		AND mad.[Type] = d.[Type]

	SET @xml = (
			SELECT [PayrollId] AS '@pi'
				,[CurrencyId] AS '@ci'
				,[TotalAmount] AS '@ta'
				,[DirectBanking1CurrencyId] AS '@db1ci'
				,[DirectBanking1Amount] AS '@db1a'
				,[DirectBanking1AccountId] AS '@db1ai'
				,[DirectBanking2CurrencyId] AS '@db2ci'
				,[DirectBanking2Amount] AS '@db2a'
				,[DirectBanking2AccountId] AS '@db2ai'
				,[CashCurrencyId] AS '@cci'
				,[CashAmount] AS '@ca'
				,[CashAccountId] AS '@cai'
				,[DSTNetSalaryCurrencyId] AS '@dnsci'
				,[DSTNetSalaryAmount] AS '@dnsa'
				,[AutoCalculateOn] AS '@aco'
				,[TotalNetAmount] AS '@tna'
				,[Status] AS '@st'
				,[AdditionAmount] AS '@aa'
				,[DeductionAmount] AS '@da'
				,(
					SELECT maa.[Type] AS '@t'
						,maa.UnitAmount AS '@ua'
						,maa.NoOfDays AS '@nod'
						,maa.TotalAmount AS '@ta'
						,maa.NetAllowance AS '@na'
						,maa.CurrencyId AS '@ci'
					FROM dbo.MonthlyAllowanceAddition AS maa WITH (NOLOCK)
					WHERE maa.PayrollId = @PayrollId
					FOR XML PATH('maa')
						,TYPE
					) AS 'maas'
				,(
					SELECT mad.[Type] AS '@t'
						,mad.UnitAmount AS '@ua'
						,mad.NoOfDays AS '@nod'
						,mad.TotalAmount AS '@ta'
						,mad.NetAllowance AS '@na'
						,mad.CurrencyId AS '@ci'
					FROM dbo.MonthlyAllowanceDeduction AS mad WITH (NOLOCK)
					WHERE mad.PayrollId = @PayrollId
					FOR XML PATH('mad')
						,TYPE
					) AS 'mads'
			FROM [dbo].[MonthlyAllowance] WITH (NOLOCK)
			WHERE PayrollId = @PayrollId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UserId
		,@SPName = 'MonthlyPayroll_UpdateAllowances'
		,@ObjectId = @PayrollId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #addition

	DROP TABLE #deduction
END