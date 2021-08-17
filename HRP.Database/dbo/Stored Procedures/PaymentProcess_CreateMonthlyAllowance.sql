
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlyAllowance] @Month DATE
	,@AllowanceXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Allowance
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

	CREATE TABLE #tblTempMonthlyAllowance (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,[PayrollId] INT
		,[CurrencyId] INT
		,[TotalAmount] VARCHAR(MAX)
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking1AccountId] INT
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[DirectBanking2AccountId] INT
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[CashAccountId] INT
		,[DSTNetSalaryCurrencyId] INT
		,[DSTNetSalaryAmount] VARCHAR(MAX)
		,[AutoCalculateOn] VARCHAR(30)
		,[TotalNetAmount] VARCHAR(MAX)
		,[Status] TINYINT
		,[AdditionAmount] VARCHAR(MAX)
		,[DeductionAmount] VARCHAR(MAX)
		,AdditionComment NVARCHAR(MAX)
		,DeductionComment NVARCHAR(MAX)
		)

	CREATE TABLE #tblOldLogData (
		[PayrollId] INT
		,[CurrencyId] INT
		,[TotalAmount] VARCHAR(MAX)
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking1AccountId] INT
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[DirectBanking2AccountId] INT
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[CashAccountId] INT
		,[DSTNetSalaryCurrencyId] INT
		,[DSTNetSalaryAmount] VARCHAR(MAX)
		,[AutoCalculateOn] VARCHAR(30)
		,[TotalNetAmount] VARCHAR(MAX)
		,[Status] TINYINT
		,[AdditionAmount] VARCHAR(MAX)
		,[DeductionAmount] VARCHAR(MAX)
		,AdditionComment NVARCHAR(MAX)
		,DeductionComment NVARCHAR(MAX)
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@AllowanceXMLDoc

	INSERT INTO #tblTempMonthlyAllowance (
		[PayrollId]
		,[CurrencyId]
		,[TotalAmount]
		,[DirectBanking1CurrencyId]
		,[DirectBanking1Amount]
		,[DirectBanking1AccountId]
		,[DirectBanking2CurrencyId]
		,[DirectBanking2Amount]
		,[DirectBanking2AccountId]
		,[CashCurrencyId]
		,[CashAmount]
		,[CashAccountId]
		,[DSTNetSalaryCurrencyId]
		,[DSTNetSalaryAmount]
		,[AutoCalculateOn]
		,[TotalNetAmount]
		,[Status]
		,[AdditionAmount]
		,[DeductionAmount]
		,AdditionComment
		,DeductionComment
		)
	SELECT mp.PayrollId
		,NULLIF(ISNULL(Data.[CurrencyId], ''), '')
		,Data.[TotalAmount]
		,NULLIF(ISNULL(Data.[DirectBanking1CurrencyId], ''), '')
		,Data.[DirectBanking1Amount]
		,NULLIF(ISNULL(Data.[DirectBanking1AccountId], ''), '')
		,NULLIF(ISNULL(Data.[DirectBanking2CurrencyId], ''), '')
		,Data.[DirectBanking2Amount]
		,NULLIF(ISNULL(Data.[DirectBanking2AccountId], ''), '')
		,NULLIF(ISNULL(Data.[CashCurrencyId], ''), '')
		,Data.[CashAmount]
		,NULLIF(ISNULL(Data.[CashAccountId], ''), '')
		,NULLIF(ISNULL(Data.[DSTNetSalaryCurrencyId], ''), '')
		,Data.[DSTNetSalaryAmount]
		,Data.[AutoCalculateOn]
		,Data.[TotalNetAmount]
		,Data.[Status]
		,Data.[AdditionAmount]
		,Data.[DeductionAmount]
		,Data.AdditionComment
		,Data.DeductionComment
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[CurrencyId] VARCHAR(50)
			,[TotalAmount] VARCHAR(MAX)
			,[DirectBanking1CurrencyId] VARCHAR(50)
			,[DirectBanking1Amount] VARCHAR(MAX)
			,[DirectBanking1AccountId] VARCHAR(50)
			,[DirectBanking2CurrencyId] VARCHAR(50)
			,[DirectBanking2Amount] VARCHAR(MAX)
			,[DirectBanking2AccountId] VARCHAR(50)
			,[CashCurrencyId] VARCHAR(50)
			,[CashAmount] VARCHAR(MAX)
			,[CashAccountId] VARCHAR(50)
			,[DSTNetSalaryCurrencyId] VARCHAR(50)
			,[DSTNetSalaryAmount] VARCHAR(MAX)
			,[AutoCalculateOn] VARCHAR(30)
			,[TotalNetAmount] VARCHAR(MAX)
			,[Status] TINYINT
			,[AdditionAmount] VARCHAR(MAX)
			,[DeductionAmount] VARCHAR(MAX)
			,AdditionComment NVARCHAR(MAX)
			,DeductionComment NVARCHAR(MAX)
			) AS Data
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlyAllowance AS a
	USING (
		SELECT [PayrollId]
			,[CurrencyId]
			,[TotalAmount]
			,[DirectBanking1CurrencyId]
			,[DirectBanking1Amount]
			,[DirectBanking1AccountId]
			,[DirectBanking2CurrencyId]
			,[DirectBanking2Amount]
			,[DirectBanking2AccountId]
			,[CashCurrencyId]
			,[CashAmount]
			,[CashAccountId]
			,[DSTNetSalaryCurrencyId]
			,[DSTNetSalaryAmount]
			,[AutoCalculateOn]
			,[TotalNetAmount]
			,[Status]
			,[AdditionAmount]
			,[DeductionAmount]
			,AdditionComment
			,DeductionComment
		FROM #tblTempMonthlyAllowance WITH (NOLOCK)
		) AS val([PayrollId], [CurrencyId], [TotalAmount], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking1AccountId]
			,[DirectBanking2CurrencyId], [DirectBanking2Amount], [DirectBanking2AccountId]
			,[CashCurrencyId], [CashAmount], [CashAccountId]
			,[DSTNetSalaryCurrencyId], [DSTNetSalaryAmount], [AutoCalculateOn], [TotalNetAmount], [Status], [AdditionAmount], [DeductionAmount], AdditionComment ,DeductionComment)
		ON a.PayrollId = val.PayrollId
	WHEN MATCHED
		THEN
			UPDATE
			SET a.CurrencyId = val.CurrencyId
				,a.[TotalAmount] = val.[TotalAmount]
				,a.DirectBanking1CurrencyId = val.DirectBanking1CurrencyId
				,a.DirectBanking1Amount = val.DirectBanking1Amount
				,a.[DirectBanking1AccountId] = val.[DirectBanking1AccountId]
				,a.DirectBanking2CurrencyId = val.DirectBanking2CurrencyId
				,a.DirectBanking2Amount = val.DirectBanking2Amount
				,a.[DirectBanking2AccountId] = val.[DirectBanking2AccountId]
				,a.CashCurrencyId = val.CashCurrencyId
				,a.CashAmount = val.CashAmount
				,a.[CashAccountId] = val.[CashAccountId]
				,a.[DSTNetSalaryCurrencyId] = val.[DSTNetSalaryCurrencyId]
				,a.[DSTNetSalaryAmount] = val.[DSTNetSalaryAmount]
				,a.AutoCalculateOn = val.AutoCalculateOn
				,a.[TotalNetAmount] = val.[TotalNetAmount]
				,a.[Status] = val.[Status]
				,a.[AdditionAmount] = val.[AdditionAmount]
				,a.[DeductionAmount] = val.[DeductionAmount]
				,a.AdditionComment = val.AdditionComment
				,a.DeductionComment = val.DeductionComment
	WHEN NOT MATCHED
		THEN
			INSERT (
				[PayrollId]
				,[CurrencyId]
				,[TotalAmount]
				,[DirectBanking1CurrencyId]
				,[DirectBanking1Amount]
				,[DirectBanking1AccountId]
				,[DirectBanking2CurrencyId]
				,[DirectBanking2Amount]
				,[DirectBanking2AccountId]
				,[CashCurrencyId]
				,[CashAmount]
				,[CashAccountId]
				,[DSTNetSalaryCurrencyId]
				,[DSTNetSalaryAmount]
				,[AutoCalculateOn]
				,[TotalNetAmount]
				,[Status]
				,[AdditionAmount]
				,[DeductionAmount]
				,AdditionComment
				,DeductionComment
				)
			VALUES (
				val.PayrollId
				,val.[CurrencyId]
				,val.[TotalAmount]
				,val.[DirectBanking1CurrencyId]
				,val.[DirectBanking1Amount]
				,val.[DirectBanking1AccountId]
				,val.[DirectBanking2CurrencyId]
				,val.[DirectBanking2Amount]
				,val.[DirectBanking2AccountId]
				,val.[CashCurrencyId]
				,val.[CashAmount]
				,val.[CashAccountId]
				,val.[DSTNetSalaryCurrencyId]
				,val.[DSTNetSalaryAmount]
				,val.[AutoCalculateOn]
				,val.[TotalNetAmount]
				,val.[Status]
				,val.[AdditionAmount]
				,val.[DeductionAmount]
				,val.AdditionComment
				,val.DeductionComment
				)
	OUTPUT DELETED.PayrollId
		,DELETED.[CurrencyId]
		,DELETED.[TotalAmount]
		,DELETED.[DirectBanking1CurrencyId]
		,DELETED.[DirectBanking1Amount]
		,DELETED.[DirectBanking1AccountId]
		,DELETED.[DirectBanking2CurrencyId]
		,DELETED.[DirectBanking2Amount]
		,DELETED.[DirectBanking2AccountId]
		,DELETED.[CashCurrencyId]
		,DELETED.[CashAmount]
		,DELETED.[CashAccountId]
		,DELETED.[DSTNetSalaryCurrencyId]
		,DELETED.[DSTNetSalaryAmount]
		,DELETED.[AutoCalculateOn]
		,DELETED.[TotalNetAmount]
		,DELETED.[Status]
		,DELETED.[AdditionAmount]
		,DELETED.[DeductionAmount]
		,DELETED.AdditionComment
		,DELETED.DeductionComment
	INTO #tblOldLogData([PayrollId], [CurrencyId], [TotalAmount], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking1AccountId]
		,[DirectBanking2CurrencyId], [DirectBanking2Amount], [DirectBanking2AccountId]
		,[CashCurrencyId], [CashAmount], [CashAccountId]
		,[DSTNetSalaryCurrencyId], [DSTNetSalaryAmount], [AutoCalculateOn], [TotalNetAmount], [Status], [AdditionAmount], [DeductionAmount], AdditionComment, DeductionComment);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlyAllowance

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pid'
					,o.[CurrencyId] AS '@cid'
					,o.[TotalAmount] AS '@ta'
					,o.[DirectBanking1CurrencyId] AS '@d1ci'
					,o.[DirectBanking1Amount] AS '@d1a'
					,o.[DirectBanking1AccountId] AS '@d1ai'
					,o.[DirectBanking2CurrencyId] AS '@d2ci'
					,o.[DirectBanking2Amount] AS '@d2a'
					,o.[DirectBanking2AccountId] AS '@d2ai'
					,o.[CashCurrencyId] AS '@cci'
					,o.[CashAmount] AS '@ca'
					,o.[CashAccountId] AS '@cai'
					,o.[DSTNetSalaryCurrencyId] AS '@dsci'
					,o.[DSTNetSalaryAmount] AS '@dsa'
					,o.[AutoCalculateOn] AS '@aco'
					,o.[TotalNetAmount] AS '@tna'
					,o.[Status] AS '@s'
					,o.[AdditionAmount] AS '@aa'
					,o.[DeductionAmount] AS '@da'
					,o.AdditionComment AS '@acomment'
					,o.DeductionComment AS '@dcomment'
				FROM #tblOldLogData AS o WITH (NOLOCK)
					INNER JOIN #tblTempMonthlyAllowance AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		------
		SET @xml = (
				SELECT t.PayrollId AS '@pid'
					,t.[CurrencyId] AS '@cid'
					,t.[TotalAmount] AS '@ta'
					,t.[DirectBanking1CurrencyId] AS '@d1ci'
					,t.[DirectBanking1Amount] AS '@d1a'
					,t.[DirectBanking1AccountId] AS '@d1ai'
					,t.[DirectBanking2CurrencyId] AS '@d2ci'
					,t.[DirectBanking2Amount] AS '@d2a'
					,t.[DirectBanking2AccountId] AS '@d2ai'
					,t.[CashCurrencyId] AS '@cci'
					,t.[CashAmount] AS '@ca'
					,t.[CashAccountId] AS '@cai'
					,t.[DSTNetSalaryCurrencyId] AS '@dsci'
					,t.[DSTNetSalaryAmount] AS '@dsa'
					,t.[AutoCalculateOn] AS '@aco'
					,t.[TotalNetAmount] AS '@tna'
					,t.[Status] AS '@s'
					,t.[AdditionAmount] AS '@aa'
					,t.[DeductionAmount] AS '@da'
					,t.AdditionComment AS '@acomment'
					,t.DeductionComment AS '@dcomment'
				FROM #tblTempMonthlyAllowance AS t WITH (NOLOCK)
				WHERE RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlyAllowance AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlyAllowance'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlyAllowance
END