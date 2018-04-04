
CREATE PROCEDURE [dbo].[PaymentProcess_DeleteUploadedDSTNetSalary] @CompanyId SMALLINT
	,@Month DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180315@Raymond.Le
Task: Delete uploaded DSTNetSalary file
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
	SET IsDSTNetSalaryFileSubmited = 0
		,DSTNetSalarySessionId = NULL
		,DSTNetSalaryFileName = NULL
		,IsPayrollGenerated = 0
	WHERE CompanyId = @CompanyId
		AND [Month] = @Month
END