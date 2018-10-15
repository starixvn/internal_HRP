
CREATE PROCEDURE [dbo].[MailSetting_GetForSendMail] @CompanyId SMALLINT
AS
/*
Created: 20180726@Tim.Trinh
Task: Get mail setting for send mail
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DefaultId SMALLINT = - 1001

	IF EXISTS (
			SELECT 1
			FROM dbo.MailSetting WITH (NOLOCK)
			WHERE CompanyId = @CompanyId
			AND CompanyId > 0
			)
	BEGIN
		SELECT c.CompanyId
			,ms.[Server]
			,ms.Port
			,ms.Username
			,ms.[Password]
			,ms.IsAuth
			,ms.[Priority]
		FROM dbo.[Company] AS c WITH (NOLOCK)
			INNER JOIN dbo.MailSetting AS ms WITH (NOLOCK) ON c.CompanyId = ms.CompanyId
		WHERE c.IsActive = 1
			AND c.CompanyId = @CompanyId
	END
	ELSE
	BEGIN
		SELECT @DefaultId AS CompanyId
			,ms.[Server]
			,ms.Port
			,ms.Username
			,ms.[Password]
			,ms.IsAuth
			,ms.[Priority]
		FROM dbo.MailSetting AS ms WITH (NOLOCK)
		WHERE ms.CompanyId = @DefaultId
	END
END