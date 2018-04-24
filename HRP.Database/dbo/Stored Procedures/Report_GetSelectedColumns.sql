
CREATE PROCEDURE [dbo].[Report_GetSelectedColumns] 
	@SelectedIds VARCHAR(MAX)
AS
/*
Created: 20180423@Elton.Ton
Task: Get selected columns
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT drc.Id
	,drc.ItemId
	,drc.HeaderName
	,drc.ColumnName
	,drc.IsEncrypted
	FROM dbo.DynamicReportColumn AS drc WITH (NOLOCK)
	WHERE EXISTS(SELECT 1 FROM dbo.strSplit(@SelectedIds, ',') AS s WHERE drc.ItemId = s.val)
END