-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Check cost center item exists
-- =============================================
Create PROCEDURE [dbo].[CostCenter_CheckExists]
	@CostCenterId SMALLINT = 0
	,@DepartmentId SMALLINT = 0 
	,@Code VARCHAR(50)
AS
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