
CREATE PROCEDURE [dbo].[JobCategory_CheckExists]
	@JobCategoryId SMALLINT = 0
	,@Name NVARCHAR(50)
AS
/*
Created: 20180207@Raymond.Le
Task: Check JobCategory item existed
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE Name = @Name
		AND JobCategoryId <> @JobCategoryId
END