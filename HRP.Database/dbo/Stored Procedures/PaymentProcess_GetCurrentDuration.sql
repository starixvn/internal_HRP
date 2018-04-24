
CREATE PROCEDURE [dbo].[PaymentProcess_GetCurrentDuration] 
AS
/*
Created: 20180328@Raymond.Le
Task: Current month payment process duration
DB: bodb_HRP
Original:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @firstDayOfCurrentMonth DATE
		,@firstDayOfLastMonth DATE
		,@currentStartDate DATE
		,@currentDueDate DATE
		,@lastStartDate DATE
		,@lastDueDate DATE
		,@today DATE = GETDATE()

		 
	SET @firstDayOfCurrentMonth = DATEADD(MONTH, DATEDIFF(MONTH, 0, @today), 0)
	SET @firstDayOfLastMonth = DATEADD(MONTH, - 1, @firstDayOfCurrentMonth)

	SELECT @currentStartDate = StartDate
		,@currentDueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@firstDayOfCurrentMonth)

	SELECT @lastStartDate = StartDate
		,@lastDueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@firstDayOfLastMonth)

	IF (
			@today BETWEEN @lastStartDate
				AND @lastDueDate
			)
	BEGIN
		SELECT @firstDayOfLastMonth AS [Month]
			,@lastStartDate AS StartDate
			,@lastDueDate AS DueDate
	END
	ELSE
	BEGIN
		SELECT @firstDayOfCurrentMonth AS [Month]
			,@currentStartDate AS StartDate
			,@currentDueDate AS DueDate
	END
END