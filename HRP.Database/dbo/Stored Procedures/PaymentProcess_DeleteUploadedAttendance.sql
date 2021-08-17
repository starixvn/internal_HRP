
CREATE PROCEDURE [dbo].[PaymentProcess_DeleteUploadedAttendance] @CompanyId SMALLINT
	,@Month DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180315@Raymond.Le
Task: Delete uploaded Attendance file
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	--Update PaymentProcess
	UPDATE dbo.PaymentProcess
	WITH (ROWLOCK,UPDLOCK)
	SET IsAttendanceFileSubmitted = 0
		,AttendanceSessionId = NULL
		,AttendanceFileName = NULL
		,IsPayrollGenerated = 0
	WHERE CompanyId = @CompanyId
		AND [Month] = @Month
END