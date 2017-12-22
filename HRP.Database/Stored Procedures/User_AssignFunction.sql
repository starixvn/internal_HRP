SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_AssignFunction] @UserId INT
	,@Value VARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171205@Tim.Trinh
Task: Assign function to user
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
    Value : '<ROOT><Row FunctionId="4" CompanyId="1" IsRead="true" IsHost="true"></Row></ROOT>'
	
WRITE LOG template:
    <root><xml path="r" />
		<row item="UserId" alias="u" />
		<row item="FunctionId" alias="f" />
		<row item="CompanyId" alias="c" />
		<row item="IsRead" alias="ir" />
		<row item="IsHost" alias="ih" />
	 </root>
*/
BEGIN
	SET NOCOUNT ON

	-- For write log
	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	SET @startRun = GETDATE()
	-- Write log before assign permission	
	SET @xml = (
			SELECT uf.UserId AS '@u'
				,uf.FunctionId AS '@f'
				,uf.CompanyId AS '@c'
				,uf.IsRead AS '@ir'
				,uf.IsHost AS '@ih'
			FROM dbo.UserFunction AS uf WITH (NOLOCK)
			WHERE uf.UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	CREATE TABLE #tblFunctionPolicy (
		FunctionId SMALLINT
		,CompanyId SMALLINT
		,IsRead BIT
		,IsHost BIT
		)

	EXEC sp_xml_preparedocument @idoc OUTPUT
		,@Value

	INSERT INTO #tblFunctionPolicy (
		FunctionId
		,CompanyId
		,IsRead
		,IsHost
		)
	SELECT FunctionId
		,CompanyId
		,IsRead
		,IsHost
	FROM OPENXML(@idoc, '/Root/Row', 1) WITH (FunctionId SMALLINT, CompanyId SMALLINT, IsRead BIT, IsHost BIT) AS data

	DECLARE @totalOldRecord INT
		,@totalNewRecord INT

	SELECT @totalOldRecord = COUNT(1)
	FROM dbo.UserFunction WITH (NOLOCK)

	SELECT @totalNewRecord = COUNT(1)
	FROM #tblFunctionPolicy WITH (NOLOCK)
	WHERE IsRead = 1
		OR IsHost = 1

	IF @totalOldRecord = @totalNewRecord
		AND (
			SELECT COUNT(1)
			FROM dbo.UserFunction AS us WITH (NOLOCK)
				INNER JOIN #tblFunctionPolicy AS tbl WITH (NOLOCK) ON us.FunctionId = tbl.FunctionId
				AND us.CompanyId = tbl.CompanyId
				AND us.IsRead = tbl.IsRead
				AND us.IsHost = tbl.IsHost
			WHERE us.UserId = @UserId
			) = @totalOldRecord
	BEGIN
		RETURN
	END

	-- Delete function not set to user
	DELETE uf
	FROM dbo.UserFunction AS uf
		INNER JOIN #tblFunctionPolicy AS tbl WITH (NOLOCK) ON uf.FunctionId = tbl.FunctionId
		AND uf.CompanyId = tbl.CompanyId
	WHERE uf.UserId = @UserId
		AND tbl.IsRead = 0
		AND tbl.IsHost = 0

	-- Update policy for function of user
	UPDATE uf
	SET uf.IsRead = tbl.IsRead
		,uf.IsHost = tbl.IsHost
	FROM dbo.UserFunction AS uf WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblFunctionPolicy AS tbl WITH (NOLOCK) ON uf.FunctionId = tbl.FunctionId
		AND uf.CompanyId = tbl.CompanyId
	WHERE uf.UserId = @UserId
		AND (
			tbl.IsRead = 1
			OR tbl.IsHost = 1
			)

	--Insert new policy for function of user
	INSERT INTO dbo.UserFunction (
		UserId
		,FunctionId
		,CompanyId
		,IsRead
		,IsHost
		)
	SELECT @UserId
		,tbl.FunctionId
		,tbl.CompanyId
		,tbl.IsRead
		,tbl.IsHost
	FROM #tblFunctionPolicy AS tbl WITH (NOLOCK)
	WHERE (
			tbl.IsRead = 1
			OR tbl.IsHost = 1
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.UserFunction AS uf WITH (NOLOCK)
			WHERE uf.UserId = @UserId
				AND uf.FunctionId = tbl.FunctionId
				AND uf.CompanyId = tbl.CompanyId
			)

	-- write log after update
	SET @xml = (
			SELECT uf.UserId AS '@u'
				,uf.FunctionId AS '@f'
				,uf.CompanyId AS '@c'
				,uf.IsRead AS '@ir'
				,uf.IsHost AS '@ih'
			FROM dbo.UserFunction AS uf WITH (NOLOCK)
			WHERE uf.UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @ModifiedBy
		,@SPName = 'User_AssignFunction'
		,@ObjectId = @UserId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration

	DROP TABLE #tblFunctionPolicy
END

GO


