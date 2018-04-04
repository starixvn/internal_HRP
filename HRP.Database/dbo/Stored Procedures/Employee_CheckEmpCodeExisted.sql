
CREATE PROCEDURE [dbo].[Employee_CheckEmpCodeExisted] @EmployeeId INT
	,@CompanyCode NVARCHAR(50)
	,@EmployeeCode VARCHAR(15)
AS
/*
 Created: 20180103@Tim.Trinh
 Task : Check emp code existed
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 
 */
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.Employment AS em WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
	WHERE em.EmployeeId <> @EmployeeId
		AND c.Code = @CompanyCode
		AND em.EmployeeCode = @EmployeeCode
END