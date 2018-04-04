
CREATE PROCEDURE [dbo].[PaymentProcess_GetUploadingFilesResult] @CompanyId SMALLINT
	,@Month DATE
AS
/*
Created: 20180308@Raymond.Le
Task: Get uploading files result
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @startDate DATE
		,@dueDate DATE

	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@Month)

	--get payment process info
	SELECT p.CompanyId
		,c.Code AS CompanyCode
		,p.[Month]
		,p.AttendanceFileName
		,(
			SELECT Count(1)
			FROM dbo.AttendanceUploading AS a WITH (NOLOCK)
			WHERE a.SessionId = p.AttendanceSessionId
			) AS AttendanceTotalRow
		,p.DSTNetSalaryFileName
		,(
			SELECT Count(1)
			FROM dbo.DSTNetSalaryUploading AS d WITH (NOLOCK)
			WHERE d.SessionId = p.DSTNetSalarySessionId
			) AS DSTNetSalaryTotalRow
		,@startDate AS StartDate
		,@dueDate AS DueDate
	FROM dbo.PaymentProcess AS p WITH (NOLOCK)
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON p.CompanyId = c.CompanyId
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month
		AND c.IsActive = 1

	-- get Attendance validation result
	SELECT r.RowNumber
		,r.[Message]
	FROM dbo.AttendanceUploadingResult AS r WITH (NOLOCK)
	LEFT JOIN dbo.PaymentProcess AS p WITH (NOLOCK) ON r.SessionId = p.AttendanceSessionId
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month

	-- get DST Net salary validation result
	SELECT r.RowNumber
		,r.[Message]
	FROM dbo.DSTNetSalaryUploadingResult AS r WITH (NOLOCK)
	LEFT JOIN dbo.PaymentProcess AS p WITH (NOLOCK) ON r.SessionId = p.DSTNetSalarySessionId
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month
END