-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/30/2018
-- Description:	Check division item existed
-- =============================================
CREATE PROCEDURE [dbo].[Division_CheckExists]
	@DivisionId SMALLINT = 0
	,@CompanyId SMALLINT = 0 
	,@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@DivisionId > 0)
	BEGIN
		SELECT TOP 1 @CompanyId= CompanyId 
		FROM dbo.Division WITH (NOLOCK) 
		WHERE DivisionId = @DivisionId
	END

	SELECT 1
	FROM dbo.Division WITH (NOLOCK)
	WHERE Code = @Code
		AND CompanyId = @CompanyId
		AND DivisionId <> @DivisionId
END