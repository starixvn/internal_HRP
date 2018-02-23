
CREATE PROCEDURE [dbo].[Company_CheckExists]
	@CompanyId SMALLINT = 0
	,@Code NVARCHAR(50)
AS
/*
Created: 20180125@Raymond.Le
Task: Check company BVI item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Company WITH (NOLOCK)
	WHERE Code = @Code
		AND CompanyId <> @CompanyId
END