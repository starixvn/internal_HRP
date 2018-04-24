
CREATE FUNCTION [dbo].[fnc_CheckRefResourceItemIsUsed] (
	@RefName VARCHAR(100)
	,@ResourceId SMALLINT
	)
RETURNS BIT
AS
/*
Created: 20180406@Raymond.Le
Task : Check item in ref resource is used
DB: bodb_HRP

Param's Explanation:

Revisions:
*/
BEGIN
	DECLARE @isUsed BIT

	IF @RefName = 'LocalCompany'
	BEGIN
		SELECT @isUsed = 1
		FROM (
			SELECT 1 AS i
			FROM dbo.Employment AS e WITH (NOLOCK)
			WHERE e.LocalCompanyId = @ResourceId
			
			UNION
			
			SELECT 1 AS i
			FROM dbo.EmploymentWorkPermit WITH (NOLOCK)
			WHERE LocalCompanyId = @ResourceId
			) u

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'Nationality'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.Nationality = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'NoOfChildren'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.Employee AS e WITH (NOLOCK)
		WHERE e.NoOfChildren = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'LocalJobTitle'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.Employment AS e WITH (NOLOCK)
		WHERE e.LocalJobTitleId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'JobTitleChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.JobTitleReasonId = @ResourceId
			OR e.LocalJobTitleReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'DateCommencementChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.GroupDateReasonId = @ResourceId
			OR e.CompanyDateReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'EmployeeIDChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.IDNoReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'EmploymentType'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.Employment AS e WITH (NOLOCK)
		WHERE e.EmploymentType = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'EmploymentStatus'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.Employment AS e WITH (NOLOCK)
		WHERE e.EmploymentStatus = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'JobCategoryChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.JobCategoryReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'JobGradeChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.JobGradeReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'DivisionChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.DivisionReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'DepartmentChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.DepartmentReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'SectionChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.SectionReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'CostCenterChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory AS e WITH (NOLOCK)
		WHERE e.CostCenterReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'WorkPermitType1'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentWorkPermit AS e WITH (NOLOCK)
		WHERE e.Type1 = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'WorkPermitType2'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentWorkPermit AS e WITH (NOLOCK)
		WHERE e.Type2 = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'TypeOfBankAccount'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.BankAccount WITH (NOLOCK)
		WHERE AccountType = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'CompanyBVIChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory WITH (NOLOCK)
		WHERE CompanyReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'CompanyLocalChangeReason'
	BEGIN
		SELECT @isUsed = 1
		FROM dbo.EmploymentHistory WITH (NOLOCK)
		WHERE LocalCompanyReasonId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'PaymentTransactionEbanking'
	BEGIN
		-- Check is used
		SELECT @isUsed = 1
		FROM dbo.BankAccount AS ba WITH (NOLOCK)
		WHERE ba.BankType = 1
			AND ba.PaymentTransactionId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	IF @RefName = 'PaymentTransactionDirectBanking'
	BEGIN
		-- Check is used
		SELECT @isUsed = 1
		FROM dbo.BankAccount AS ba WITH (NOLOCK)
		WHERE ba.BankType = 2
			AND ba.PaymentTransactionId = @ResourceId

		RETURN @isUsed
	END

	--------------------------
	DECLARE @month DATE
		,@firstDayOfCurrentMonth DATE
		,@firstDayOfLastMonth DATE
		,@lastStartDate DATE
		,@lastDueDate DATE
		,@today DATE = GETDATE()

	-- Get current PaymentProcess month
	SET @firstDayOfCurrentMonth = DATEADD(MONTH, DATEDIFF(MONTH, 0, @today), 0)
	SET @firstDayOfLastMonth = DATEADD(MONTH, - 1, @firstDayOfCurrentMonth)

	SELECT @lastStartDate = StartDate
		,@lastDueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@firstDayOfLastMonth)

	IF (
			@today BETWEEN @lastStartDate
				AND @lastDueDate
			)
	BEGIN
		SET @month = @firstDayOfLastMonth
	END
	ELSE
	BEGIN
		SET @month = @firstDayOfCurrentMonth
	END

	IF @RefName = 'PaymentTransactionCash'
	BEGIN
		-- Check is used
		SELECT @isUsed = 1
		FROM (
			SELECT 1 AS i
			FROM dbo.EmployeePayrollAllowance WITH (NOLOCK)
			WHERE CashAccountId = @ResourceId
			
			UNION
			
			SELECT 1 AS i
			FROM dbo.EmployeePayrollBasicSalary WITH (NOLOCK)
			WHERE CashAccountId = @ResourceId
			
			UNION
			
			SELECT 1 AS i
			FROM dbo.MonthlyPayroll AS p WITH (NOLOCK)
			LEFT JOIN dbo.MonthlyAllowance AS a WITH (NOLOCK) ON p.PayrollId = a.PayrollId
			LEFT JOIN dbo.MonthlySalary AS s WITH (NOLOCK) ON p.PayrollId = s.PayrollId
			WHERE p.[Month] = @month
				AND (
					a.CashAccountId = @ResourceId
					OR s.CashAccountId = @ResourceId
					)
			) u

		RETURN @isUsed
	END

	RETURN @isUsed
END