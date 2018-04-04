
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlySalaryDeduction] @Month DATE
	,@SalaryDeductionXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Salary Deduction
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

	CREATE TABLE #tblTempMonthlySalaryDeduction (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,[PayrollId] INT
		,[Type] TINYINT
		,[UnitAmount] VARCHAR(MAX)
		,[NoOfDays] DECIMAL(8, 2)
		,[NetSalary] VARCHAR(MAX)
		,[Description] NVARCHAR(1000)
		)

	CREATE TABLE #tblOldLogData (
		[PayrollId] INT
		,[Type] TINYINT
		,[UnitAmount] VARCHAR(MAX)
		,[NoOfDays] DECIMAL(8, 2)
		,[NetSalary] VARCHAR(MAX)
		,[Description] NVARCHAR(1000)
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@SalaryDeductionXMLDoc

	INSERT INTO #tblTempMonthlySalaryDeduction (
		[PayrollId]
		,[Type]
		,[UnitAmount]
		,[NoOfDays]
		,[NetSalary]
		,[Description]
		)
	SELECT mp.PayrollId
		,Data.[Type]
		,Data.[UnitAmount]
		,CASE 
			WHEN ISNUMERIC(Data.[NoOfDays]) = 1
				THEN CAST(Data.[NoOfDays] AS DECIMAL(8, 2))
			ELSE NULL
			END
		,Data.[NetSalary]
		,Data.[Description]
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[Type] TINYINT
			,[UnitAmount] VARCHAR(MAX)
			,[NoOfDays] VARCHAR(50)
			,[NetSalary] VARCHAR(MAX)
			,[Description] NVARCHAR(1000)
			) AS Data
	LEFT JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlySalaryDeduction AS s
	USING (
		SELECT [PayrollId]
			,[Type]
			,[UnitAmount]
			,[NoOfDays]
			,[NetSalary]
			,[Description]
		FROM #tblTempMonthlySalaryDeduction WITH (NOLOCK)
		) AS val([PayrollId], [Type], [UnitAmount], [NoOfDays], [NetSalary], [Description])
		ON s.PayrollId = val.PayrollId
			AND s.[Type] = val.[Type]
	WHEN MATCHED
		THEN
			UPDATE
			SET s.[UnitAmount] = val.[UnitAmount]
				,s.[NoOfDays] = val.[NoOfDays]
				,s.[NetSalary] = val.[NetSalary]
				,s.[Description] = val.[Description]
	WHEN NOT MATCHED
		THEN
			INSERT (
				[PayrollId]
				,[Type]
				,[UnitAmount]
				,[NoOfDays]
				,[NetSalary]
				,[Description]
				)
			VALUES (
				val.PayrollId
				,val.[Type]
				,val.[UnitAmount]
				,val.[NoOfDays]
				,val.[NetSalary]
				,val.[Description]
				)
	OUTPUT DELETED.PayrollId
		,DELETED.[Type]
		,DELETED.[UnitAmount]
		,DELETED.[NoOfDays]
		,DELETED.[NetSalary]
		,DELETED.[Description]
	INTO #tblOldLogData([PayrollId], [Type], [UnitAmount], [NoOfDays], [NetSalary], [Description]);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlySalaryDeduction

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pi'
					,o.[Type] AS '@t'
					,o.[UnitAmount] AS '@ua'
					,o.[NoOfDays] AS '@nod'
					,o.[NetSalary] AS '@ns'
					,o.[Description] AS '@d'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlySalaryDeduction AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
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
					,t.[NetSalary] AS '@ns'
					,t.[Description] AS '@d'
				FROM #tblTempMonthlySalaryDeduction AS t WITH (NOLOCK)
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlySalaryDeduction AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlySalaryDeduction'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlySalaryDeduction
END