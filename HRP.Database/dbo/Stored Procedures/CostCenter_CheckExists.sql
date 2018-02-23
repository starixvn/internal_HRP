
CREATE PROCEDURE [dbo].[CostCenter_CheckExists]
	@CostCenterId SMALLINT = 0
	,@DepartmentId SMALLINT = 0 
	,@Code NVARCHAR(50)
AS
/*
Created: 20180201@Raymond.Le
Task: Check cost center item exists
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF (@CostCenterId > 0)
	BEGIN
		SELECT TOP 1 @DepartmentId= DepartmentId
		FROM dbo.CostCenter WITH (NOLOCK) 
		WHERE CostCenterId = @CostCenterId
	END

	SELECT 1
	FROM dbo.CostCenter WITH (NOLOCK)
	WHERE Code = @Code
		AND DepartmentId = @DepartmentId
		AND CostCenterId <> @CostCenterId
END