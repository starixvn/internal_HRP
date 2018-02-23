
CREATE PROCEDURE [dbo].[Section_CheckExists]
	@SectionId SMALLINT = 0
	,@CostCenterId SMALLINT = 0
	,@Code NVARCHAR(50)
AS
/*
Created: 20180201@Raymond.Le
Task: Check Section item exists
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
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