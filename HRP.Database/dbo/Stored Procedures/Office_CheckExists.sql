-- =============================================
-- Author:		Raymond.Le
-- Create date: 02/07/2018
-- Description:	Check Office item exists
-- =============================================
Create PROCEDURE [dbo].Office_CheckExists
	@OfficeId SMALLINT = 0
	,@CityId SMALLINT = 0 
	,@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@OfficeId > 0)
	BEGIN
		SELECT TOP 1 @CityId= CityId
		FROM dbo.Office WITH (NOLOCK) 
		WHERE OfficeId = @OfficeId
	END

	SELECT 1
	FROM dbo.Office WITH (NOLOCK)
	WHERE Code = @Code
		AND CityId = @CityId
		AND OfficeId <> @OfficeId
END