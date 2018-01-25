
CREATE PROCEDURE [dbo].[RefResource_GetByNames] @RefNames VARCHAR(4000)
AS
/*
Created: 20171228@Tim.Trinh
Task: Get reference resource by names
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblRefName (RefName VARCHAR(100))

	IF @RefNames IS NOT NULL
	BEGIN
		INSERT INTO #tblRefName (RefName)
		SELECT val
		FROM dbo.strSplit(@RefNames, ',')
	END

	SELECT ref.ResourceId
		,ref.RefName
		,ref.Value
		,ref.[Description]
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		INNER JOIN #tblRefName AS tbl WITH (NOLOCK) ON ref.RefName = tbl.RefName

	DROP TABLE #tblRefName
END