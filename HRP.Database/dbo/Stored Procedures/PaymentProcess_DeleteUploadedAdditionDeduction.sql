
CREATE PROCEDURE [dbo].[PaymentProcess_DeleteUploadedAdditionDeduction] @CompanyId SMALLINT
	,@Month DATE
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20210810@Tim.Trinh
Task: delete uploaded addition/deduction file
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SessionId NVARCHAR(100)

	SELECT @SessionId = AdditionDeductionSessionId
	FROM dbo.PaymentProcess WITH (NOLOCK)
	WHERE CompanyId = @CompanyId
		AND [Month] = @Month

	DELETE
	FROM dbo.AdditionDeductionUploading WITH (ROWLOCK, UPDLOCK)
	WHERE SessionId = @SessionId

	DELETE
	FROM dbo.AdditionDeductionUploadingResult WITH (ROWLOCK, UPDLOCK)
	WHERE SessionId = @SessionId

	UPDATE dbo.PaymentProcess WITH (ROWLOCK,UPDLOCK)
	SET IsAdditionDeductionFileSubmitted = 0
		,AdditionDeductionSessionId = NULL
		,AdditionDeductionFileName = NULL
		,IsPayrollGenerated = 0
	WHERE CompanyId = @CompanyId
		AND [Month] = @Month
END