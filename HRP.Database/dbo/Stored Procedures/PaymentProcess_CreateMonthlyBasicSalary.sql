
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlyBasicSalary] @Month DATE
	,@BasicSalaryXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Basic Salary
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

	CREATE TABLE #tblTempMonthlyBasicSalary (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,PayrollId INT
		,OldCurrencyId INT
		,OldSalary VARCHAR(MAX)
		,CurrencyId INT
		,Salary VARCHAR(MAX)
		,WorkingDays TINYINT
		,EbankingCurrencyId INT
		,EbankingAmount VARCHAR(MAX)
		,DirectBanking1CurrencyId INT
		,DirectBanking1Amount VARCHAR(MAX)
		,DirectBanking2CurrencyId INT
		,DirectBanking2Amount VARCHAR(MAX)
		,CashCurrencyId INT
		,CashAmount VARCHAR(MAX)
		,AutoCalculateOn VARCHAR(30)
		,AfterAdjustmentAmount VARCHAR(MAX)
		,AfterAdjustmentReason NVARCHAR(200)
		,CreatedBy INT
		,CreatedDate DATETIME
	)

	CREATE TABLE #tblOldLogData (
		PayrollId INT
		,OldCurrencyId INT
		,OldSalary VARCHAR(MAX)
		,CurrencyId INT
		,Salary VARCHAR(MAX)
		,WorkingDays TINYINT
		,EbankingCurrencyId INT
		,EbankingAmount VARCHAR(MAX)
		,DirectBanking1CurrencyId INT
		,DirectBanking1Amount VARCHAR(MAX)
		,DirectBanking2CurrencyId INT
		,DirectBanking2Amount VARCHAR(MAX)
		,CashCurrencyId INT
		,CashAmount VARCHAR(MAX)
		,AutoCalculateOn VARCHAR(30)
		,AfterAdjustmentAmount VARCHAR(MAX)
		,AfterAdjustmentReason NVARCHAR(200)
		,CreatedBy INT
		,CreatedDate DATETIME
	)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@BasicSalaryXMLDoc

	INSERT INTO #tblTempMonthlyBasicSalary (
		PayrollId
		,OldCurrencyId
		,OldSalary
		,CurrencyId
		,Salary
		,WorkingDays
		,EbankingCurrencyId
		,EbankingAmount
		,DirectBanking1CurrencyId
		,DirectBanking1Amount
		,DirectBanking2CurrencyId
		,DirectBanking2Amount
		,CashCurrencyId
		,CashAmount
		,AutoCalculateOn
		,AfterAdjustmentAmount
		,AfterAdjustmentReason
		,CreatedBy
		,CreatedDate
		)
	SELECT mp.PayrollId
		,NULLIF(ISNULL(Data.OldCurrencyId, ''), '')
		,Data.OldSalary
		,NULLIF(ISNULL(Data.CurrencyId, ''), '')
		,Data.Salary
		,Data.WorkingDays
		,NULLIF(ISNULL(Data.EbankingCurrencyId, ''), '')
		,Data.EbankingAmount
		,NULLIF(ISNULL(Data.DirectBanking1CurrencyId, ''), '')
		,Data.DirectBanking1Amount
		,NULLIF(ISNULL(Data.DirectBanking2CurrencyId, ''), '')
		,Data.DirectBanking2Amount
		,NULLIF(ISNULL(Data.CashCurrencyId, ''), '')
		,Data.CashAmount
		,Data.AutoCalculateOn
		,Data.AfterAdjustmentAmount
		,Data.AfterAdjustmentReason
		,@UpdatedBy
		,GETDATE()
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,OldCurrencyId VARCHAR(50)
			,OldSalary VARCHAR(MAX)
			,CurrencyId VARCHAR(50)
			,Salary VARCHAR(MAX)
			,WorkingDays TINYINT
			,EbankingCurrencyId VARCHAR(50)
			,EbankingAmount VARCHAR(MAX)
			,DirectBanking1CurrencyId VARCHAR(50)
			,DirectBanking1Amount VARCHAR(MAX)
			,DirectBanking2CurrencyId VARCHAR(50)
			,DirectBanking2Amount VARCHAR(MAX)
			,CashCurrencyId VARCHAR(50)
			,CashAmount VARCHAR(MAX)
			,AutoCalculateOn VARCHAR(30)
			,AfterAdjustmentAmount VARCHAR(MAX)
			,AfterAdjustmentReason NVARCHAR(200)
			) AS Data
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlyBasicSalary AS bs
	USING (
		SELECT PayrollId
			,OldCurrencyId
			,OldSalary
			,CurrencyId
			,Salary
			,WorkingDays
			,EbankingCurrencyId
			,EbankingAmount
			,DirectBanking1CurrencyId
			,DirectBanking1Amount
			,DirectBanking2CurrencyId
			,DirectBanking2Amount
			,CashCurrencyId
			,CashAmount
			,AutoCalculateOn
			,AfterAdjustmentAmount
			,AfterAdjustmentReason
			,CreatedBy
			,CreatedDate
		FROM #tblTempMonthlyBasicSalary WITH (NOLOCK)
		) AS val(PayrollId, OldCurrencyId, OldSalary, CurrencyId, Salary, WorkingDays, EbankingCurrencyId, EbankingAmount, DirectBanking1CurrencyId, DirectBanking1Amount, DirectBanking2CurrencyId, DirectBanking2Amount, CashCurrencyId, CashAmount, AutoCalculateOn, AfterAdjustmentAmount, AfterAdjustmentReason, CreatedBy, CreatedDate)
		ON bs.PayrollId = val.PayrollId
	WHEN MATCHED
		THEN
			UPDATE
			SET bs.OldCurrencyId = val.OldCurrencyId
				,bs.OldSalary = val.OldSalary
				,bs.CurrencyId = val.CurrencyId
				,bs.Salary = val.Salary
				,bs.WorkingDays = val.WorkingDays
				,bs.EbankingCurrencyId = val.EbankingCurrencyId
				,bs.EbankingAmount = val.EbankingAmount
				,bs.DirectBanking1CurrencyId = val.DirectBanking1CurrencyId
				,bs.DirectBanking1Amount = val.DirectBanking1Amount
				,bs.DirectBanking2CurrencyId = val.DirectBanking2CurrencyId
				,bs.DirectBanking2Amount = val.DirectBanking2Amount
				,bs.CashCurrencyId = val.CashCurrencyId
				,bs.CashAmount = val.CashAmount
				,bs.AutoCalculateOn = val.AutoCalculateOn
				,bs.AfterAdjustmentAmount = val.AfterAdjustmentAmount
				,bs.AfterAdjustmentReason = val.AfterAdjustmentReason
				,bs.CreatedBy = val.CreatedBy
				,bs.CreatedDate = val.CreatedDate
	WHEN NOT MATCHED
		THEN
			INSERT (
				PayrollId
			,OldCurrencyId
			,OldSalary
			,CurrencyId
			,Salary
			,WorkingDays
			,EbankingCurrencyId
			,EbankingAmount
			,DirectBanking1CurrencyId
			,DirectBanking1Amount
			,DirectBanking2CurrencyId
			,DirectBanking2Amount
			,CashCurrencyId
			,CashAmount
			,AutoCalculateOn
			,AfterAdjustmentAmount
			,AfterAdjustmentReason
			,CreatedBy
			,CreatedDate
				)
			VALUES (
			val.PayrollId
			,val.OldCurrencyId
			,val.OldSalary
			,val.CurrencyId
			,val.Salary
			,val.WorkingDays
			,val.EbankingCurrencyId
			,val.EbankingAmount
			,val.DirectBanking1CurrencyId
			,val.DirectBanking1Amount
			,val.DirectBanking2CurrencyId
			,val.DirectBanking2Amount
			,val.CashCurrencyId
			,val.CashAmount
			,val.AutoCalculateOn
			,val.AfterAdjustmentAmount
			,val.AfterAdjustmentReason
			,val.CreatedBy
			,val.CreatedDate
				)
	OUTPUT 	DELETED.PayrollId
			,DELETED.OldCurrencyId
			,DELETED.OldSalary
			,DELETED.CurrencyId
			,DELETED.Salary
			,DELETED.WorkingDays
			,DELETED.EbankingCurrencyId
			,DELETED.EbankingAmount
			,DELETED.DirectBanking1CurrencyId
			,DELETED.DirectBanking1Amount
			,DELETED.DirectBanking2CurrencyId
			,DELETED.DirectBanking2Amount
			,DELETED.CashCurrencyId
			,DELETED.CashAmount
			,DELETED.AutoCalculateOn
			,DELETED.AfterAdjustmentAmount
			,DELETED.AfterAdjustmentReason
			,DELETED.CreatedBy
			,DELETED.CreatedDate
	INTO #tblOldLogData(PayrollId, OldCurrencyId, OldSalary, CurrencyId, Salary, WorkingDays, EbankingCurrencyId, EbankingAmount, DirectBanking1CurrencyId, DirectBanking1Amount, DirectBanking2CurrencyId, DirectBanking2Amount, CashCurrencyId, CashAmount, AutoCalculateOn, AfterAdjustmentAmount, AfterAdjustmentReason, CreatedBy, CreatedDate);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlyBasicSalary

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pi'
					,o.OldCurrencyId AS '@oci'
					,o.OldSalary AS '@os'
					,o.CurrencyId AS '@ci'
					,o.Salary AS '@s'
					,o.WorkingDays AS '@wd'
					,o.EbankingCurrencyId AS '@eci'
					,o.EbankingAmount AS '@ea'
					,o.DirectBanking1CurrencyId AS '@d1ci'
					,o.DirectBanking1Amount AS '@d1a'
					,o.DirectBanking2CurrencyId AS '@d2ci'
					,o.DirectBanking2Amount AS '@d2a'
					,o.CashCurrencyId AS '@cci'
					,o.CashAmount AS '@ca'
					,o.AutoCalculateOn AS '@aco'
					,o.AfterAdjustmentAmount AS '@aaa'
					,o.AfterAdjustmentReason AS '@aar'
					,o.CreatedBy AS '@cb'
					,o.CreatedDate AS '@cd'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlyBasicSalary AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
				WHERE t.RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
		------
		SET @xml = (
				SELECT t.PayrollId AS '@pi'
					,t.OldCurrencyId AS '@oci'
					,t.OldSalary AS '@os'
					,t.CurrencyId AS '@ci'
					,t.Salary AS '@s'
					,t.WorkingDays AS '@wd'
					,t.EbankingCurrencyId AS '@eci'
					,t.EbankingAmount AS '@ea'
					,t.DirectBanking1CurrencyId AS '@d1ci'
					,t.DirectBanking1Amount AS '@d1a'
					,t.DirectBanking2CurrencyId AS '@d2ci'
					,t.DirectBanking2Amount AS '@d2a'
					,t.CashCurrencyId AS '@cci'
					,t.CashAmount AS '@ca'
					,t.AutoCalculateOn AS '@aco'
					,t.AfterAdjustmentAmount AS '@aaa'
					,t.AfterAdjustmentReason AS '@aar'
					,t.CreatedBy AS '@cb'
					,t.CreatedDate AS '@cd'
				FROM #tblTempMonthlyBasicSalary AS t WITH (NOLOCK)
				WHERE RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlyBasicSalary AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlyBasicSalary'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlyBasicSalary
END