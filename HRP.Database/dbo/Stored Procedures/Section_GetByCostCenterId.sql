
CREATE PROCEDURE [dbo].[Section_GetByCostCenterId] @CostCenterId SMALLINT
	,@IsActive BIT = NULL
AS
/*
Created: 20180104@Tim.Trinh
Task: Get section by cost center id
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT SectionId
		,Code
		,[Name]
		,IsActive
	FROM dbo.Section WITH (NOLOCK)
	WHERE CostCenterId = @CostCenterId
		AND ISNULL(@IsActive, IsActive) = IsActive
END