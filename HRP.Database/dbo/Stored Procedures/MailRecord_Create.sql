
CREATE PROCEDURE [dbo].[MailRecord_Create] @MailTo VARCHAR(254)
	,@Subject NVARCHAR(200) = NULL
	,@SentTime DATETIME = NULL
	,@IsSuccessful BIT = NULL
	,@ErrorMessage NVARCHAR(1000) = NULL
	,@StackTrace NVARCHAR(2000) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180524@Tim.Trinh
Task: write log send mail
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.MailRecord (
		MailTo
		,[Subject]
		,SentTime
		,IsSuccessful
		,ErrorMessage
		,StackTrace
		)
	VALUES (
		@MailTo
		,@Subject
		,@SentTime
		,@IsSuccessful
		,@ErrorMessage
		,@StackTrace
		)
END