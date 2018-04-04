
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlyAllowanceDeduction] @Month DATE
	,@AllowanceDeductionXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Allowance Deduction
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

	CREATE TABLE #tblTempMonthlyAllowanceDeduction (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,[PayrollId] INT
		,[Type] TINYINT
		,[UnitAmount] VARCHAR(MAX)
		,[NoOfDays] DECIMAL(6, 2)
		,[TotalAmount] VARCHAR(MAX)
		,[NetAllowance] VARCHAR(MAX)
		,[CurrencyId] TINYINT
		)

	CREATE TABLE #tblOldLogData (
		[PayrollId] INT
		,[Type] TINYINT
		,[UnitAmount] VARCHAR(MAX)
		,[NoOfDays] DECIMAL(6, 2)
		,[TotalAmount] VARCHAR(MAX)
		,[NetAllowance] VARCHAR(MAX)
		,[CurrencyId] TINYINT
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@AllowanceDeductionXMLDoc

	INSERT INTO #tblTempMonthlyAllowanceDeduction (
		[PayrollId]
		,[Type]
		,[NoOfDays]
		,[UnitAmount]
		,[TotalAmount]
		,[NetAllowance]
		,[CurrencyId]
		)
	SELECT mp.PayrollId
		,Data.[Type]
		,CASE 
			WHEN ISNUMERIC(Data.[NoOfDays]) = 1
				THEN CAST(Data.[NoOfDays] AS DECIMAL(6, 2))
			ELSE NULL
			END
		,Data.[UnitAmount]
		,Data.[TotalAmount]
		,Data.[NetAllowance]
		,NULLIF(ISNULL(Data.[CurrencyId], ''), '')
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[Type] TINYINT
			,[UnitAmount] VARCHAR(MAX)
			,[NoOfDays] VARCHAR(50)
			,[TotalAmount] VARCHAR(MAX)
			,[NetAllowance] VARCHAR(MAX)
			,[CurrencyId] VARCHAR(50)
			) AS Data
	LEFT JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlyAllowanceDeduction AS a
	USING (
		SELECT [PayrollId]
			,[Type]
			,[UnitAmount]
			,[NoOfDays]
			,[TotalAmount]
			,[NetAllowance]
			,[CurrencyId]
		FROM #tblTempMonthlyAllowanceDeduction WITH (NOLOCK)
		) AS val([PayrollId], [Type], [UnitAmount], [NoOfDays], [TotalAmount], [NetAllowance], [CurrencyId])
		ON a.PayrollId = val.PayrollId
			AND a.[Type] = val.[Type]
	WHEN MATCHED
		THEN
			UPDATE
			SET a.[UnitAmount] = val.[UnitAmount]
				,a.[NoOfDays] = val.[NoOfDays]
				,a.[TotalAmount] = val.[TotalAmount]
				,a.[NetAllowance] = val.[NetAllowance]
				,a.[CurrencyId] = val.[CurrencyId]
	WHEN NOT MATCHED
		THEN
			INSERT (
				[PayrollId]
				,[Type]
				,[UnitAmount]
				,[NoOfDays]
				,[TotalAmount]
				,[NetAllowance]
				,[CurrencyId]
				)
			VALUES (
				val.PayrollId
				,val.[Type]
				,val.[UnitAmount]
				,val.[NoOfDays]
				,val.[TotalAmount]
				,val.[NetAllowance]
				,val.[CurrencyId]
				)
	OUTPUT DELETED.PayrollId
		,DELETED.[Type]
		,DELETED.[UnitAmount]
		,DELETED.[NoOfDays]
		,DELETED.[TotalAmount]
		,DELETED.[NetAllowance]
		,DELETED.[CurrencyId]
	INTO #tblOldLogData([PayrollId], [Type], [UnitAmount], [NoOfDays], [TotalAmount], [NetAllowance], [CurrencyId]);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlyAllowanceDeduction

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pi'
					,o.[Type] AS '@t'
					,o.[UnitAmount] AS '@ua'
					,o.[NoOfDays] AS '@nod'
					,o.[TotalAmount] AS '@ta'
					,o.[NetAllowance] AS '@na'
					,o.[CurrencyId] AS '@ci'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlyAllowanceDeduction AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
					AND o.[Type] = t.[Type]
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		------
		SET @xml = (
				SELECT t.PayrollId AS '@pi'
					,t.[Type] AS '@t'
					,t.[UnitAmount] AS '@ua'
					,t.[NoOfDays] AS '@nod'
					,t.[TotalAmount] AS '@ta'
					,t.[NetAllowance] AS '@na'
					,t.[CurrencyId] AS '@ci'
				FROM #tblTempMonthlyAllowanceDeduction AS t WITH (NOLOCK)
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlyAllowanceDeduction AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlyAllowanceDeduction'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlyAllowanceDeduction
END