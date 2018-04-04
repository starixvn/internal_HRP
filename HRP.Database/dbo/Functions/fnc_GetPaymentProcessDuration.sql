
CREATE FUNCTION [dbo].[fnc_GetPaymentProcessDuration] (@month DATE)
RETURNS @processDuration TABLE (
	StartDate DATE
	,DueDate DATE
	)
AS
/*
Created: 20180315@Tim.Trinh
Task : get payment process duration
DB: bodb_HRP

Param's Explanation:

Revisions:
*/
BEGIN
	DECLARE @startDate DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @month), 0)
		,@dueDate DATE = DATEADD(MONTH, DATEDIFF(MONTH, 0, @month), 30)

	INSERT INTO @processDuration (
		StartDate
		,DueDate
		)
	VALUES (
		@startDate
		,@dueDate
		)

	RETURN
END