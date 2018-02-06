-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/01/2018
-- Description:	Check Section item exists
-- =============================================
CREATE PROCEDURE [dbo].[Section_CheckExists]
	@SectionId SMALLINT = 0
	,@CostCenterId SMALLINT = 0
	,@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@SectionId > 0)
	BEGIN
		SELECT TOP 1 @CostCenterId= CostCenterId
		FROM dbo.Section WITH (NOLOCK) 
		WHERE SectionId = @SectionId
	END

	SELECT 1
	FROM dbo.Section WITH (NOLOCK)
	WHERE Code = @Code
		AND CostCenterId = @CostCenterId
		AND SectionId <> @SectionId
END