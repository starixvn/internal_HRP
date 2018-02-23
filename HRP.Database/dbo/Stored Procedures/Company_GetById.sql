
CREATE PROCEDURE [dbo].[Company_GetById] 
	@CompanyId SMALLINT = NULL
AS
/*
Created: 20180126@Raymond.Le
Task: Get company BVI by id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CompanyId
		,Code
		,Name
		,IsActive
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @CompanyId

END