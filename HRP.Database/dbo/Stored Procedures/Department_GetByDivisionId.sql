
CREATE PROCEDURE [dbo].[Department_GetByDivisionId] @DivisionId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get department by divisionid
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT DepartmentId
		,Code
		,[Name]
		,IsActive
	FROM dbo.Department WITH (NOLOCK)
	WHERE DivisionId = @DivisionId
		AND ISNULL(@IsActive, IsActive) = IsActive
END