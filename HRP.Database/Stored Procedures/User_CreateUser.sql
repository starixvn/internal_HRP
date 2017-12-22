SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[User_CreateUser] @Username VARCHAR(30)
	,@FullName NVARCHAR(70) = NULL
	,@Phone VARCHAR(20) = NULL
	,@Email VARCHAR(254)
	,@Password VARCHAR(200)
	,@IsActive BIT
	,@CreatedBy INT
	,@UserId INT OUTPUT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
 Created: 20171205@EltonTon
 Task : Create new user
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 
 */
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	SET @startRun = GETDATE()

	INSERT INTO [dbo].[User] (
		Username
		,FullName
		,Phone
		,Email
		,[Password]
		,IsActive
		,CreatedBy
		,CreatedDate
		,IsOTPRegistered
		)
	VALUES (
		@Username
		,@FullName
		,@Phone
		,@Email
		,@Password
		,@IsActive
		,@CreatedBy
		,GETDATE()
		,0
		)

	SET @UserId = SCOPE_IDENTITY()
	SET @xml = (
			SELECT u.UserId AS '@u'
				,u.Username AS '@un'
				,u.Password AS '@p'
				,u.FullName AS '@f'
				,u.Phone AS '@ph'
				,u.Email AS '@e'
				,u.IsActive AS '@i'
				,u.CreatedBy AS '@cb'
				,u.CreatedDate AS '@cd'
			FROM dbo.[User] AS u WITH (NOLOCK)
			WHERE u.UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'User_CreateUser'
		,@ObjectId = @UserId
		,@ObjectTypeId = 2
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END

GO


