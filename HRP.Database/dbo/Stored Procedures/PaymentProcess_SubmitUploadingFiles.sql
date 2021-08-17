
CREATE PROCEDURE [dbo].[PaymentProcess_SubmitUploadingFiles] @CompanyId SMALLINT
	,@Month DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180315@Raymond.Le
Task: Insert data from AttendanceUploading & DSTNetSalaryUploading to table MonthlyAttendance & MonthlyDSTNetSalary
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @AttendanceSessionId NVARCHAR(100)
		,@DSTNetSalarySessionId NVARCHAR(100)
		,@IsAttendanceFileSubmitted BIT
		,@IsDSTNetSalaryFileSubmitted BIT
		,@IsAdditionDeductionFileSubmitted BIT

	CREATE TABLE #tblRecordError (
		SessionId NVARCHAR(100)
		,RowNumber INT
		)

	SELECT @AttendanceSessionId = p.AttendanceSessionId
		,@IsAttendanceFileSubmitted = p.IsAttendanceFileSubmitted
		,@DSTNetSalarySessionId = p.DSTNetSalarySessionId
		,@IsDSTNetSalaryFileSubmitted = p.IsDSTNetSalaryFileSubmitted
		,@IsAdditionDeductionFileSubmitted = p.IsAdditionDeductionFileSubmitted
	FROM dbo.PaymentProcess AS p WITH (NOLOCK)
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month

	BEGIN TRY
		--Check if has Attendance file is uploading
		IF ISNULL(@IsAttendanceFileSubmitted, 0) = 0
		BEGIN
			--Get Attendance records has error
			INSERT INTO #tblRecordError (
				SessionId
				,RowNumber
				)
			SELECT DISTINCT r.SessionId
				,r.RowNumber
			FROM dbo.AttendanceUploadingResult AS r WITH (NOLOCK)
			WHERE r.SessionId = @AttendanceSessionId
				AND r.ResultType = 2

			--Update table MonthlyAttendance
			DELETE ma
			FROM dbo.MonthlyAttendance AS ma WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON ma.EmployeeId = em.EmployeeId
			WHERE ma.[Month] = @Month
				AND em.CompanyId = @CompanyId

			INSERT INTO dbo.MonthlyAttendance (EmployeeId, [Month], ActualWorkingDay)
			SELECT e.EmployeeId
				,@Month
				,CASE 
					WHEN ISNUMERIC(u.ActualWorkingDay) = 1 THEN CAST(u.ActualWorkingDay AS DECIMAL(5, 1))
					ELSE - 1
					END
			FROM dbo.AttendanceUploading AS u WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON u.EmployeeCode = e.EmployeeCode
				AND e.CompanyId = @CompanyId
			WHERE u.SessionId = @AttendanceSessionId
				AND u.RowNumber NOT IN (
					SELECT RowNumber
					FROM #tblRecordError
					WHERE SessionId = @AttendanceSessionId
					)

			--Update status PaymentProcess
			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET IsAttendanceFileSubmitted = 1
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END

		------------------------------------------------
		IF ISNULL(@IsDSTNetSalaryFileSubmitted, 0) = 0
		BEGIN
			--Get DST Net Salary records has error
			INSERT INTO #tblRecordError (
				SessionId
				,RowNumber
				)
			SELECT DISTINCT r.SessionId
				,r.RowNumber
			FROM dbo.DSTNetSalaryUploadingResult AS r WITH (NOLOCK)
			WHERE r.SessionId = @DSTNetSalarySessionId
				AND r.ResultType = 2

			--Update table MonthlyDSTNetSalary
			DELETE s
			FROM dbo.MonthlyDSTNetSalary AS s WITH (ROWLOCK, UPDLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON s.EmployeeId = e.EmployeeId
			WHERE s.[Month] = @Month
				AND e.CompanyId = @CompanyId

			INSERT INTO dbo.MonthlyDSTNetSalary (EmployeeId, [Month], CurrencyId, NetSalaryReceived)
			SELECT e.EmployeeId
				,@Month
				,c.CurrencyId
				,u.NetSalaryReceived
			FROM dbo.DSTNetSalaryUploading AS u WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON u.EmployeeCode = e.EmployeeCode
				AND e.CompanyId = @CompanyId
				INNER JOIN dbo.Currency AS c WITH (NOLOCK) ON u.Currency = c.CurrencyCode
			WHERE u.SessionId = @DSTNetSalarySessionId
				AND u.RowNumber NOT IN (
					SELECT RowNumber
					FROM #tblRecordError
					WHERE SessionId = @DSTNetSalarySessionId
					)

			--Update status PaymentProcess
			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET IsDSTNetSalaryFileSubmitted = 1
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END

		IF ISNULL(@IsAdditionDeductionFileSubmitted, 0) = 0
		BEGIN
			UPDATE dbo.PaymentProcess WITH (ROWLOCK, UPDLOCK)
			SET IsAdditionDeductionFileSubmitted = 1
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END
	END TRY

	BEGIN CATCH
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = 'Failed to retrieve data from Attendance file and DST Net Salary file.'
		END
	END CATCH

	DROP TABLE #tblRecordError
END