-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/25/2018
-- Description:	Check company BVI item existed
-- =============================================
CREATE PROCEDURE [DataConfiguration_CheckCompanyBVIExists]
	@Code VARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Company
	WHERE Code = @Code
END