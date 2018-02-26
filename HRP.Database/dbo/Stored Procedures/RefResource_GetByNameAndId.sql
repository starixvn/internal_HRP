CREATE PROCEDURE [dbo].RefResource_GetByNameAndId @ResourceId SMALLINT = NULL
	,@RefName VARCHAR(100)
AS
/*
Created: 20180208@Raymond.Le
Task: Get RefResource by id and Name
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT ResourceId
		,RefName
		,Value
		,[Description]
		,IsActive
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName = @RefName
		AND ResourceId = @ResourceId
END
