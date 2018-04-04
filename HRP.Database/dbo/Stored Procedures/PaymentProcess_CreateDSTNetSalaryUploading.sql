
CREATE PROCEDURE [dbo].[PaymentProcess_CreateDSTNetSalaryUploading] @SessionId NVARCHAR(100)
	,@CompanyId SMALLINT
	,@Month DATE
	,@FileName NVARCHAR(50)
	,@UploadingXMLDoc NVARCHAR(MAX)
	,@ResultXMLDoc NVARCHAR(MAX)
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180313@Raymond.Le
Task: Create DST Net Salary uploading
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @upDoc INT
		,@reDoc INT

	--Insert into table uploading
	EXEC sp_xml_preparedocument @upDoc OUTPUT
		,@UploadingXMLDoc

	INSERT INTO dbo.DSTNetSalaryUploading(
		SessionId
		,RowNumber
		,EmployeeCode
		,Currency
		,NetSalaryReceived
		)
	SELECT @SessionId
		,Data.RowNumber
		,Data.EmployeeCode
		,Data.Currency
		,Data.NetSalaryReceived
	FROM OPENXML(@upDoc, '/Root/Row', 1) WITH (
			RowNumber INT
			,EmployeeCode NVARCHAR(2000)
			,Currency NVARCHAR(20)
			,NetSalaryReceived VARCHAR(MAX)
			) AS Data

	EXEC sp_xml_removedocument @upDoc

	--Insert into table result
	EXEC sp_xml_preparedocument @reDoc OUTPUT
		,@ResultXMLDoc

	INSERT INTO dbo.DSTNetSalaryUploadingResult(
		SessionId
		,RowNumber
		,ResultType
		,[Message]
		)
	SELECT @SessionId
		,Data.RowNumber
		,Data.ResultType
		,Data.[Message]
	FROM OPENXML(@reDoc, '/Root/Row', 1) WITH (
			RowNumber INT
			,ResultType SMALLINT
			,[Message] NVARCHAR(MAX)
			) AS Data

	EXEC sp_xml_removedocument @reDoc

	--Update table PaymentProcess
	MERGE INTO dbo.PaymentProcess AS pp
	USING (
		SELECT @CompanyId
			,@Month
		) AS val(CompanyId, [Month])
		ON pp.CompanyId = val.CompanyId
			AND pp.[Month] = val.[Month]
	WHEN MATCHED
		THEN
			UPDATE
			SET pp.DSTNetSalaryFileName = @FileName
				,pp.DSTNetSalarySessionId = @SessionId
				,pp.IsDSTNetSalaryFileSubmited = 0
	WHEN NOT MATCHED
		THEN
			INSERT (
				CompanyId
				,[Month]
				,DSTNetSalarySessionId
				,DSTNetSalaryFileName
				,IsDSTNetSalaryFileSubmited
				,IsPayrollGenerated
				)
			VALUES (
				@CompanyId
				,@Month
				,@SessionId
				,@FileName
				,0
				,0
				);

END