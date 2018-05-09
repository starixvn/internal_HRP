
CREATE PROCEDURE [dbo].[PaymentProcess_CreateMonthlyBasicAllowance] @Month DATE
	,@BasicAllowanceXMLDoc NVARCHAR(MAX)
	,@UpdatedBy INT = 0
AS
/*
Created: 20180322@Raymond.Le
Task: Create Monthly Basic Allowance
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

	CREATE TABLE #tblTempMonthlyBasicAllowance (
		RowNum INT NOT NULL IDENTITY(1, 1) PRIMARY KEY
		,[PayrollId] INT
		,[CurrencyId] INT
		,[TotalAmount] VARCHAR(MAX)
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[SubsistenceCurrencyId] INT
		,[SubsistenceAmount] VARCHAR(MAX)
		,[HomePassageCurrencyId] INT
		,[HomePassageAmount] VARCHAR(MAX)
		,[HousingCurrencyId] INT
		,[HousingAmount] VARCHAR(MAX)
		,[MobilePhoneCurrencyId] INT
		,[MobilePhoneAmount] VARCHAR(MAX)
		,[HPICurrencyId] INT
		,[HPIAmount] VARCHAR(MAX)
		,[DutyMealCurrencyId] INT
		,[DutyMealAmount] VARCHAR(MAX)
		,[SecondmentCurrencyId] INT
		,[SecondmentAmount] VARCHAR(MAX)
		,[OthersCurrencyId] INT
		,[OthersAmount] VARCHAR(MAX)
		,[RoomingChargesCurrencyId] INT
		,[RoomingChargesAmount] VARCHAR(MAX)
		,[FitnessFirstCurrencyId] INT
		,[FitnessFirstAmount] VARCHAR(MAX)
		,[AutoCalculateOn] VARCHAR(30)
		,[CreatedBy] INT
		,[CreatedDate] DATETIME
		,[AfterAdjustmentAmount] VARCHAR(MAX)
		,[Reason] VARCHAR(200)
		,[SubsistenceAfterAdjustmentAmount] VARCHAR(MAX)
		,[HousingAfterAdjustmentAmount] VARCHAR(MAX)
		,[HomePassageAfterAdjustmentAmount] VARCHAR(MAX)
		,[MobilePhoneAfterAdjustmentAmount] VARCHAR(MAX)
		,[DutyMealAfterAdjustmentAmount] VARCHAR(MAX)
		,[HPIAfterAdjustmentAmount] VARCHAR(MAX)
		,[OthersAfterAdjustmentAmount] VARCHAR(MAX)
		,[SecondmentAfterAdjustmentAmount] VARCHAR(MAX)
		)

	CREATE TABLE #tblOldLogData (
		[PayrollId] INT
		,[CurrencyId] INT
		,[TotalAmount] VARCHAR(MAX)
		,[DirectBanking1CurrencyId] INT
		,[DirectBanking1Amount] VARCHAR(MAX)
		,[DirectBanking2CurrencyId] INT
		,[DirectBanking2Amount] VARCHAR(MAX)
		,[CashCurrencyId] INT
		,[CashAmount] VARCHAR(MAX)
		,[SubsistenceCurrencyId] INT
		,[SubsistenceAmount] VARCHAR(MAX)
		,[HomePassageCurrencyId] INT
		,[HomePassageAmount] VARCHAR(MAX)
		,[HousingCurrencyId] INT
		,[HousingAmount] VARCHAR(MAX)
		,[MobilePhoneCurrencyId] INT
		,[MobilePhoneAmount] VARCHAR(MAX)
		,[HPICurrencyId] INT
		,[HPIAmount] VARCHAR(MAX)
		,[DutyMealCurrencyId] INT
		,[DutyMealAmount] VARCHAR(MAX)
		,[SecondmentCurrencyId] INT
		,[SecondmentAmount] VARCHAR(MAX)
		,[OthersCurrencyId] INT
		,[OthersAmount] VARCHAR(MAX)
		,[RoomingChargesCurrencyId] INT
		,[RoomingChargesAmount] VARCHAR(MAX)
		,[FitnessFirstCurrencyId] INT
		,[FitnessFirstAmount] VARCHAR(MAX)
		,[AutoCalculateOn] VARCHAR(30)
		,[CreatedBy] INT
		,[CreatedDate] DATETIME
		,[AfterAdjustmentAmount] VARCHAR(MAX)
		,[Reason] VARCHAR(200)
		,[SubsistenceAfterAdjustmentAmount] VARCHAR(MAX)
		,[HousingAfterAdjustmentAmount] VARCHAR(MAX)
		,[HomePassageAfterAdjustmentAmount] VARCHAR(MAX)
		,[MobilePhoneAfterAdjustmentAmount] VARCHAR(MAX)
		,[DutyMealAfterAdjustmentAmount] VARCHAR(MAX)
		,[HPIAfterAdjustmentAmount] VARCHAR(MAX)
		,[OthersAfterAdjustmentAmount] VARCHAR(MAX)
		,[SecondmentAfterAdjustmentAmount] VARCHAR(MAX)
		)

	-- Convert XMLDoc to table
	SET @startRun = GETDATE()

	EXEC sp_xml_preparedocument @iDoc OUTPUT
		,@BasicAllowanceXMLDoc

	INSERT INTO #tblTempMonthlyBasicAllowance (
		[PayrollId]
		,[CurrencyId]
		,[TotalAmount]
		,[DirectBanking1CurrencyId]
		,[DirectBanking1Amount]
		,[DirectBanking2CurrencyId]
		,[DirectBanking2Amount]
		,[CashCurrencyId]
		,[CashAmount]
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
		,[RoomingChargesCurrencyId]
		,[RoomingChargesAmount]
		,[FitnessFirstCurrencyId]
		,[FitnessFirstAmount]
		,[AutoCalculateOn]
		,[CreatedBy]
		,[CreatedDate]
		,[AfterAdjustmentAmount]
		,[Reason]
		,[SubsistenceAfterAdjustmentAmount]
		,[HousingAfterAdjustmentAmount]
		,[HomePassageAfterAdjustmentAmount]
		,[MobilePhoneAfterAdjustmentAmount]
		,[DutyMealAfterAdjustmentAmount]
		,[HPIAfterAdjustmentAmount]
		,[OthersAfterAdjustmentAmount]
		,[SecondmentAfterAdjustmentAmount]
		)
	SELECT mp.PayrollId
		,NULLIF(ISNULL(Data.[CurrencyId], ''), '')
		,Data.[TotalAmount]
		,NULLIF(ISNULL(Data.[DirectBanking1CurrencyId], ''), '')
		,Data.[DirectBanking1Amount]
		,NULLIF(ISNULL(Data.[DirectBanking2CurrencyId], ''), '')
		,Data.[DirectBanking2Amount]
		,NULLIF(ISNULL(Data.[CashCurrencyId], ''), '')
		,Data.[CashAmount]
		,NULLIF(ISNULL(Data.[SubsistenceCurrencyId], ''), '')
		,Data.[SubsistenceAmount]
		,NULLIF(ISNULL(Data.[HomePassageCurrencyId], ''), '')
		,Data.[HomePassageAmount]
		,NULLIF(ISNULL(Data.[HousingCurrencyId], ''), '')
		,Data.[HousingAmount]
		,NULLIF(ISNULL(Data.[MobilePhoneCurrencyId], ''), '')
		,Data.[MobilePhoneAmount]
		,NULLIF(ISNULL(Data.[HPICurrencyId], ''), '')
		,Data.[HPIAmount]
		,NULLIF(ISNULL(Data.[DutyMealCurrencyId], ''), '')
		,Data.[DutyMealAmount]
		,NULLIF(ISNULL(Data.[SecondmentCurrencyId], ''), '')
		,Data.[SecondmentAmount]
		,NULLIF(ISNULL(Data.[OthersCurrencyId], ''), '')
		,Data.[OthersAmount]
		,NULLIF(ISNULL(Data.[RoomingChargesCurrencyId], ''), '')
		,Data.[RoomingChargesAmount]
		,NULLIF(ISNULL(Data.[FitnessFirstCurrencyId], ''), '')
		,Data.[FitnessFirstAmount]
		,Data.[AutoCalculateOn]
		,@UpdatedBy
		,GETDATE()
		,Data.[AfterAdjustmentAmount]
		,Data.[Reason]
		,Data.[SubsistenceAfterAdjustmentAmount]
		,Data.[HousingAfterAdjustmentAmount]
		,Data.[HomePassageAfterAdjustmentAmount]
		,Data.[MobilePhoneAfterAdjustmentAmount]
		,Data.[DutyMealAfterAdjustmentAmount]
		,Data.[HPIAfterAdjustmentAmount]
		,Data.[OthersAfterAdjustmentAmount]
		,Data.[SecondmentAfterAdjustmentAmount]
	FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
			EmployeeId INT
			,[CurrencyId] VARCHAR(50)
			,[TotalAmount] VARCHAR(MAX)
			,[DirectBanking1CurrencyId] VARCHAR(50)
			,[DirectBanking1Amount] VARCHAR(MAX)
			,[DirectBanking2CurrencyId] VARCHAR(50)
			,[DirectBanking2Amount] VARCHAR(MAX)
			,[CashCurrencyId] VARCHAR(50)
			,[CashAmount] VARCHAR(MAX)
			,[SubsistenceCurrencyId] VARCHAR(50)
			,[SubsistenceAmount] VARCHAR(MAX)
			,[HomePassageCurrencyId] VARCHAR(50)
			,[HomePassageAmount] VARCHAR(MAX)
			,[HousingCurrencyId] VARCHAR(50)
			,[HousingAmount] VARCHAR(MAX)
			,[MobilePhoneCurrencyId] VARCHAR(50)
			,[MobilePhoneAmount] VARCHAR(MAX)
			,[HPICurrencyId] VARCHAR(50)
			,[HPIAmount] VARCHAR(MAX)
			,[DutyMealCurrencyId] VARCHAR(50)
			,[DutyMealAmount] VARCHAR(MAX)
			,[SecondmentCurrencyId] VARCHAR(50)
			,[SecondmentAmount] VARCHAR(MAX)
			,[OthersCurrencyId] VARCHAR(50)
			,[OthersAmount] VARCHAR(MAX)
			,[RoomingChargesCurrencyId] VARCHAR(50)
			,[RoomingChargesAmount] VARCHAR(MAX)
			,[FitnessFirstCurrencyId] VARCHAR(50)
			,[FitnessFirstAmount] VARCHAR(MAX)
			,[AutoCalculateOn] VARCHAR(30)
			,[AfterAdjustmentAmount] VARCHAR(MAX)
			,[Reason] VARCHAR(200)
			,[SubsistenceAfterAdjustmentAmount] VARCHAR(MAX)
			,[HousingAfterAdjustmentAmount] VARCHAR(MAX)
			,[HomePassageAfterAdjustmentAmount] VARCHAR(MAX)
			,[MobilePhoneAfterAdjustmentAmount] VARCHAR(MAX)
			,[DutyMealAfterAdjustmentAmount] VARCHAR(MAX)
			,[HPIAfterAdjustmentAmount] VARCHAR(MAX)
			,[OthersAfterAdjustmentAmount] VARCHAR(MAX)
			,[SecondmentAfterAdjustmentAmount] VARCHAR(MAX)
			) AS Data
	INNER JOIN dbo.MonthlyPayroll AS mp WITH (NOLOCK) ON mp.EmployeeId = Data.EmployeeId
		AND mp.[Month] = @Month

	EXEC sp_xml_removedocument @iDoc

	--Update table 
	MERGE INTO dbo.MonthlyBasicAllowance AS ba
	USING (
		SELECT [PayrollId]
			,[CurrencyId]
			,[TotalAmount]
			,[DirectBanking1CurrencyId]
			,[DirectBanking1Amount]
			,[DirectBanking2CurrencyId]
			,[DirectBanking2Amount]
			,[CashCurrencyId]
			,[CashAmount]
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
			,[RoomingChargesCurrencyId]
			,[RoomingChargesAmount]
			,[FitnessFirstCurrencyId]
			,[FitnessFirstAmount]
			,[AutoCalculateOn]
			,[CreatedBy]
			,[CreatedDate]
			,[AfterAdjustmentAmount]
			,[Reason]
			,[SubsistenceAfterAdjustmentAmount]
			,[HousingAfterAdjustmentAmount]
			,[HomePassageAfterAdjustmentAmount]
			,[MobilePhoneAfterAdjustmentAmount]
			,[DutyMealAfterAdjustmentAmount]
			,[HPIAfterAdjustmentAmount]
			,[OthersAfterAdjustmentAmount]
			,[SecondmentAfterAdjustmentAmount]
		FROM #tblTempMonthlyBasicAllowance WITH (NOLOCK)
		) AS val([PayrollId], [CurrencyId], [TotalAmount], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking2CurrencyId], [DirectBanking2Amount], [CashCurrencyId], [CashAmount], [SubsistenceCurrencyId], [SubsistenceAmount], [HomePassageCurrencyId], [HomePassageAmount], [HousingCurrencyId], [HousingAmount], [MobilePhoneCurrencyId], [MobilePhoneAmount], [HPICurrencyId], [HPIAmount], [DutyMealCurrencyId], [DutyMealAmount], [SecondmentCurrencyId], [SecondmentAmount], [OthersCurrencyId], [OthersAmount], [RoomingChargesCurrencyId], [RoomingChargesAmount], [FitnessFirstCurrencyId], [FitnessFirstAmount], [AutoCalculateOn], [CreatedBy], [CreatedDate], [AfterAdjustmentAmount], [Reason], [SubsistenceAfterAdjustmentAmount], [HousingAfterAdjustmentAmount], [HomePassageAfterAdjustmentAmount], [MobilePhoneAfterAdjustmentAmount], [DutyMealAfterAdjustmentAmount], [HPIAfterAdjustmentAmount], [OthersAfterAdjustmentAmount], [SecondmentAfterAdjustmentAmount])
		ON ba.PayrollId = val.PayrollId
	WHEN MATCHED
		THEN
			UPDATE
			SET ba.CurrencyId = val.CurrencyId
				,ba.[TotalAmount] = val.[TotalAmount]
				,ba.DirectBanking1CurrencyId = val.DirectBanking1CurrencyId
				,ba.DirectBanking1Amount = val.DirectBanking1Amount
				,ba.DirectBanking2CurrencyId = val.DirectBanking2CurrencyId
				,ba.DirectBanking2Amount = val.DirectBanking2Amount
				,ba.CashCurrencyId = val.CashCurrencyId
				,ba.CashAmount = val.CashAmount
				,ba.[SubsistenceCurrencyId] = val.[SubsistenceCurrencyId]
				,ba.[SubsistenceAmount] = val.[SubsistenceAmount]
				,ba.[HomePassageCurrencyId] = val.[HomePassageCurrencyId]
				,ba.[HomePassageAmount] = val.[HomePassageAmount]
				,ba.[HousingCurrencyId] = val.[HousingCurrencyId]
				,ba.[HousingAmount] = val.[HousingAmount]
				,ba.[MobilePhoneCurrencyId] = val.[MobilePhoneCurrencyId]
				,ba.[MobilePhoneAmount] = val.[MobilePhoneAmount]
				,ba.[HPICurrencyId] = val.[HPICurrencyId]
				,ba.[HPIAmount] = val.[HPIAmount]
				,ba.[DutyMealCurrencyId] = val.[DutyMealCurrencyId]
				,ba.[DutyMealAmount] = val.[DutyMealAmount]
				,ba.[SecondmentCurrencyId] = val.[SecondmentCurrencyId]
				,ba.[SecondmentAmount] = val.[SecondmentAmount]
				,ba.[OthersCurrencyId] = val.[OthersCurrencyId]
				,ba.[OthersAmount] = val.[OthersAmount]
				,ba.[RoomingChargesCurrencyId] = val.[RoomingChargesCurrencyId]
				,ba.[RoomingChargesAmount] = val.[RoomingChargesAmount]
				,ba.[FitnessFirstCurrencyId] = val.[FitnessFirstCurrencyId]
				,ba.[FitnessFirstAmount] = val.[FitnessFirstAmount]
				,ba.AutoCalculateOn = val.AutoCalculateOn
				,ba.CreatedBy = val.CreatedBy
				,ba.CreatedDate = val.CreatedDate
				,ba.AfterAdjustmentAmount = val.AfterAdjustmentAmount
				,ba.[Reason] = val.[Reason]
				,ba.[SubsistenceAfterAdjustmentAmount] = val.[SubsistenceAfterAdjustmentAmount]
				,ba.[HousingAfterAdjustmentAmount] = val.[HousingAfterAdjustmentAmount]
				,ba.[HomePassageAfterAdjustmentAmount] = val.[HomePassageAfterAdjustmentAmount]
				,ba.[MobilePhoneAfterAdjustmentAmount] = val.[MobilePhoneAfterAdjustmentAmount]
				,ba.[DutyMealAfterAdjustmentAmount] = val.[DutyMealAfterAdjustmentAmount]
				,ba.[HPIAfterAdjustmentAmount] = val.[HPIAfterAdjustmentAmount]
				,ba.[OthersAfterAdjustmentAmount] = val.[OthersAfterAdjustmentAmount]
				,ba.[SecondmentAfterAdjustmentAmount] = val.[SecondmentAfterAdjustmentAmount]
	WHEN NOT MATCHED
		THEN
			INSERT (
				[PayrollId]
				,[CurrencyId]
				,[TotalAmount]
				,[DirectBanking1CurrencyId]
				,[DirectBanking1Amount]
				,[DirectBanking2CurrencyId]
				,[DirectBanking2Amount]
				,[CashCurrencyId]
				,[CashAmount]
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
				,[RoomingChargesCurrencyId]
				,[RoomingChargesAmount]
				,[FitnessFirstCurrencyId]
				,[FitnessFirstAmount]
				,[AutoCalculateOn]
				,[CreatedBy]
				,[CreatedDate]
				,[AfterAdjustmentAmount]
				,[Reason]
				,[SubsistenceAfterAdjustmentAmount]
				,[HousingAfterAdjustmentAmount]
				,[HomePassageAfterAdjustmentAmount]
				,[MobilePhoneAfterAdjustmentAmount]
				,[DutyMealAfterAdjustmentAmount]
				,[HPIAfterAdjustmentAmount]
				,[OthersAfterAdjustmentAmount]
				,[SecondmentAfterAdjustmentAmount]
				)
			VALUES (
				val.PayrollId
				,val.[CurrencyId]
				,val.[TotalAmount]
				,val.[DirectBanking1CurrencyId]
				,val.[DirectBanking1Amount]
				,val.[DirectBanking2CurrencyId]
				,val.[DirectBanking2Amount]
				,val.[CashCurrencyId]
				,val.[CashAmount]
				,val.[SubsistenceCurrencyId]
				,val.[SubsistenceAmount]
				,val.[HomePassageCurrencyId]
				,val.[HomePassageAmount]
				,val.[HousingCurrencyId]
				,val.[HousingAmount]
				,val.[MobilePhoneCurrencyId]
				,val.[MobilePhoneAmount]
				,val.[HPICurrencyId]
				,val.[HPIAmount]
				,val.[DutyMealCurrencyId]
				,val.[DutyMealAmount]
				,val.[SecondmentCurrencyId]
				,val.[SecondmentAmount]
				,val.[OthersCurrencyId]
				,val.[OthersAmount]
				,val.[RoomingChargesCurrencyId]
				,val.[RoomingChargesAmount]
				,val.[FitnessFirstCurrencyId]
				,val.[FitnessFirstAmount]
				,val.[AutoCalculateOn]
				,val.[CreatedBy]
				,val.[CreatedDate]
				,val.[AfterAdjustmentAmount]
				,val.[Reason]
				,val.[SubsistenceAfterAdjustmentAmount]
				,val.[HousingAfterAdjustmentAmount]
				,val.[HomePassageAfterAdjustmentAmount]
				,val.[MobilePhoneAfterAdjustmentAmount]
				,val.[DutyMealAfterAdjustmentAmount]
				,val.[HPIAfterAdjustmentAmount]
				,val.[OthersAfterAdjustmentAmount]
				,val.[SecondmentAfterAdjustmentAmount]
				)
	OUTPUT DELETED.PayrollId
		,DELETED.[CurrencyId]
		,DELETED.[TotalAmount]
		,DELETED.[DirectBanking1CurrencyId]
		,DELETED.[DirectBanking1Amount]
		,DELETED.[DirectBanking2CurrencyId]
		,DELETED.[DirectBanking2Amount]
		,DELETED.[CashCurrencyId]
		,DELETED.[CashAmount]
		,DELETED.[SubsistenceCurrencyId]
		,DELETED.[SubsistenceAmount]
		,DELETED.[HomePassageCurrencyId]
		,DELETED.[HomePassageAmount]
		,DELETED.[HousingCurrencyId]
		,DELETED.[HousingAmount]
		,DELETED.[MobilePhoneCurrencyId]
		,DELETED.[MobilePhoneAmount]
		,DELETED.[HPICurrencyId]
		,DELETED.[HPIAmount]
		,DELETED.[DutyMealCurrencyId]
		,DELETED.[DutyMealAmount]
		,DELETED.[SecondmentCurrencyId]
		,DELETED.[SecondmentAmount]
		,DELETED.[OthersCurrencyId]
		,DELETED.[OthersAmount]
		,DELETED.[RoomingChargesCurrencyId]
		,DELETED.[RoomingChargesAmount]
		,DELETED.[FitnessFirstCurrencyId]
		,DELETED.[FitnessFirstAmount]
		,DELETED.[AutoCalculateOn]
		,DELETED.[CreatedBy]
		,DELETED.[CreatedDate]
		,DELETED.[AfterAdjustmentAmount]
		,DELETED.[Reason]
		,DELETED.[SubsistenceAfterAdjustmentAmount]
		,DELETED.[HousingAfterAdjustmentAmount]
		,DELETED.[HomePassageAfterAdjustmentAmount]
		,DELETED.[MobilePhoneAfterAdjustmentAmount]
		,DELETED.[DutyMealAfterAdjustmentAmount]
		,DELETED.[HPIAfterAdjustmentAmount]
		,DELETED.[OthersAfterAdjustmentAmount]
		,DELETED.[SecondmentAfterAdjustmentAmount]
	INTO #tblOldLogData([PayrollId], [CurrencyId], [TotalAmount], [DirectBanking1CurrencyId], [DirectBanking1Amount], [DirectBanking2CurrencyId], [DirectBanking2Amount], [CashCurrencyId], [CashAmount], [SubsistenceCurrencyId], [SubsistenceAmount], [HomePassageCurrencyId], [HomePassageAmount], [HousingCurrencyId], [HousingAmount], [MobilePhoneCurrencyId], [MobilePhoneAmount], [HPICurrencyId], [HPIAmount], [DutyMealCurrencyId], [DutyMealAmount], [SecondmentCurrencyId], [SecondmentAmount], [OthersCurrencyId], [OthersAmount], [RoomingChargesCurrencyId], [RoomingChargesAmount], [FitnessFirstCurrencyId], [FitnessFirstAmount], [AutoCalculateOn], [CreatedBy], [CreatedDate], [AfterAdjustmentAmount], [Reason], [SubsistenceAfterAdjustmentAmount], [HousingAfterAdjustmentAmount], [HomePassageAfterAdjustmentAmount], [MobilePhoneAfterAdjustmentAmount], [DutyMealAfterAdjustmentAmount], [HPIAfterAdjustmentAmount], [OthersAfterAdjustmentAmount], [SecondmentAfterAdjustmentAmount]);

	--WRITE LOG
	SELECT @maxLength = Count(1)
	FROM #tblTempMonthlyBasicAllowance

	WHILE @i <= @maxLength
	BEGIN
		SET @xml = (
				SELECT o.PayrollId AS '@pid'
					,o.[CurrencyId] AS '@cid'
					,o.[TotalAmount] AS '@ta'
					,o.[DirectBanking1CurrencyId] AS '@d1ci'
					,o.[DirectBanking1Amount] AS '@d1a'
					,o.[DirectBanking2CurrencyId] AS '@d2ci'
					,o.[DirectBanking2Amount] AS '@d2a'
					,o.[CashCurrencyId] AS '@cci'
					,o.[CashAmount] AS '@ca'
					,o.[SubsistenceCurrencyId] AS '@sci'
					,o.[SubsistenceAmount] AS '@sa'
					,o.[HomePassageCurrencyId] AS '@hpci'
					,o.[HomePassageAmount] AS '@hpa'
					,o.[HousingCurrencyId] AS '@hci'
					,o.[HousingAmount] AS '@ha'
					,o.[MobilePhoneCurrencyId] AS '@mci'
					,o.[MobilePhoneAmount] AS '@ma'
					,o.[HPICurrencyId] AS '@hpici'
					,o.[HPIAmount] AS '@hpia'
					,o.[DutyMealCurrencyId] AS '@dmci'
					,o.[DutyMealAmount] AS '@dma'
					,o.[SecondmentCurrencyId] AS '@seci'
					,o.[SecondmentAmount] AS '@sea'
					,o.[OthersCurrencyId] AS '@oci'
					,o.[OthersAmount] AS '@oa'
					,o.[RoomingChargesCurrencyId] AS '@rcci'
					,o.[RoomingChargesAmount] AS '@rca'
					,o.[FitnessFirstCurrencyId] AS '@ffci'
					,o.[FitnessFirstAmount] AS '@ffa'
					,o.[AutoCalculateOn] AS '@aco'
					,o.[CreatedBy] AS '@cb'
					,o.[CreatedDate] AS '@cd'
					,o.[AfterAdjustmentAmount] AS '@aaa'
					,o.[Reason] AS '@r'
					,o.[SubsistenceAfterAdjustmentAmount] AS '@suaaa'
					,o.[HousingAfterAdjustmentAmount] AS '@haaa'
					,o.[HomePassageAfterAdjustmentAmount] AS '@hpaaa'
					,o.[MobilePhoneAfterAdjustmentAmount] AS '@mpaaa'
					,o.[DutyMealAfterAdjustmentAmount] AS '@dmaaa'
					,o.[HPIAfterAdjustmentAmount] AS '@hpiaaa'
					,o.[OthersAfterAdjustmentAmount] AS '@oaaa'
					,o.[SecondmentAfterAdjustmentAmount] AS '@seaaa'
				FROM #tblOldLogData AS o WITH (NOLOCK)
				INNER JOIN #tblTempMonthlyBasicAllowance AS t WITH (NOLOCK) ON o.PayrollId = t.PayrollId
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
					,t.[DirectBanking2CurrencyId] AS '@d2ci'
					,t.[DirectBanking2Amount] AS '@d2a'
					,t.[CashCurrencyId] AS '@cci'
					,t.[CashAmount] AS '@ca'
					,t.[SubsistenceCurrencyId] AS '@sci'
					,t.[SubsistenceAmount] AS '@sa'
					,t.[HomePassageCurrencyId] AS '@hpci'
					,t.[HomePassageAmount] AS '@hpa'
					,t.[HousingCurrencyId] AS '@hci'
					,t.[HousingAmount] AS '@ha'
					,t.[MobilePhoneCurrencyId] AS '@mci'
					,t.[MobilePhoneAmount] AS '@ma'
					,t.[HPICurrencyId] AS '@hpici'
					,t.[HPIAmount] AS '@hpia'
					,t.[DutyMealCurrencyId] AS '@dmci'
					,t.[DutyMealAmount] AS '@dma'
					,t.[SecondmentCurrencyId] AS '@seci'
					,t.[SecondmentAmount] AS '@sea'
					,t.[OthersCurrencyId] AS '@oci'
					,t.[OthersAmount] AS '@oa'
					,t.[RoomingChargesCurrencyId] AS '@rcci'
					,t.[RoomingChargesAmount] AS '@rca'
					,t.[FitnessFirstCurrencyId] AS '@ffci'
					,t.[FitnessFirstAmount] AS '@ffa'
					,t.[AutoCalculateOn] AS '@aco'
					,t.[CreatedBy] AS '@cb'
					,t.[CreatedDate] AS '@cd'
					,t.[AfterAdjustmentAmount] AS '@aaa'
					,t.[Reason] AS '@r'
					,t.[SubsistenceAfterAdjustmentAmount] AS '@suaaa'
					,t.[HousingAfterAdjustmentAmount] AS '@haaa'
					,t.[HomePassageAfterAdjustmentAmount] AS '@hpaaa'
					,t.[MobilePhoneAfterAdjustmentAmount] AS '@mpaaa'
					,t.[DutyMealAfterAdjustmentAmount] AS '@dmaaa'
					,t.[HPIAfterAdjustmentAmount] AS '@hpiaaa'
					,t.[OthersAfterAdjustmentAmount] AS '@oaaa'
					,t.[SecondmentAfterAdjustmentAmount] AS '@seaaa'
				FROM #tblTempMonthlyBasicAllowance AS t WITH (NOLOCK)
				WHERE RowNum = @i
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		SELECT @ObjectId = t.PayrollId
		FROM #tblTempMonthlyBasicAllowance AS t WITH (NOLOCK)
		WHERE t.RowNum = @i

		SET @endRun = GETDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
			,@SPName = 'PaymentProcess_CreateMonthlyBasicAllowance'
			,@ObjectId = @ObjectId
			,@ObjectTypeId = NULL
			,@Data = @logData
			,@OldData = @oldLogData
			,@Duration = @Duration

		SET @i = @i + 1
	END

	DROP TABLE #tblOldLogData

	DROP TABLE #tblTempMonthlyBasicAllowance
END