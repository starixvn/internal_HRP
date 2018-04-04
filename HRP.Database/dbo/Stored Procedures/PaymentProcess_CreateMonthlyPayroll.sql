
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlyPayroll] @Month DATE
	,@PayrollXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180321@Raymond.Le
Task: Create Monthly Payroll
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

	CREATE TABLE #tblTempMonthlyPayroll (
		RowNum INT NOT NULL identity(1, 1) PRIMARY KEY
		,EmployeeId INT
		,[Month] DATE
		,SalaryEffectiveDate DATE
		,SalaryChangeReasonId TINYINT
		,GroupStartDate DATE
		,GroupEndDate DATE
		)

	CREATE TABLE #tblOldLogData (
		EmployeeId INT
		,[Month] DATE
		,SalaryEffectiveDate DATE
		,SalaryChangeReasonId TINYINT
		,GroupStartDate DATE
		,GroupEndDate DATE
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@PayrollXMLDoc

	INSERT INTO #tblTempMonthlyPayroll (
		EmployeeId
		,[Month]
		,SalaryEffectiveDate
		,SalaryChangeReasonId
		,GroupStartDate
		,GroupEndDate
		)
	SELECT Data.EmployeeId
		,Data.[Month]
		,NULLIF(ISNULL(Data.SalaryEffectiveDate, ''), '')
		,NULLIF(ISNULL(Data.SalaryChangeReasonId, ''), '')
		,Data.GroupStartDate
		,NULLIF(ISNULL(Data.GroupEndDate, ''), '')
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[Month] DATE
			,SalaryEffectiveDate NVARCHAR(50)
			,SalaryChangeReasonId NVARCHAR(50)
			,GroupStartDate DATE
			,GroupEndDate NVARCHAR(50)
			) AS Data

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlyPayroll AS mp
	USING (
		SELECT EmployeeId
			,[Month]
			,SalaryEffectiveDate
			,SalaryChangeReasonId
			,GroupStartDate
			,GroupEndDate
		FROM #tblTempMonthlyPayroll WITH (NOLOCK)
		) AS val(EmployeeId, [Month], SalaryEffectiveDate, SalaryChangeReasonId, GroupStartDate, GroupEndDate)
		ON mp.EmployeeId = val.EmployeeId
			AND mp.[Month] = val.[Month]
	WHEN MATCHED
		THEN
			UPDATE
			SET mp.SalaryEffectiveDate = val.SalaryEffectiveDate
				,mp.SalaryChangeReasonId = val.SalaryChangeReasonId
				,mp.GroupStartDate = val.GroupStartDate
				,mp.GroupEndDate = val.GroupEndDate
	WHEN NOT MATCHED
		THEN
			INSERT (
				EmployeeId
				,[Month]
				,SalaryEffectiveDate
				,SalaryChangeReasonId
				,GroupStartDate
				,GroupEndDate
				)
			VALUES (
				val.EmployeeId
				,val.[Month]
				,val.SalaryEffectiveDate
				,val.SalaryChangeReasonId
				,val.GroupStartDate
				,val.GroupEndDate
				)
	OUTPUT DELETED.EmployeeId
		,DELETED.[Month]
		,DELETED.SalaryEffectiveDate
		,DELETED.SalaryChangeReasonId
		,DELETED.GroupStartDate
		,DELETED.GroupEndDate
	INTO #tblOldLogData(EmployeeId, [Month], SalaryEffectiveDate, SalaryChangeReasonId, GroupStartDate, GroupEndDate);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlyPayroll

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.EmployeeId AS '@eid'
					,o.[Month] AS '@m'
					,o.SalaryEffectiveDate AS '@sed'
					,o.SalaryChangeReasonId AS '@scr'
					,o.GroupStartDate AS '@gsd'
					,o.GroupEndDate AS '@ged'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlyPayroll AS t WITH (NOLOCK) ON o.EmployeeId = t.EmployeeId
					AND o.[Month] = t.[Month]
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		------
		SET @xml = (
				SELECT EmployeeId AS '@eid'
					,[Month] AS '@m'
					,SalaryEffectiveDate AS '@sed'
					,SalaryChangeReasonId AS '@scr'
					,GroupStartDate AS '@gsd'
					,GroupEndDate AS '@ged'
				FROM #tblTempMonthlyPayroll WITH (NOLOCK)
				WHERE RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = PayrollId
		FROM dbo.MonthlyPayroll AS m WITH (NOLOCK)
		INNER JOIN #tblTempMonthlyPayroll AS t WITH (NOLOCK) ON m.EmployeeId = t.EmployeeId
			AND m.[Month] = t.[Month]
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlyPayroll'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlyPayroll
END