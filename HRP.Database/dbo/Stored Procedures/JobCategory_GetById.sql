
CREATE PROCEDURE [dbo].[JobCategory_GetById] 
	@JobCategoryId SMALLINT = NULL
AS
/*
Created: 20180207@Raymond.Le
Task: Get JobCategory by id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT JobCategoryId
		,Name
		,[Description]
		,IsActive
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE JobCategoryId = @JobCategoryId

END