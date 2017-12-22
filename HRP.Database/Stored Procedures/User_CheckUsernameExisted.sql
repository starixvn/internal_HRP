SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_CheckUsernameExisted] @Username VARCHAR(30)
AS
/*
 Created: 20171205@EltonTon
 Task : Check username is existed
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 
 */
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.[User] WITH (NOLOCK)
	WHERE Username = @Username
END

GO


