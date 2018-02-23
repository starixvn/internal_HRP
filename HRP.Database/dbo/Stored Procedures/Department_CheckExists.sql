
CREATE PROCEDURE [dbo].[Department_CheckExists]
	@DepartmentId SMALLINT = 0
	,@DivisionId SMALLINT = 0 
	,@Code NVARCHAR(50)
AS
/*
Created: 20180131@Raymond.Le
Task: Check department item exists
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@DepartmentId > 0)
	BEGIN
		SELECT TOP 1 @DivisionId= DivisionId
		FROM dbo.Department WITH (NOLOCK) 
		WHERE DepartmentId = @DepartmentId
	END

	SELECT 1
	FROM dbo.Department WITH (NOLOCK)
	WHERE Code = @Code
		AND DivisionId = @DivisionId
		AND DepartmentId <> @DepartmentId
END