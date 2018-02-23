
CREATE PROCEDURE [dbo].[Division_GetById] @DivisionId SMALLINT
AS
/*
Created: 20180131@Raymond.Le
Task: Get division by divisionId
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DivisionId
		,d.Code
		,d.[Name]
		,d.IsActive
		,d.CompanyId
		,c.Code AS CompanyCode
	FROM dbo.Division AS d WITH (NOLOCK)
	LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON d.CompanyId = c.CompanyId
	WHERE DivisionId = @DivisionId
END