SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[LogDetail_Insert] @UserId INT
	,@SPName NVARCHAR(50)
	,@ObjectId INT = NULL
	,@ObjectTypeId INT = NULL
	,@Data NVARCHAR(MAX) = NULL
	,@OldData NVARCHAR(MAX) = NULL
	,@Duration INT = NULL
AS
/*
Created: 20171208@Tim.Trinh
Task: write log
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
    
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @actionId SMALLINT

	SELECT @actionId = ActionId
	FROM dbo.LogAction WITH (NOLOCK)
	WHERE SPName = @SPName

	INSERT INTO dbo.LogDetail (
		UserId
		,ActionId
		,ObjectId
		,ObjectTypeId
		,LogDate
		,Duration
		,Data
		,OldData
		)
	VALUES (
		@UserId
		,@actionId
		,@ObjectId
		,@ObjectTypeId
		,GETDATE()
		,@Duration
		,@Data
		,@OldData
		)
END

GO


