
CREATE PROCEDURE [dbo].[MailError_Create] @MailTo VARCHAR(254)
	,@Subject NVARCHAR(200) = NULL
	,@ErrorMessage NVARCHAR(1000) = NULL
	,@StackTrace NVARCHAR(2000) = NULL
AS
/*
Created: 20180524@Tim.Trinh
Task: write log send mail error
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.MailError (
		MailTo
		,[Subject]
		,ErrorMessage
		,StackTrace
		,ErrorTime
		)
	VALUES (
		@MailTo
		,@Subject
		,@ErrorMessage
		,@StackTrace
		,GETDATE()
		)
END