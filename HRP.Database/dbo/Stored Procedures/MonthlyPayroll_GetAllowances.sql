
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetAllowances] 
	@PayrollId INT
AS
/*
Created: 20180305@EltonTon
Task : payment process - get monthly allowances
DB: bodb_HRP
Original:
 */
BEGIN
	DECLARE @startDate DATE, @dueDate DATE, @month DATE

	SELECT [PayrollId]
		,[CurrencyId]
		,[TotalAmount]
		,[DirectBanking1CurrencyId]
		,[DirectBanking1Amount]
		,[DirectBanking1BankTypeId]
		,[DirectBanking1AccountId]
		,ISNULL(dba1.AccountName, rr1.Value) AS 'DirectBanking1AccountName'
		,[DirectBanking2CurrencyId]
		,[DirectBanking2Amount]
		,[DirectBanking2BankTypeId]
		,[DirectBanking2AccountId]
		,ISNULL(dba2.AccountName, rr2.Value) AS 'DirectBanking2AccountName'
		,[CashCurrencyId]
		,[CashAmount]
		,[CashAccountId]
		,[DSTNetSalaryCurrencyId]
		,[DSTNetSalaryAmount]
		,[AutoCalculateOn]
		,[TotalNetAmount]
		,[Status]
	FROM [dbo].[MonthlyAllowance] WITH(NOLOCK)
	LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON DirectBanking1AccountId = dba1.Id
	AND DirectBanking1BankTypeId <> 3
	LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON DirectBanking2AccountId = dba2.Id
	AND DirectBanking2BankTypeId <> 3
	LEFT JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON DirectBanking1AccountId = rr1.ResourceId
	AND DirectBanking1BankTypeId = 3
	AND rr1.RefName = 'ContraCoAccount'
	LEFT JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON DirectBanking2AccountId = rr2.ResourceId
	AND DirectBanking2BankTypeId = 3 AND rr2.RefName = 'ContraCoAccount'
	WHERE PayrollId = @PayrollId

	SELECT [PayrollId]
		,[Type]
		,[NoOfDays]
		,[UnitAmount]
		,[TotalAmount]
		,[NetAllowance]
		,[CurrencyId]
	FROM [dbo].[MonthlyAllowanceAddition] WITH(NOLOCK)
	WHERE PayrollId = @PayrollId
	ORDER BY [Type]

	SELECT [PayrollId]
		,[Type]
		,[NoOfDays]
		,[UnitAmount]
		,[TotalAmount]
		,[NetAllowance]
		,[CurrencyId]
	FROM [dbo].[MonthlyAllowanceDeduction] WITH(NOLOCK)
	WHERE PayrollId = @PayrollId
	ORDER BY [Type]

	SELECT [PayrollId]
      ,[CurrencyId]
      ,[TotalAmount]
      ,[DirectBanking1CurrencyId]
      ,[DirectBanking1Amount]
      ,[DirectBanking2CurrencyId]
      ,[DirectBanking2Amount]
      ,[CashCurrencyId]
      ,[CashAmount]
      ,[SubsistenceCurrencyId]
      ,[SubsistenceAmount]
      ,[HomePassageCurrencyId]
      ,[HomePassageAmount]
      ,[HousingCurrencyId]
      ,[HousingAmount]
      ,[MobilePhoneCurrencyId]
      ,[MobilePhoneAmount]
      ,[HPICurrencyId]
      ,[HPIAmount]
      ,[DutyMealCurrencyId]
      ,[DutyMealAmount]
      ,[SecondmentCurrencyId]
      ,[SecondmentAmount]
      ,[OthersCurrencyId]
      ,[OthersAmount]
      ,[RoomingChargesCurrencyId]
      ,[RoomingChargesAmount]
      ,[FitnessFirstCurrencyId]
      ,[FitnessFirstAmount]
      ,[AutoCalculateOn]
      ,[CreatedBy]
      ,[CreatedDate]
	  ,[AfterAdjustmentAmount]
	  ,[Reason]
	FROM [dbo].[MonthlyBasicAllowance] WITH(NOLOCK)
	WHERE PayrollId = @PayrollId

	SELECT @month = mp.[Month]
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
	WHERE mp.PayrollId = @PayrollId

	SELECT @startDate = StartDate
		,@dueDate = DueDate
	FROM dbo.fnc_GetPaymentProcessDuration(@month)

	SELECT emp.EmployeeId
		,emp.FullName
		,emp.Nickname
		,c.Code + em.EmployeeCode AS IdNo
		,em.CompanyId
		,c.Code AS CompanyCode
		,cc.Code AS CostCenter
		,@startDate AS StartDate
		,@dueDate AS DueDate
		,mbs.WorkingDays
	FROM dbo.MonthlyPayroll AS mp WITH (NOLOCK)
	INNER JOIN dbo.MonthlyBasicSalary mbs on mp.PayrollId = mbs.PayrollId
	INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON mp.EmployeeId = emp.EmployeeId
	INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON emp.EmployeeId = em.EmployeeId
	INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
	INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) ON em.CostCenterId = cc.CostCenterId
	WHERE mp.PayrollId = @PayrollId
END