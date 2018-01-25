
CREATE PROCEDURE [dbo].[Employee_CheckEmpCodeExisted] @EmployeeId INT
	,@CompanyCode VARCHAR(10)
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
	FROM dbo.Employment WITH (NOLOCK)
	WHERE EmployeeId <> @EmployeeId
		AND CompanyCode = @CompanyCode
		AND EmployeeCode = @EmployeeCode
END