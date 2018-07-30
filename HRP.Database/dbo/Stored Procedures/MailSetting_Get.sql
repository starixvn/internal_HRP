
CREATE PROCEDURE [dbo].[MailSetting_Get]
AS
/*
Created: 20180723@Tim.Trinh
Task: Get mail setting
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @DefaultCompany TABLE (
		CompanyId SMALLINT
		,Code NVARCHAR(50)
		,[Name] NVARCHAR(200)
		)

	INSERT INTO @DefaultCompany
	VALUES (
		- 1001
		,'Default'
		,'Default'
		)

	SELECT c.CompanyId
		,c.Code
		,c.[Name]
		,ms.[Server]
		,ms.Port
		,ms.Username
		,ms.[Password]
		,ms.IsAuth
		,ms.[Priority]
	FROM dbo.[Company] AS c WITH (NOLOCK)
		LEFT JOIN dbo.MailSetting AS ms WITH (NOLOCK) ON c.CompanyId = ms.CompanyId
	WHERE IsActive = 1
	
	UNION
	SELECT c.CompanyId
		,c.Code
		,c.[Name]
		,ms.[Server]
		,ms.Port
		,ms.Username
		,ms.[Password]
		,ms.IsAuth
		,ms.[Priority]
	FROM @DefaultCompany AS c
		LEFT JOIN dbo.MailSetting AS ms WITH (NOLOCK) ON c.CompanyId = ms.CompanyId
END