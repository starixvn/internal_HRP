
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
		,@IsAttendanceFileSubmited BIT
		,@IsDSTNetSalaryFileSubmited BIT

	CREATE TABLE #RecordError (
		SessionId NVARCHAR(100)
		,RowNumber INT
		)

	SELECT @AttendanceSessionId = p.AttendanceSessionId
		,@IsAttendanceFileSubmited = p.IsAttendanceFileSubmited
		,@DSTNetSalarySessionId = p.DSTNetSalarySessionId
		,@IsDSTNetSalaryFileSubmited = p.IsDSTNetSalaryFileSubmited
	FROM dbo.PaymentProcess AS p WITH (NOLOCK)
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month

	BEGIN TRY	
		--Check if has Attendance file is uploading
		IF (ISNULL(@IsAttendanceFileSubmited, 0) = 0)
		BEGIN
			--DELETE data from MonthlyAttendance
			DELETE a
			FROM dbo.MonthlyAttendance AS a WITH (NOLOCK)
			LEFT JOIN dbo.Employment AS e WITH (NOLOCK) ON a.EmployeeId = e.EmployeeId
			WHERE a.[Month] = @Month
			   AND e.CompanyId = @CompanyId

			--Get Attendance records has error
			INSERT INTO #RecordError (
				SessionId
				,RowNumber
				)
			SELECT DISTINCT r.SessionId
				,r.RowNumber
			FROM dbo.AttendanceUploadingResult AS r WITH (NOLOCK)
			WHERE r.SessionId = @AttendanceSessionId
				AND r.ResultType = 2

			--Update table MonthlyAttendance
			MERGE INTO dbo.MonthlyAttendance AS a
			USING (
				SELECT e.EmployeeId
					,@Month
					,CASE 
						WHEN ISNUMERIC(u.ActualWorkingDay) = 1 THEN CAST(u.ActualWorkingDay AS DECIMAL(5,1))
						ELSE - 1
					END
				FROM dbo.AttendanceUploading AS u WITH (NOLOCK)
				LEFT JOIN dbo.Employment AS e WITH (NOLOCK) ON u.EmployeeCode = e.EmployeeCode
				WHERE u.SessionId = @AttendanceSessionId
					AND u.RowNumber NOT IN (
						SELECT RowNumber
						FROM #RecordError
						WHERE SessionId = @AttendanceSessionId
						)
				) AS val(EmployeeId, [Month], ActualWorkingDay)
				ON a.EmployeeId = val.EmployeeId
					AND a.[Month] = val.[Month]
			WHEN MATCHED
				THEN
					UPDATE
					SET a.ActualWorkingDay = val.ActualWorkingDay
			WHEN NOT MATCHED
				THEN
					INSERT (
						EmployeeId
						,[Month]
						,ActualWorkingDay
						)
					VALUES (
						val.EmployeeId
						,val.[Month]
						,val.ActualWorkingDay
						);

			--Update status PaymentProcess
			UPDATE dbo.PaymentProcess	WITH (ROWLOCK,UPDLOCK)
			SET IsAttendanceFileSubmited = 1
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
		END

		------------------------------------------------
		IF (ISNULL(@IsDSTNetSalaryFileSubmited, 0) = 0)
		BEGIN
			--DELETE data from MonthlyDSTNetSalary
			DELETE s
			FROM dbo.MonthlyDSTNetSalary AS s WITH (NOLOCK)
			LEFT JOIN dbo.Employment AS e WITH (NOLOCK) ON s.EmployeeId = e.EmployeeId
			WHERE s.[Month] = @Month
			   AND e.CompanyId = @CompanyId

			--Get DST Net Salary records has error
			INSERT INTO #RecordError (
				SessionId
				,RowNumber
				)
			SELECT DISTINCT r.SessionId
				,r.RowNumber
			FROM dbo.DSTNetSalaryUploadingResult AS r WITH (NOLOCK)
			WHERE r.SessionId = @DSTNetSalarySessionId
				AND r.ResultType = 2

			--Update table MonthlyDSTNetSalary
			MERGE INTO dbo.MonthlyDSTNetSalary AS d
			USING (
				SELECT e.EmployeeId
					,@Month
					,c.CurrencyId
					,u.NetSalaryReceived
				FROM dbo.DSTNetSalaryUploading AS u WITH (NOLOCK)
				LEFT JOIN dbo.Employment AS e WITH (NOLOCK) ON u.EmployeeCode = e.EmployeeCode
				LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON u.Currency = c.CurrencyCode
				WHERE u.SessionId = @DSTNetSalarySessionId
					AND u.RowNumber NOT IN (
						SELECT RowNumber
						FROM #RecordError
						WHERE SessionId = @DSTNetSalarySessionId
						)
				) AS val(EmployeeId, [Month], CurrencyId, NetSalaryReceived)
				ON d.EmployeeId = val.EmployeeId
					AND d.[Month] = val.[Month]
			WHEN MATCHED
				THEN
					UPDATE
					SET d.CurrencyId = val.CurrencyId
						,d.NetSalaryReceived = val.NetSalaryReceived
			WHEN NOT MATCHED
				THEN
					INSERT (
						EmployeeId
						,[Month]
						,CurrencyId
						,NetSalaryReceived
						)
					VALUES (
						val.EmployeeId
						,val.[Month]
						,val.CurrencyId
						,val.NetSalaryReceived
						);

			--Update status PaymentProcess
			UPDATE dbo.PaymentProcess	WITH (ROWLOCK,UPDLOCK)
			SET IsDSTNetSalaryFileSubmited = 1
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

	DROP TABLE #RecordError
END