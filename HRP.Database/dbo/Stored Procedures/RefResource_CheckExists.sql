CREATE PROCEDURE [dbo].[RefResource_CheckExists] @ResourceId SMALLINT = 0
	,@RefName VARCHAR(100)
	,@Value NVARCHAR(50)
AS
/*
Created: 20180208@Raymond.Le
Task: Check RefResource item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = @RefName
		AND Value = @Value
		AND ResourceId <> @ResourceId
END
