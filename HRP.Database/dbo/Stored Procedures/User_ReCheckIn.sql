
CREATE PROCEDURE [dbo].[User_ReCheckIn] @OnUserId INT
AS
/*
Created: 20171221@Tim.Trinh
Task: User recheckin
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @IsKickedOut BIT

	IF EXISTS (
			SELECT 1
			FROM dbo.OnUser WITH (NOLOCK)
			WHERE Id = @OnUserId
				AND IsKickedOut = 1
			)
	BEGIN
		SET @IsKickedOut = 1
	END
	ELSE
	BEGIN
		SET @IsKickedOut = 0

		UPDATE dbo.OnUser WITH (ROWLOCK, UPDLOCK)
		SET ReloadTime = GETDATE()
		WHERE Id = @OnUserId

		IF @@ROWCOUNT = 0
		BEGIN
			SET @IsKickedOut = 1
		END
	END
	
	SELECT @IsKickedOut
END