
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetFilterInfo] @UserId INT
	,@Month DATE
	,@CompanyId SMALLINT
	,@PayrollId INT = NULL
AS
/*
Created: 20180423@Tim.Trinh
Task: get filter info
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'
		,@policy TINYINT
		,@startDate DATE
		,@dueDate DATE
		,@today DATE = GETDATE()

	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@Month)

	SELECT @policy = dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, @functionKey)

	SELECT TOP 1 mp.PayrollId
		,emp.EmployeeId
		,emp.FullName
		,emp.Nickname
		,c.Code + em.EmployeeCode AS IDNo
		,em.CompanyId
		,c.Code AS CompanyCode
		,cc.Code AS CostCenter
		,@startDate AS StartDate
		,@dueDate AS DueDate
		,mbs.WorkingDays
		,(CASE 
			WHEN @today BETWEEN @startDate
					AND @dueDate
				AND @policy = 2 THEN 1
			ELSE 0 END
			) AS IsHost
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON mp.EmployeeId = emp.EmployeeId
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON em.CostCenterId = cc.CostCenterId
		INNER JOIN dbo.MonthlyBasicSalary AS mbs ON mp.PayrollId = mbs.PayrollId
	WHERE ISNULL(@PayrollId, mp.PayrollId) = mp.PayrollId
		AND c.CompanyId = @CompanyId
		AND mp.[Month] = @Month
END