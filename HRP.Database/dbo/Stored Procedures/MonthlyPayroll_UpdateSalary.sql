CREATE PROCEDURE [dbo].[MonthlyPayroll_UpdateSalary] @UserId INT
	,@PayrollId INT
	,@CompanyId SMALLINT
	,@Month DATE
	,@Salary VARCHAR(MAX) = NULL
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
	,@AdditionAmount VARCHAR(MAX) = NULL
	,@DeductionAmount VARCHAR(MAX) = NULL
	,@AdditionXml VARCHAR(MAX) = NULL
	,@DeductionXml VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180305@Tim.Trinh
Task: update salary
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
Update:
20180418@Elton: remove DirectbankingBankTypeId
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'
		,@startDate DATE
		,@dueDate DATE
		,@today DATE = GETDATE()
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	CREATE TABLE #tblAddition (
		[Type] TINYINT
		,UnitAmount VARCHAR(MAX)
		,NoOfDays DECIMAL(8, 2) NULL
		,NetSalary VARCHAR(MAX)
		,[Description] NVARCHAR(1000)
		)

	CREATE TABLE #tblDeduction (
		[Type] TINYINT
		,UnitAmount VARCHAR(MAX)
		,NoOfDays DECIMAL(8, 2) NULL
		,NetSalary VARCHAR(MAX)
		,[Description] NVARCHAR(1000)
		)

	-- Check permission
	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@Month)

	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, @functionKey) <> 2
		OR @today < @startDate
		OR @today > @dueDate
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You do not have permission to update basic salary.'

		RETURN
	END

	-- Write log before update
	SET @xml = (
			SELECT mp.PayrollId AS '@pid'
				,ms.Salary AS '@salary'
				,ms.EbankingCurrencyId AS '@ebcid'
				,ms.EbankingAmount AS '@ebamount'
				,ms.EbankingAccountId AS '@ebaccid'
				,ms.DirectBanking1CurrencyId AS '@db1cid'
				,ms.DirectBanking1Amount AS '@db1amount'
				,ms.DirectBanking1AccountId AS '@db1accid'
				,ms.DirectBanking2CurrencyId AS '@db2cid'
				,ms.DirectBanking2Amount AS '@db2amount'
				,ms.DirectBanking2AccountId AS '@db2accid'
				,ms.CashCurrencyId AS '@cashcid'
				,ms.CashAmount AS '@cashamount'
				,ms.CashAccountId AS '@cashaccid'
				,ms.AutoCalculateOn AS '@autocal'
				,ms.[Status] AS '@status'
				,(
					SELECT msa.[Type] AS '@type'
						,msa.UnitAmount AS '@amount'
						,msa.NoOfDays AS '@days'
						,msa.NetSalary AS '@salary'
						,msa.[Description] AS '@des'
					FROM dbo.MonthlySalaryAddition AS msa WITH (NOLOCK)
					WHERE msa.PayrollId = @PayrollId
					FOR XML PATH('z')
						,TYPE
					) AS 'msa'
				,(
					SELECT msd.[Type] AS '@type'
						,msd.UnitAmount AS '@amount'
						,msd.NoOfDays AS '@days'
						,msd.NetSalary AS '@salary'
						,msd.[Description] AS '@des'
					FROM dbo.MonthlySalaryDeduction AS msd WITH (NOLOCK)
					WHERE msd.PayrollId = @PayrollId
					FOR XML PATH('z')
						,TYPE
					) AS 'msd'
			FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
				INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
			WHERE mp.PayrollId = @PayrollId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@AdditionXml

	INSERT INTO #tblAddition (
		[Type]
		,UnitAmount
		,NoOfDays
		,NetSalary
		,[Description]
		)
	SELECT [Type]
		,UnitAmount
		,CASE 
			WHEN ISNUMERIC(NoOfDays) <> 1 THEN NULL
			ELSE NoOfDays
			END
		,NetSalary
		,[Description]
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH ([Type] TINYINT, UnitAmount VARCHAR(MAX), NoOfDays VARCHAR(20), NetSalary VARCHAR(MAX), [Description] NVARCHAR(1000)) AS data

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@DeductionXml

	INSERT INTO #tblDeduction (
		[Type]
		,UnitAmount
		,NoOfDays
		,NetSalary
		,[Description]
		)
	SELECT [Type]
		,UnitAmount
		,CASE 
			WHEN ISNUMERIC(NoOfDays) <> 1 THEN NULL
			ELSE NoOfDays
			END
		,NetSalary
		,[Description]
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH ([Type] TINYINT, UnitAmount VARCHAR(MAX), NoOfDays VARCHAR(20), NetSalary VARCHAR(MAX), [Description] NVARCHAR(1000)) AS data

	UPDATE ms
	SET ms.Salary = @Salary
		,ms.EbankingCurrencyId = @EbankingCurrencyId
		,ms.EbankingAmount = @EbankingAmount
		,ms.EbankingAccountId = @EbankingAccountId
		,ms.DirectBanking1CurrencyId = @DirectBanking1CurrencyId
		,ms.DirectBanking1Amount = @DirectBanking1Amount
		,ms.DirectBanking1AccountId = @DirectBanking1AccountId
		,ms.DirectBanking2CurrencyId = @DirectBanking2CurrencyId
		,ms.DirectBanking2Amount = @DirectBanking2Amount
		,ms.DirectBanking2AccountId = @DirectBanking2AccountId
		,ms.CashCurrencyId = @CashCurrencyId
		,ms.CashAmount = @CashAmount
		,ms.CashAccountId = @CashAccountId
		,ms.AutoCalculateOn = @AutoCalculateOn
		,ms.AdditionAmount = @AdditionAmount
		,ms.DeductionAmount = @DeductionAmount
		,ms.[Status] = 1 -- Satisfied
	FROM dbo.MonthlySalary AS ms WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON ms.PayrollId = mp.PayrollId
		AND mp.PayrollId = @PayrollId

	UPDATE msa
	SET msa.UnitAmount = tbl.UnitAmount
		,msa.NoOfDays = tbl.NoOfDays
		,msa.NetSalary = tbl.NetSalary
		,msa.[Description] = tbl.[Description]
	FROM dbo.MonthlySalaryAddition AS msa WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblAddition AS tbl WITH (NOLOCK) ON msa.[Type] = tbl.[Type]
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON msa.PayrollId = mp.PayrollId
		AND mp.PayrollId = @PayrollId

	INSERT INTO dbo.MonthlySalaryAddition (
		PayrollId
		,[Type]
		,UnitAmount
		,NoOfDays
		,NetSalary
		,[Description]
		)
	SELECT @PayrollId
		,tbl.[Type]
		,tbl.UnitAmount
		,tbl.NoOfDays
		,tbl.NetSalary
		,tbl.[Description]
	FROM #tblAddition AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.MonthlySalaryAddition AS msa WITH (NOLOCK) ON tbl.[Type] = msa.[Type]
		AND msa.PayrollId = @PayrollId
	WHERE msa.[Type] IS NULL

	UPDATE msd
	SET msd.UnitAmount = tbl.UnitAmount
		,msd.NoOfDays = tbl.NoOfDays
		,msd.NetSalary = tbl.NetSalary
		,msd.[Description] = tbl.[Description]
	FROM dbo.MonthlySalaryDeduction AS msd WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblDeduction AS tbl WITH (NOLOCK) ON msd.[Type] = tbl.[Type]
		INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON msd.PayrollId = mp.PayrollId
		AND mp.PayrollId = @PayrollId

	INSERT INTO dbo.MonthlySalaryDeduction (
		PayrollId
		,[Type]
		,UnitAmount
		,NoOfDays
		,NetSalary
		,[Description]
		)
	SELECT @PayrollId
		,tbl.[Type]
		,tbl.UnitAmount
		,tbl.NoOfDays
		,tbl.NetSalary
		,tbl.[Description]
	FROM #tblDeduction AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.MonthlySalaryDeduction AS msd WITH (NOLOCK) ON tbl.[Type] = msd.[Type]
		AND msd.PayrollId = @PayrollId
	WHERE msd.[Type] IS NULL

	-- write log after reset
	SET @xml = (
			SELECT mp.PayrollId AS '@pid'
				,ms.Salary AS '@salary'
				,ms.EbankingCurrencyId AS '@ebcid'
				,ms.EbankingAmount AS '@ebamount'
				,ms.EbankingAccountId AS '@ebaccid'
				,ms.DirectBanking1CurrencyId AS '@db1cid'
				,ms.DirectBanking1Amount AS '@db1amount'
				,ms.DirectBanking1AccountId AS '@db1accid'
				,ms.DirectBanking2CurrencyId AS '@db2cid'
				,ms.DirectBanking2Amount AS '@db2amount'
				,ms.DirectBanking2AccountId AS '@db2accid'
				,ms.CashCurrencyId AS '@cashcid'
				,ms.CashAmount AS '@cashamount'
				,ms.CashAccountId AS '@cashaccid'
				,ms.AutoCalculateOn AS '@autocal'
				,ms.[Status] AS '@status'
				,(
					SELECT msa.[Type] AS '@type'
						,msa.UnitAmount AS '@amount'
						,msa.NoOfDays AS '@days'
						,msa.NetSalary AS '@salary'
						,msa.[Description] AS '@des'
					FROM dbo.MonthlySalaryAddition AS msa WITH (NOLOCK)
					WHERE msa.PayrollId = @PayrollId
					FOR XML PATH('z')
						,TYPE
					) AS 'msa'
				,(
					SELECT msd.[Type] AS '@type'
						,msd.UnitAmount AS '@amount'
						,msd.NoOfDays AS '@days'
						,msd.NetSalary AS '@salary'
						,msd.[Description] AS '@des'
					FROM dbo.MonthlySalaryDeduction AS msd WITH (NOLOCK)
					WHERE msd.PayrollId = @PayrollId
					FOR XML PATH('z')
						,TYPE
					) AS 'msd'
			FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
				INNER JOIN dbo.MonthlySalary AS ms WITH (NOLOCK) ON mp.PayrollId = ms.PayrollId
			WHERE mp.PayrollId = @PayrollId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UserId
		,@SPName = 'MonthlyPayroll_UpdateSalary'
		,@ObjectId = @PayrollId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblAddition

	DROP TABLE #tblDeduction
END

GO

