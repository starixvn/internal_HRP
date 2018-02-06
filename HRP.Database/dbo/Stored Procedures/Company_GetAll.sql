
CREATE PROCEDURE [dbo].[Company_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20171205@Tim.Trinh
Task: Get all company
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT CompanyId
		,Code
		,[Name]
		,IsActive
	FROM dbo.[Company] WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
	ORDER BY IsActive DESC
		,Code 
END