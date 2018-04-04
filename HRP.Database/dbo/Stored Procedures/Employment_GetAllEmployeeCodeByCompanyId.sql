
CREATE PROCEDURE [dbo].[Employment_GetAllEmployeeCodeByCompanyId] @CompanyId INT 
AS
/*
 Created: 20180307@Raymond.Le
 Task : Get all employee code by Company
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 
 */
BEGIN
	SET NOCOUNT ON;

	SELECT EmployeeCode
	FROM dbo.Employment WITH (NOLOCK)
	WHERE CompanyId = @CompanyId
END