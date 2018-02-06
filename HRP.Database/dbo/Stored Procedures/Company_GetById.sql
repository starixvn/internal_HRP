-- =============================================
-- Author:		Raymond.Le
-- Create date: 01/26/2018
-- Description:	Get company BVI by id
-- =============================================
CREATE PROCEDURE [dbo].[Company_GetById] 
	@CompanyId SMALLINT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT CompanyId
		,Code
		,Name
		,IsActive
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @CompanyId

END