
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetAllowances] @PayrollId INT
AS
/*
Created: 20180305@EltonTon
Task : payment process - get monthly allowances
DB: bodb_HRP
Original:
Updated: 
 ---- 20180417@Elton: dont need to join RefResource for contra co account, remove direct banking type
 */
BEGIN
	SELECT [PayrollId]
		,ma.[CurrencyId]
		,c.CurrencyCode
		,[TotalAmount]
		,[DirectBanking1CurrencyId]
		,[DirectBanking1Amount]
		,[DirectBanking1AccountId]
		,rr1.Value AS 'DirectBanking1AccountName'
		,[DirectBanking2CurrencyId]
		,[DirectBanking2Amount]
		,[DirectBanking2AccountId]
		,rr2.Value AS 'DirectBanking2AccountName'
		,[CashCurrencyId]
		,[CashAmount]
		,[CashAccountId]
		,[DSTNetSalaryCurrencyId]
		,[DSTNetSalaryAmount]
		,dst.CurrencyCode AS [DSTNetSalaryCurrencyCode]
		,[AutoCalculateOn]
		,[TotalNetAmount]
		,[Status]
		,[AdditionAmount]
		,[DeductionAmount]
		,ma.AdditionComment
		,ma.DeductionComment
	FROM [dbo].[MonthlyAllowance] AS ma WITH (NOLOCK)
		LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON c.CurrencyId = ma.CurrencyId
		LEFT JOIN dbo.Currency AS dst WITH (NOLOCK) ON dst.CurrencyId = ma.DSTNetSalaryCurrencyId
		LEFT JOIN dbo.BankAccount AS dba1 WITH (NOLOCK) ON DirectBanking1AccountId = dba1.Id
		LEFT JOIN dbo.RefResource rr1 WITH (NOLOCK) ON dba1.PaymentTransactionId = rr1.ResourceId
		AND rr1.RefName = 'PaymentTransactionDirectBanking'
		LEFT JOIN dbo.BankAccount AS dba2 WITH (NOLOCK) ON DirectBanking2AccountId = dba2.Id
		LEFT JOIN dbo.RefResource rr2 WITH (NOLOCK) ON dba2.PaymentTransactionId = rr2.ResourceId
		AND rr2.RefName = 'PaymentTransactionDirectBanking'
	WHERE PayrollId = @PayrollId

	SELECT [PayrollId]
		,ref.ResourceId AS [Type]
		,[NoOfDays]
		,[UnitAmount]
		,[TotalAmount]
		,[NetAllowance]
		,a.[CurrencyId]
		,c.CurrencyCode
		,ref.DisplayOrder
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		LEFT JOIN dbo.MonthlyAllowanceAddition AS a WITH (NOLOCK) ON ref.ResourceId = a.[Type]
		AND a.PayrollId = @PayrollId
		LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON a.CurrencyId = c.CurrencyId
	WHERE ref.RefName = 'MonthlyAllowanceAdditionType'
	ORDER BY ref.DisplayOrder

	SELECT [PayrollId]
		,ref.ResourceId AS [Type]
		,[NoOfDays]
		,[UnitAmount]
		,[TotalAmount]
		,[NetAllowance]
		,d.[CurrencyId]
		,c.CurrencyCode
		,ref.DisplayOrder
	FROM dbo.RefResource AS ref WITH (NOLOCK)
		LEFT JOIN [dbo].[MonthlyAllowanceDeduction] AS d WITH (NOLOCK) ON ref.ResourceId = d.[Type]
		AND d.PayrollId = @PayrollId
		LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON d.CurrencyId = c.CurrencyId
	WHERE ref.RefName = 'MonthlyAllowanceAdditionType'
	ORDER BY ref.DisplayOrder

	SELECT [PayrollId]
		,mba.[CurrencyId]
		,c.[CurrencyCode]
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
	FROM [dbo].[MonthlyBasicAllowance] AS mba WITH (NOLOCK)
		LEFT JOIN dbo.Currency AS c WITH (NOLOCK) ON c.CurrencyId = mba.CurrencyId
	WHERE PayrollId = @PayrollId
END