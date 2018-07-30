
CREATE PROCEDURE [dbo].[MailSetting_Update] @CompanyId SMALLINT = 0
	,@Server NVARCHAR(250) = NULL
	,@Port INT = 0
	,@Username NVARCHAR(100) = NULL
	,@Password NVARCHAR(100) = NULL
	,@IsAuth BIT = 0
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20180726@Tim.Trinh
Task: update mail setting
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.MailSetting WITH (NOLOCK)
			WHERE CompanyId = @CompanyId
			)
	BEGIN
		INSERT INTO dbo.MailSetting (
			CompanyId
			,[Server]
			,[Port]
			,Username
			,[Password]
			,IsAuth
			,[Priority]
			)
		VALUES (
			@CompanyId
			,@Server
			,@Port
			,@Username
			,@Password
			,@IsAuth
			,1 -- normal
			)

		SET @xml = (
				SELECT CompanyId AS '@cid'
					,[Server] AS '@server'
					,[Port] AS '@port'
					,Username AS '@username'
					,[Password] AS '@password'
					,IsAuth AS '@isauth'
				FROM dbo.MailSetting WITH (NOLOCK)
				WHERE CompanyId = @CompanyId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	END
	ELSE
	BEGIN
		SET @xml = (
				SELECT CompanyId AS '@cid'
					,[Server] AS '@server'
					,[Port] AS '@port'
					,Username AS '@username'
					,[Password] AS '@password'
					,IsAuth AS '@isauth'
				FROM dbo.MailSetting WITH (NOLOCK)
				WHERE CompanyId = @CompanyId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

		UPDATE dbo.MailSetting WITH (ROWLOCK, UPDLOCK)
		SET [Server] = @Server
			,[Port] = @Port
			,Username = @Username
			,[Password] = @Password
			,IsAuth = @IsAuth
		WHERE CompanyId = @CompanyId

		SET @xml = (
				SELECT CompanyId AS '@cid'
					,[Server] AS '@server'
					,[Port] AS '@port'
					,Username AS '@username'
					,[Password] AS '@password'
					,IsAuth AS '@isauth'
				FROM dbo.MailSetting WITH (NOLOCK)
				WHERE CompanyId = @CompanyId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	END

	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'MailSetting_Update'
		,@ObjectId = @CompanyId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END