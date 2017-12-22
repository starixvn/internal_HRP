SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_UpdateUserOTPStatus] @UserId INT
	,@IsOTPRegistered BIT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
 Created: 20171211@Hunter.Tran
 Task : Update User OTP Registered status
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 */
BEGIN
	SET NOCOUNT ON

	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET IsOTPRegistered = @IsOTPRegistered
	WHERE UserId = @UserId
END

GO


