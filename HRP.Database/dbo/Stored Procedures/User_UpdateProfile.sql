

CREATE PROCEDURE [dbo].[User_UpdateProfile] @UserId INT
	,@FullName VARCHAR(70)
	,@Phone VARCHAR(20)
	,@Email VARCHAR(250)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20171201@Oliver.Dang
Task: Update User profile.
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND FullName = @FullName
				AND Phone = @Phone
				AND Email = @Email
			)
	BEGIN
		RETURN
	END

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@xml XML

	SET @startRun = GETDATE()
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.FullName AS '@f'
				,u.Phone AS '@p'
				,u.Email AS '@e'
				,u.IsActive AS '@i'
			FROM dbo.[User] u WITH (NOLOCK)
			WHERE u.UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].[User] WITH (ROWLOCK, UPDLOCK)
	SET FullName = @FullName
		,Phone = @Phone
		,Email = @Email
	WHERE UserId = @UserId

	-- write log after update
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.FullName AS '@f'
				,u.Phone AS '@p'
				,u.Email AS '@e'
			FROM dbo.[User] AS u WITH (NOLOCK)
			WHERE u.UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UserId
		,@SPName = 'User_UpdateProfile'
		,@ObjectId = @UserId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END