
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlySalary] @Month DATE
	,@SalaryXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Salary
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
		,@iDoc INT
		,@ObjectId INT
		,@i INT = 1
		,@maxLength INT

	CREATE TABLE #tblTempMonthlySalary (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,[PayrollId] INT
		,[CurrencyId] INT
		,[Salary] VARCHAR(MAX)
		,[EbankingCurrencyId] INT
		,[EbankingAmount] VARCHAR(MAX)
		,[EbankingAccountId] INT
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking1AccountId] INT
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[DirectBanking2AccountId] INT
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[CashAccountId] INT
		,[AutoCalculateOn] VARCHAR(30)
		,[AdditionAmount] VARCHAR(MAX)
		,[DeductionAmount] VARCHAR(MAX)
		,[Status] TINYINT
		)

	CREATE TABLE #tblOldLogData (
		[PayrollId] INT
		,[CurrencyId] INT
		,[Salary] VARCHAR(MAX)
		,[EbankingCurrencyId] INT
		,[EbankingAmount] VARCHAR(MAX)
		,[EbankingAccountId] INT
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking1AccountId] INT
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[DirectBanking2AccountId] INT
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[CashAccountId] INT
		,[AutoCalculateOn] VARCHAR(30)
		,[AdditionAmount] VARCHAR(MAX)
		,[DeductionAmount] VARCHAR(MAX)
		,[Status] TINYINT
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@SalaryXMLDoc

	INSERT INTO #tblTempMonthlySalary (
		[PayrollId]
		,[CurrencyId]
		,[Salary]
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
		,[AdditionAmount]
		,[DeductionAmount]
		,[Status]
		)
	SELECT mp.PayrollId
		,NULLIF(ISNULL(Data.[CurrencyId], ''), '')
		,Data.[Salary]
		,NULLIF(ISNULL(Data.[EbankingCurrencyId], ''), '')
		,Data.[EbankingAmount]
		,NULLIF(ISNULL(Data.[EbankingAccountId], ''), '')
		,NULLIF(ISNULL(Data.[DirectBanking1CurrencyId], ''), '')
		,Data.[DirectBanking1Amount]
		,NULLIF(ISNULL(Data.[DirectBanking1AccountId], ''), '')
		,NULLIF(ISNULL(Data.[DirectBanking2CurrencyId], ''), '')
		,Data.[DirectBanking2Amount]
		,NULLIF(ISNULL(Data.[DirectBanking2AccountId], ''), '')
		,NULLIF(ISNULL(Data.[CashCurrencyId], ''), '')
		,Data.[CashAmount]
		,NULLIF(ISNULL(Data.[CashAccountId], ''), '')
		,Data.[AutoCalculateOn]
		,Data.[AdditionAmount]
		,Data.[DeductionAmount]
		,Data.[Status]
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[CurrencyId] VARCHAR(50)
			,[Salary] VARCHAR(MAX)
			,[EbankingCurrencyId] VARCHAR(50)
			,[EbankingAmount] VARCHAR(MAX)
			,[EbankingAccountId] VARCHAR(50)
			,[DirectBanking1CurrencyId] VARCHAR(50)
			,[DirectBanking1Amount] VARCHAR(MAX)
			,[DirectBanking1AccountId] VARCHAR(50)
			,[DirectBanking2CurrencyId] VARCHAR(50)
			,[DirectBanking2Amount] VARCHAR(MAX)
			,[DirectBanking2AccountId] VARCHAR(50)
			,[CashCurrencyId] VARCHAR(50)
			,[CashAmount] VARCHAR(MAX)
			,[CashAccountId] VARCHAR(50)
			,[AutoCalculateOn] VARCHAR(30)
			,[AdditionAmount] VARCHAR(MAX)
			,[DeductionAmount] VARCHAR(MAX)
			,[Status] TINYINT
			) AS Data
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlySalary AS s
	USING (
		SELECT [PayrollId]
			,[CurrencyId]
			,[Salary]
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
			,[AdditionAmount]
			,[DeductionAmount]
			,[Status]
		FROM #tblTempMonthlySalary WITH (NOLOCK)
		) AS val([PayrollId], [CurrencyId], [Salary], [EbankingCurrencyId], [EbankingAmount], [EbankingAccountId], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking1AccountId], [DirectBanking2CurrencyId], [DirectBanking2Amount], [DirectBanking2AccountId], [CashCurrencyId], [CashAmount], [CashAccountId], [AutoCalculateOn], [AdditionAmount], [DeductionAmount], [Status])
		ON s.PayrollId = val.PayrollId
	WHEN MATCHED
		THEN
			UPDATE
			SET s.[CurrencyId] = val.[CurrencyId]
				,s.[Salary] = val.[Salary]
				,s.[EbankingCurrencyId] = val.[EbankingCurrencyId]
				,s.[EbankingAmount] = val.[EbankingAmount]
				,s.[EbankingAccountId] = val.[EbankingAccountId]
				,s.[DirectBanking1CurrencyId] = val.[DirectBanking1CurrencyId]
				,s.[DirectBanking1Amount] = val.[DirectBanking1Amount]
				,s.[DirectBanking1AccountId] = val.[DirectBanking1AccountId]
				,s.[DirectBanking2CurrencyId] = val.[DirectBanking2CurrencyId]
				,s.[DirectBanking2Amount] = val.[DirectBanking2Amount]
				,s.[DirectBanking2AccountId] = val.[DirectBanking2AccountId]
				,s.[CashCurrencyId] = val.[CashCurrencyId]
				,s.[CashAmount] = val.[CashAmount]
				,s.[CashAccountId] = val.[CashAccountId]
				,s.[AutoCalculateOn] = val.[AutoCalculateOn]
				,s.[AdditionAmount] = val.[AdditionAmount]
				,s.[DeductionAmount] = val.[DeductionAmount]
				,s.[Status] = val.[Status]
	WHEN NOT MATCHED
		THEN
			INSERT (
				[PayrollId]
				,[CurrencyId]
				,[Salary]
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
				,[AdditionAmount]
				,[DeductionAmount]
				,[Status]
				)
			VALUES (
				val.PayrollId
				,val.[CurrencyId]
				,val.[Salary]
				,val.[EbankingCurrencyId]
				,val.[EbankingAmount]
				,val.[EbankingAccountId]
				,val.[DirectBanking1CurrencyId]
				,val.[DirectBanking1Amount]
				,val.[DirectBanking1AccountId]
				,val.[DirectBanking2CurrencyId]
				,val.[DirectBanking2Amount]
				,val.[DirectBanking2AccountId]
				,val.[CashCurrencyId]
				,val.[CashAmount]
				,val.[CashAccountId]
				,val.[AutoCalculateOn]
				,val.[AdditionAmount]
				,val.[DeductionAmount]
				,val.[Status]
				)
	OUTPUT DELETED.PayrollId
		,DELETED.[CurrencyId]
		,DELETED.[Salary]
		,DELETED.[EbankingCurrencyId]
		,DELETED.[EbankingAmount]
		,DELETED.[EbankingAccountId]
		,DELETED.[DirectBanking1CurrencyId]
		,DELETED.[DirectBanking1Amount]
		,DELETED.[DirectBanking1AccountId]
		,DELETED.[DirectBanking2CurrencyId]
		,DELETED.[DirectBanking2Amount]
		,DELETED.[DirectBanking2AccountId]
		,DELETED.[CashCurrencyId]
		,DELETED.[CashAmount]
		,DELETED.[CashAccountId]
		,DELETED.[AutoCalculateOn]
		,DELETED.[AdditionAmount]
		,DELETED.[DeductionAmount]
		,DELETED.[Status]
	INTO #tblOldLogData([PayrollId], [CurrencyId], [Salary], [EbankingCurrencyId], [EbankingAmount], [EbankingAccountId], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking1AccountId], [DirectBanking2CurrencyId], [DirectBanking2Amount], [DirectBanking2AccountId], [CashCurrencyId], [CashAmount], [CashAccountId], [AutoCalculateOn], [AdditionAmount], [DeductionAmount], [Status]);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlySalary

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pi'
					,o.[CurrencyId] AS '@ci'
					,o.[Salary] AS '@s'
					,o.EbankingCurrencyId AS '@eci'
					,o.EbankingAmount AS '@ea'
					,o.[EbankingAccountId] AS '@eai'
					,o.DirectBanking1CurrencyId AS '@d1ci'
					,o.DirectBanking1Amount AS '@d1a'
					,o.[DirectBanking1AccountId] AS '@d1ai'
					,o.DirectBanking2CurrencyId AS '@d2ci'
					,o.DirectBanking2Amount AS '@d2a'
					,o.[DirectBanking2AccountId] AS '@d2ai'
					,o.CashCurrencyId AS '@cci'
					,o.CashAmount AS '@ca'
					,o.[CashAccountId] AS '@cai'
					,o.AutoCalculateOn AS '@aco'
					,o.[AdditionAmount] AS '@aa'
					,o.[DeductionAmount] AS '@da'
					,o.[Status] AS '@st'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlySalary AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		------
		SET @xml = (
				SELECT t.PayrollId AS '@pi'
					,t.[CurrencyId] AS '@ci'
					,t.[Salary] AS '@s'
					,t.EbankingCurrencyId AS '@eci'
					,t.EbankingAmount AS '@ea'
					,t.[EbankingAccountId] AS '@eai'
					,t.DirectBanking1CurrencyId AS '@d1ci'
					,t.DirectBanking1Amount AS '@d1a'
					,t.[DirectBanking1AccountId] AS '@d1ai'
					,t.DirectBanking2CurrencyId AS '@d2ci'
					,t.DirectBanking2Amount AS '@d2a'
					,t.[DirectBanking2AccountId] AS '@d2ai'
					,t.CashCurrencyId AS '@cci'
					,t.CashAmount AS '@ca'
					,t.[CashAccountId] AS '@cai'
					,t.AutoCalculateOn AS '@aco'
					,t.[AdditionAmount] AS '@aa'
					,t.[DeductionAmount] AS '@da'
					,t.[Status] AS '@st'
				FROM #tblTempMonthlySalary AS t WITH (NOLOCK)
				WHERE RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlySalary AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlySalary'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlySalary
END