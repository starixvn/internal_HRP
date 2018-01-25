
CREATE PROCEDURE [dbo].[JobCategory_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20180116@Tim.Trinh
Task: Get all job category
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT JobCategoryId
		,[Name]
		,[Description]
		,IsActive
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE ISNULL(@IsActive, IsActive) = IsActive
END