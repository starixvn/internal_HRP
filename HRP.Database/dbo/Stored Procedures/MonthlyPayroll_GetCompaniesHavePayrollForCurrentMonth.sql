
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetCompaniesHavePayrollForCurrentMonth] 
AS
/*
 Created: 20180411@EltonTon
 Task : Get companies having payroll for current month
 DB: bodb_HRP
 Original:
 Revisions:
 Param's Explanation:
 */
BEGIN
	SET NOCOUNT ON;

	DECLARE @currentDate DATE = GETDATE()

	SELECT c.Code
	FROM dbo.Company c WITH (NOLOCK)
	WHERE EXISTS (SELECT 1
				  FROM dbo.Employment e WITH (NOLOCK) INNER JOIN dbo.MonthlyPayroll mp WITH (NOLOCK) ON e.EmployeeId = mp.EmployeeId
				  AND YEAR(@currentDate) = YEAR(mp.Month) AND MONTH(@currentDate) = MONTH(mp.Month)
				  WHERE e.CompanyId = c.CompanyId)
END