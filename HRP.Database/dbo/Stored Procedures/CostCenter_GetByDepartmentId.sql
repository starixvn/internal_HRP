
CREATE PROCEDURE [dbo].[CostCenter_GetByDepartmentId] @DepartmentId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get cost center by department id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT CostCenterId
		,Code
		,[Name]
		,IsActive
	FROM dbo.CostCenter WITH (NOLOCK)
	WHERE DepartmentId = @DepartmentId
		AND ISNULL(@IsActive, IsActive) = IsActive
END