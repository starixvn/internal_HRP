-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/25/2018
-- Description:	Check company BVI item existed
-- =============================================
CREATE PROCEDURE [dbo].[Company_CheckExists]
	@CompanyId SMALLINT = 0
	,@Code VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Company
	WHERE Code = @Code
		AND CompanyId <> @CompanyId
END