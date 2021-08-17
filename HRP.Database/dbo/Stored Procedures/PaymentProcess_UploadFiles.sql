
CREATE PROCEDURE dbo.PaymentProcess_UploadFiles @CompanyId SMALLINT
	,@Month DATE
	,@AttendanceSessionId NVARCHAR(100) = NULL
	,@AttendanceFileName NVARCHAR(50) = NULL
	,@AttendanceUploadingXMLDoc NVARCHAR(MAX) = NULL
	,@AttendanceResultXMLDoc NVARCHAR(MAX) = NULL
	,@DSTNetSalarySessionId NVARCHAR(100) = NULL
	,@DSTNetSalaryFileName NVARCHAR(50) = NULL
	,@DSTNetSalaryUploadingXMLDoc NVARCHAR(MAX) = NULL
	,@DSTNetSalaryResultXMLDoc NVARCHAR(MAX) = NULL
	,@AdditionDeductionSessionId NVARCHAR(100) = NULL
	,@AdditionDeductionFileName NVARCHAR(50) = NULL
	,@AdditionDeductionUploadingXMLDoc NVARCHAR(MAX) = NULL
	,@AdditionDeductionResultXMLDoc NVARCHAR(MAX) = NULL
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(500) = '' OUTPUT
AS
/*
Created: 20210810@Tim.Trinh
Task: upload files
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @iDoc INT

	BEGIN TRY
		BEGIN TRANSACTION UPLOADFILES

		-- Upload Attendance
		IF @AttendanceSessionId IS NOT NULL
		BEGIN
			EXEC sp_xml_preparedocument @iDoc OUTPUT
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
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (RowNumber INT, EmployeeCode NVARCHAR(2000), ActualWorkingDay NVARCHAR(20)) AS Data

			EXEC sp_xml_removedocument @iDoc

			EXEC sp_xml_preparedocument @iDoc OUTPUT
				,@AttendanceResultXMLDoc

			INSERT INTO dbo.AttendanceUploadingResult (
				SessionId
				,RowNumber
				,ResultType
				,Message
				)
			SELECT @AttendanceSessionId
				,Data.RowNumber
				,Data.ResultType
				,Data.Message
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (RowNumber INT, ResultType SMALLINT, Message NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @iDoc

			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET AttendanceFileName = @AttendanceFileName
				,AttendanceSessionId = @AttendanceSessionId
				,IsAttendanceFileSubmitted = 0
				,IsPayrollGenerated = 0
			WHERE CompanyId = @CompanyId
				AND Month = @Month
		END

		-- Upload DST Net Salary
		IF @DSTNetSalarySessionId IS NOT NULL
		BEGIN
			EXEC sp_xml_preparedocument @iDoc OUTPUT
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
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (RowNumber INT, EmployeeCode NVARCHAR(2000), Currency NVARCHAR(20), NetSalaryReceived VARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @iDoc

			EXEC sp_xml_preparedocument @iDoc OUTPUT
				,@DSTNetSalaryResultXMLDoc

			INSERT INTO dbo.DSTNetSalaryUploadingResult (
				SessionId
				,RowNumber
				,ResultType
				,Message
				)
			SELECT @DSTNetSalarySessionId
				,Data.RowNumber
				,Data.ResultType
				,Data.Message
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (RowNumber INT, ResultType SMALLINT, Message NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @iDoc

			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET DSTNetSalaryFileName = @DSTNetSalaryFileName
				,DSTNetSalarySessionId = @DSTNetSalarySessionId
				,IsDSTNetSalaryFileSubmitted = 0
				,IsPayrollGenerated = 0
			WHERE CompanyId = @CompanyId
				AND Month = @Month
		END

		-- Upload Addition & Deduction
		IF @AdditionDeductionSessionId IS NOT NULL
		BEGIN
			EXEC sp_xml_preparedocument @iDoc OUTPUT
				,@AdditionDeductionUploadingXMLDoc

			INSERT INTO dbo.AdditionDeductionUploading (
				SessionId
				,RowNumber
				,EmployeeCode
				,Salary_Addition_Adjustments_Comment
				,Salary_Addition_Adjustments_Amount
				,Salary_Addition_Adjustments_NoOfDays
				,Salary_Addition_Others1_Comment
				,Salary_Addition_Others1_Amount
				,Salary_Addition_Others1_NoOfDays
				,Salary_Addition_Others2_Comment
				,Salary_Addition_Others2_Amount
				,Salary_Addition_Others2_NoOfDays
				,Salary_Deduction_Absenteeism_Comment
				,Salary_Deduction_Absenteeism_Days
				,Salary_Deduction_Tardiness_Comment
				,Salary_Deduction_Tardiness_Minutes
				,Salary_Deduction_Advance_Comment
				,Salary_Deduction_Advance_Amount
				,Salary_Deduction_Others_Comment
				,Salary_Deduction_Others_Amount
				,Allowance_Addition_Subsistence_Amount
				,Allowance_Addition_Subsistence_CurrencyCode
				,Allowance_Addition_Subsistence_NoOfDays
				,Allowance_Addition_HomePassage_Amount
				,Allowance_Addition_HomePassage_CurrencyCode
				,Allowance_Addition_HomePassage_NoOfDays
				,Allowance_Addition_Housing_Amount
				,Allowance_Addition_Housing_CurrencyCode
				,Allowance_Addition_Housing_NoOfDays
				,Allowance_Addition_MobilePhone_Amount
				,Allowance_Addition_MobilePhone_CurrencyCode
				,Allowance_Addition_MobilePhone_NoOfDays
				,Allowance_Addition_HPI_Amount
				,Allowance_Addition_HPI_CurrencyCode
				,Allowance_Addition_HPI_NoOfDays
				,Allowance_Addition_DutyMeal_CurrencyCode
				,Allowance_Addition_DutyMeal_NoOfDays
				,Allowance_Addition_Secondment_Amount
				,Allowance_Addition_Secondment_CurrencyCode
				,Allowance_Addition_Secondment_NoOfDays
				,Allowance_Addition_SpecialAllowance_Amount
				,Allowance_Addition_SpecialAllowance_CurrencyCode
				,Allowance_Addition_SpecialAllowance_NoOfDays
				,Allowance_Addition_OT_Amount
				,Allowance_Addition_OT_CurrencyCode
				,Allowance_Addition_OT_NoOfDays
				,Allowance_Addition_Others_Amount
				,Allowance_Addition_Others_CurrencyCode
				,Allowance_Addition_Others_NoOfDays
				,Allowance_Addition_Comment
				,Allowance_Deduction_Subsistence_Amount
				,Allowance_Deduction_Subsistence_CurrencyCode
				,Allowance_Deduction_Subsistence_NoOfDays
				,Allowance_Deduction_HomePassage_Amount
				,Allowance_Deduction_HomePassage_CurrencyCode
				,Allowance_Deduction_HomePassage_NoOfDays
				,Allowance_Deduction_Housing_Amount
				,Allowance_Deduction_Housing_CurrencyCode
				,Allowance_Deduction_Housing_NoOfDays
				,Allowance_Deduction_MobilePhone_Amount
				,Allowance_Deduction_MobilePhone_CurrencyCode
				,Allowance_Deduction_MobilePhone_NoOfDays
				,Allowance_Deduction_HPI_Amount
				,Allowance_Deduction_HPI_CurrencyCode
				,Allowance_Deduction_HPI_NoOfDays
				,Allowance_Deduction_DutyMeal_CurrencyCode
				,Allowance_Deduction_DutyMeal_NoOfDays
				,Allowance_Deduction_Secondment_Amount
				,Allowance_Deduction_Secondment_CurrencyCode
				,Allowance_Deduction_Secondment_NoOfDays
				,Allowance_Deduction_Others_Amount
				,Allowance_Deduction_Others_CurrencyCode
				,Allowance_Deduction_Others_NoOfDays
				,Allowance_Deduction_Comment
				)
			SELECT @AdditionDeductionSessionId
				,RowNumber
				,EmployeeCode
				,Salary_Addition_Adjustments_Comment
				,Salary_Addition_Adjustments_Amount
				,Salary_Addition_Adjustments_NoOfDays
				,Salary_Addition_Others1_Comment
				,Salary_Addition_Others1_Amount
				,Salary_Addition_Others1_NoOfDays
				,Salary_Addition_Others2_Comment
				,Salary_Addition_Others2_Amount
				,Salary_Addition_Others2_NoOfDays
				,Salary_Deduction_Absenteeism_Comment
				,Salary_Deduction_Absenteeism_Days
				,Salary_Deduction_Tardiness_Comment
				,Salary_Deduction_Tardiness_Minutes
				,Salary_Deduction_Advance_Comment
				,Salary_Deduction_Advance_Amount
				,Salary_Deduction_Others_Comment
				,Salary_Deduction_Others_Amount
				,Allowance_Addition_Subsistence_Amount
				,Allowance_Addition_Subsistence_CurrencyCode
				,Allowance_Addition_Subsistence_NoOfDays
				,Allowance_Addition_HomePassage_Amount
				,Allowance_Addition_HomePassage_CurrencyCode
				,Allowance_Addition_HomePassage_NoOfDays
				,Allowance_Addition_Housing_Amount
				,Allowance_Addition_Housing_CurrencyCode
				,Allowance_Addition_Housing_NoOfDays
				,Allowance_Addition_MobilePhone_Amount
				,Allowance_Addition_MobilePhone_CurrencyCode
				,Allowance_Addition_MobilePhone_NoOfDays
				,Allowance_Addition_HPI_Amount
				,Allowance_Addition_HPI_CurrencyCode
				,Allowance_Addition_HPI_NoOfDays
				,Allowance_Addition_DutyMeal_CurrencyCode
				,Allowance_Addition_DutyMeal_NoOfDays
				,Allowance_Addition_Secondment_Amount
				,Allowance_Addition_Secondment_CurrencyCode
				,Allowance_Addition_Secondment_NoOfDays
				,Allowance_Addition_SpecialAllowance_Amount
				,Allowance_Addition_SpecialAllowance_CurrencyCode
				,Allowance_Addition_SpecialAllowance_NoOfDays
				,Allowance_Addition_OT_Amount
				,Allowance_Addition_OT_CurrencyCode
				,Allowance_Addition_OT_NoOfDays
				,Allowance_Addition_Others_Amount
				,Allowance_Addition_Others_CurrencyCode
				,Allowance_Addition_Others_NoOfDays
				,Allowance_Addition_Comment
				,Allowance_Deduction_Subsistence_Amount
				,Allowance_Deduction_Subsistence_CurrencyCode
				,Allowance_Deduction_Subsistence_NoOfDays
				,Allowance_Deduction_HomePassage_Amount
				,Allowance_Deduction_HomePassage_CurrencyCode
				,Allowance_Deduction_HomePassage_NoOfDays
				,Allowance_Deduction_Housing_Amount
				,Allowance_Deduction_Housing_CurrencyCode
				,Allowance_Deduction_Housing_NoOfDays
				,Allowance_Deduction_MobilePhone_Amount
				,Allowance_Deduction_MobilePhone_CurrencyCode
				,Allowance_Deduction_MobilePhone_NoOfDays
				,Allowance_Deduction_HPI_Amount
				,Allowance_Deduction_HPI_CurrencyCode
				,Allowance_Deduction_HPI_NoOfDays
				,Allowance_Deduction_DutyMeal_CurrencyCode
				,Allowance_Deduction_DutyMeal_NoOfDays
				,Allowance_Deduction_Secondment_Amount
				,Allowance_Deduction_Secondment_CurrencyCode
				,Allowance_Deduction_Secondment_NoOfDays
				,Allowance_Deduction_Others_Amount
				,Allowance_Deduction_Others_CurrencyCode
				,Allowance_Deduction_Others_NoOfDays
				,Allowance_Deduction_Comment
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (
				RowNumber INT
				,EmployeeCode NVARCHAR(50)
				,Salary_Addition_Adjustments_Comment NVARCHAR(1000)
				,Salary_Addition_Adjustments_Amount VARCHAR(MAX)
				,Salary_Addition_Adjustments_NoOfDays VARCHAR(20)
				,Salary_Addition_Others1_Comment NVARCHAR(1000)
				,Salary_Addition_Others1_Amount VARCHAR(MAX)
				,Salary_Addition_Others1_NoOfDays VARCHAR(20)
				,Salary_Addition_Others2_Comment NVARCHAR(1000)
				,Salary_Addition_Others2_Amount VARCHAR(MAX)
				,Salary_Addition_Others2_NoOfDays VARCHAR(20)
				,Salary_Deduction_Absenteeism_Comment NVARCHAR(1000)
				,Salary_Deduction_Absenteeism_Days VARCHAR(10)
				,Salary_Deduction_Tardiness_Comment NVARCHAR(1000)
				,Salary_Deduction_Tardiness_Minutes VARCHAR(10)
				,Salary_Deduction_Advance_Comment NVARCHAR(1000)
				,Salary_Deduction_Advance_Amount VARCHAR(MAX)
				,Salary_Deduction_Others_Comment NVARCHAR(1000)
				,Salary_Deduction_Others_Amount VARCHAR(MAX)
				,Allowance_Addition_Subsistence_Amount VARCHAR(MAX)
				,Allowance_Addition_Subsistence_CurrencyCode VARCHAR(50)
				,Allowance_Addition_Subsistence_NoOfDays VARCHAR(10)
				,Allowance_Addition_HomePassage_Amount VARCHAR(MAX)
				,Allowance_Addition_HomePassage_CurrencyCode VARCHAR(50)
				,Allowance_Addition_HomePassage_NoOfDays VARCHAR(10)
				,Allowance_Addition_Housing_Amount VARCHAR(MAX)
				,Allowance_Addition_Housing_CurrencyCode VARCHAR(50)
				,Allowance_Addition_Housing_NoOfDays VARCHAR(10)
				,Allowance_Addition_MobilePhone_Amount VARCHAR(MAX)
				,Allowance_Addition_MobilePhone_CurrencyCode VARCHAR(50)
				,Allowance_Addition_MobilePhone_NoOfDays VARCHAR(10)
				,Allowance_Addition_HPI_Amount VARCHAR(MAX)
				,Allowance_Addition_HPI_CurrencyCode VARCHAR(50)
				,Allowance_Addition_HPI_NoOfDays VARCHAR(10)
				,Allowance_Addition_DutyMeal_CurrencyCode VARCHAR(50)
				,Allowance_Addition_DutyMeal_NoOfDays VARCHAR(10)
				,Allowance_Addition_Secondment_Amount VARCHAR(MAX)
				,Allowance_Addition_Secondment_CurrencyCode VARCHAR(50)
				,Allowance_Addition_Secondment_NoOfDays VARCHAR(10)
				,Allowance_Addition_SpecialAllowance_Amount VARCHAR(MAX)
				,Allowance_Addition_SpecialAllowance_CurrencyCode VARCHAR(50)
				,Allowance_Addition_SpecialAllowance_NoOfDays VARCHAR(10)
				,Allowance_Addition_OT_Amount VARCHAR(MAX)
				,Allowance_Addition_OT_CurrencyCode VARCHAR(50)
				,Allowance_Addition_OT_NoOfDays VARCHAR(10)
				,Allowance_Addition_Others_Amount VARCHAR(MAX)
				,Allowance_Addition_Others_CurrencyCode VARCHAR(50)
				,Allowance_Addition_Others_NoOfDays VARCHAR(10)
				,Allowance_Addition_Comment NVARCHAR(MAX)
				,Allowance_Deduction_Subsistence_Amount VARCHAR(MAX)
				,Allowance_Deduction_Subsistence_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_Subsistence_NoOfDays VARCHAR(10)
				,Allowance_Deduction_HomePassage_Amount VARCHAR(MAX)
				,Allowance_Deduction_HomePassage_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_HomePassage_NoOfDays VARCHAR(10)
				,Allowance_Deduction_Housing_Amount VARCHAR(MAX)
				,Allowance_Deduction_Housing_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_Housing_NoOfDays VARCHAR(10)
				,Allowance_Deduction_MobilePhone_Amount VARCHAR(MAX)
				,Allowance_Deduction_MobilePhone_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_MobilePhone_NoOfDays VARCHAR(10)
				,Allowance_Deduction_HPI_Amount VARCHAR(MAX)
				,Allowance_Deduction_HPI_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_HPI_NoOfDays VARCHAR(10)
				,Allowance_Deduction_DutyMeal_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_DutyMeal_NoOfDays VARCHAR(10)
				,Allowance_Deduction_Secondment_Amount VARCHAR(MAX)
				,Allowance_Deduction_Secondment_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_Secondment_NoOfDays VARCHAR(10)
				,Allowance_Deduction_Others_Amount VARCHAR(MAX)
				,Allowance_Deduction_Others_CurrencyCode VARCHAR(50)
				,Allowance_Deduction_Others_NoOfDays VARCHAR(10)
				,Allowance_Deduction_Comment NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @iDoc

			EXEC sp_xml_preparedocument @iDoc OUTPUT
				,@AdditionDeductionResultXMLDoc

			INSERT INTO dbo.AdditionDeductionUploadingResult (
				SessionId
				,RowNumber
				,ResultType
				,Message
				)
			SELECT @AdditionDeductionSessionId
				,Data.RowNumber
				,Data.ResultType
				,Data.Message
			FROM OPENXML(@iDoc, '/Root/Row', 1) WITH (RowNumber INT, ResultType SMALLINT, Message NVARCHAR(MAX)) AS Data

			EXEC sp_xml_removedocument @iDoc

			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET AdditionDeductionFileName = @AdditionDeductionFileName
				,AdditionDeductionSessionId = @AdditionDeductionSessionId
				,IsAdditionDeductionFileSubmitted = 0
				,IsPayrollGenerated = 0
			WHERE CompanyId = @CompanyId
				AND Month = @Month
		END

		-- Commit
		COMMIT TRANSACTION UPLOADFILES

		PRINT 'End. Insert table uploading - OK'
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION UPLOADFILES

			PRINT 'Error-Rollback'
		END

		SET @ErrCode = 1
		SET @ErrMsg = 'ENumber: ' + CAST(ERROR_NUMBER() AS VARCHAR(50)) + ' --EProcedure: ' + ERROR_PROCEDURE() + ' --ELine: ' + CAST(ERROR_LINE() AS VARCHAR(50)) + ' --ErrorMessage: ' + ERROR_MESSAGE()
	END CATCH
END