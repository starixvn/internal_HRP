
CREATE PROCEDURE [dbo].[Division_GetByCompanyId] @CompanyId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get division by companyid
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT DivisionId
		,Code
		,[Name]
		,IsActive
	FROM dbo.Division WITH (NOLOCK)
	WHERE CompanyId = @CompanyId
		AND ISNULL(@IsActive, IsActive) = IsActive
END