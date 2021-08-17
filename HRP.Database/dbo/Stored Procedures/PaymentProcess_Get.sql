
CREATE PROCEDURE [dbo].[PaymentProcess_Get] @CompanyId SMALLINT
	,@Month DATE
AS
/*
Created: 20180316@Raymond.Le
Task: Get payment process info
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startDate DATE
		,@dueDate DATE

	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@Month)

	-- Check exists data
	IF NOT EXISTS (
			SELECT 1
			FROM dbo.PaymentProcess WITH (NOLOCK)
			WHERE CompanyId = @CompanyId
				AND [Month] = @Month
			)
	BEGIN
		INSERT INTO dbo.PaymentProcess (
			CompanyId
			,[Month]
			,IsPayrollGenerated
			,IsAttendanceFileSubmitted
			,IsDSTNetSalaryFileSubmitted
			,IsAdditionDeductionFileSubmitted
			)
		VALUES (
			@CompanyId
			,@Month
			,0
			,1
			,1
			,1
			)
	END

	SELECT p.CompanyId
		,p.[Month]
		,p.AttendanceSessionId
		,p.AttendanceFileName
		,p.IsAttendanceFileSubmitted
		,p.DSTNetSalarySessionId
		,p.DSTNetSalaryFileName
		,p.IsDSTNetSalaryFileSubmitted
		,p.IsPayrollGenerated
		,p.AdditionDeductionFileName
		,p.AdditionDeductionSessionId
		,p.IsAdditionDeductionFileSubmitted
		,@startDate AS StartDate
		,@dueDate AS DueDate
		,CASE 
			WHEN EXISTS (
					SELECT 1
					FROM dbo.CurrencyExchangeRate AS r WITH (NOLOCK)
					WHERE r.ExchangeRateMonth = @Month
					)
				THEN 1
			ELSE 0
			END AS IsHasCurrencySetting
	FROM dbo.PaymentProcess AS p WITH (NOLOCK)
	WHERE p.CompanyId = @CompanyId
		AND p.[Month] = @Month
END