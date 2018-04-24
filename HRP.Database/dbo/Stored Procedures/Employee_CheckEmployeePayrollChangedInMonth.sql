
CREATE PROCEDURE [dbo].[Employee_CheckEmployeePayrollChangedInMonth] @Month DATE
	,@EmployeeId INT
AS
/*
 Created: 20180406@EltonTon
 Task : Check employee payroll has already been changed in month
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 1
	FROM dbo.EmployeePayrollBasicSalaryHistory AS epbsh WITH (NOLOCK)
	WHERE MONTH(@Month) = MONTH(epbsh.EffectiveDate)
		AND YEAR(@Month) = YEAR(epbsh.EffectiveDate)
		AND epbsh.EmployeeId = @EmployeeId
END