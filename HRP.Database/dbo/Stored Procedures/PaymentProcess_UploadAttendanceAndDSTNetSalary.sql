
CREATE PROCEDURE [dbo].[PaymentProcess_UploadAttendanceAndDSTNetSalary] @CompanyId SMALLINT
	,@Month DATE
	,@AttendanceSessionId NVARCHAR(100) = NULL
	,@AttendanceFileName NVARCHAR(50) = NULL
	,@AttendanceUploadingXMLDoc NVARCHAR(MAX) = NULL
	,@AttendanceResultXMLDoc NVARCHAR(MAX) = NULL
	,@DSTNetSalarySessionId NVARCHAR(100) = NULL
	,@DSTNetSalaryFileName NVARCHAR(50) = NULL
	,@DSTNetSalaryUploadingXMLDoc NVARCHAR(MAX) = NULL
	,@DSTNetSalaryResultXMLDoc NVARCHAR(MAX) = NULL
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(500) = '' OUTPUT
AS
/*
Created: 20180326@Raymond.Le
Task: Insert excel record into AttendanceUploading and DSTNetSalaryUploading table
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @upDoc INT
		,@reDoc INT

	BEGIN TRY
		BEGIN TRANSACTION UPLOADFILES

		-- Upload Attendance
		IF (@AttendanceSessionId IS NOT NULL)
		BEGIN
			--Insert into AttendanceUploading
			EXEC sp_xml_preparedocument @upDoc OUTPUT
				,@AttendanceUploadingXMLDoc

			INSERT INTO dbo.AttendanceUploading (
				SessionId
				,RowNumber
				,EmployeeCode
				,ActualWorkingDay
				)
			SELECT @AttendanceSessionId
				,Data.RowNumber
				,Data.EmployeeCode
				,Data.ActualWorkingDay
			FROM OPENXML(@upDoc, '/Root/Row', 1) WITH (RowNumber INT, EmployeeCode NVARCHAR(2000), ActualWorkingDay NVARCHAR(20)) AS Data

			EXEC sp_xml_removedocument @upDoc

			--Insert into table result
			EXEC sp_xml_preparedocument @reDoc OUTPUT
				,@AttendanceResultXMLDoc

			INSERT INTO dbo.AttendanceUploadingResult (
				SessionId
				,RowNumber
				,ResultType
				,[Message]
				)
			SELECT @AttendanceSessionId
				,Data.RowNumber
				,Data.ResultType
				,Data.[Message]
			FROM OPENXML(@reDoc, '/Root/Row', 1) WITH (RowNumber INT, ResultType SMALLINT, [Message] NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @reDoc

			--Update table PaymentProcess
			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET AttendanceFileName = @AttendanceFileName
				,AttendanceSessionId = @AttendanceSessionId
				,IsAttendanceFileSubmitted = 0
				,IsPayrollGenerated = 0
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END

		-- Upload DST Net Salary
		IF @DSTNetSalarySessionId IS NOT NULL
		BEGIN
			--Insert into table DSTNetSalaryUploading
			EXEC sp_xml_preparedocument @upDoc OUTPUT
				,@DSTNetSalaryUploadingXMLDoc

			INSERT INTO dbo.DSTNetSalaryUploading (
				SessionId
				,RowNumber
				,EmployeeCode
				,Currency
				,NetSalaryReceived
				)
			SELECT @DSTNetSalarySessionId
				,Data.RowNumber
				,Data.EmployeeCode
				,Data.Currency
				,Data.NetSalaryReceived
			FROM OPENXML(@upDoc, '/Root/Row', 1) WITH (RowNumber INT, EmployeeCode NVARCHAR(2000), Currency NVARCHAR(20), NetSalaryReceived VARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @upDoc

			--Insert into table result
			EXEC sp_xml_preparedocument @reDoc OUTPUT
				,@DSTNetSalaryResultXMLDoc

			INSERT INTO dbo.DSTNetSalaryUploadingResult (
				SessionId
				,RowNumber
				,ResultType
				,[Message]
				)
			SELECT @DSTNetSalarySessionId
				,Data.RowNumber
				,Data.ResultType
				,Data.[Message]
			FROM OPENXML(@reDoc, '/Root/Row', 1) WITH (RowNumber INT, ResultType SMALLINT, [Message] NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @reDoc

			--Update table PaymentProcess
			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET DSTNetSalaryFileName = @DSTNetSalaryFileName
				,DSTNetSalarySessionId = @DSTNetSalarySessionId
				,IsDSTNetSalaryFileSubmitted = 0
				,IsPayrollGenerated = 0
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END

		-- Commit
		COMMIT TRANSACTION UPLOADFILES

		PRINT 'End. Insert table uploading - OK'
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
		BEGIN
			ROLLBACK TRANSACTION UPLOADFILES

			PRINT 'Error-Rollback'
		END

		SET @ErrCode = 1
		SET @ErrMsg = 'ENumber: ' + CAST(ERROR_NUMBER() AS VARCHAR(50)) + ' --EProcedure: ' + ERROR_PROCEDURE() + ' --ELine: ' + CAST(ERROR_LINE() AS VARCHAR(50)) + ' --ErrorMessage: ' + ERROR_MESSAGE()
	END CATCH
END