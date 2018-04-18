
CREATE PROCEDURE [dbo].[BankAccount_GetBankAccountsByEmpId]
	@EmployeeId INT,
	@BankTypeId INT = NULL,
	@IsActive BIT = NULL
AS
/*
Created: 20180111@Hunter.Tran
Task: Get Bank Accounts of an employee by EmployeeId
DB: bodb_HRP
Original:
Revisions:

Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT
	ba.Id AS BankAccountId
	, ba.EmployeeId
	, ba.BankType
	, rr.Value AS BankTypeName
	, ba.BankName
	, ba.BankBranch
	, ba.BankAddress
	, ba.BankSwiftCode
	, ba.AccountName
	, ba.AccountNumber
	, ba.AccountType
	, rr1.Value AS AccountTypeName
	, ba.BeneficiaryName
	, rr2.Value AS PaymentTransaction
	, ba.IsActive
	FROM [dbo].[BankAccount] AS ba WITH (NOLOCK)
	INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON ba.EmployeeId = e.EmployeeId
	INNER JOIN [dbo].Employment AS emp WITH (NOLOCK) ON e.EmployeeId = emp.EmployeeId
	INNER JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON rr1.ResourceId = ba.AccountType AND rr1.RefName = 'TypeOfBankAccount'
	INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON rr.ResourceId = ba.BankType AND rr.RefName = 'BankType'	
	INNER JOIN dbo.RefResource AS rr2 WITH (NOLOCK) ON 
	CASE
		WHEN ba.BankType = 1 THEN 'PaymentTransactionEbanking'
		ELSE 'PaymentTransactionDirectBanking'
	END = rr2.RefName AND 
	rr2.ResourceId = ba.PaymentTransactionId
	WHERE ba.EmployeeId = @EmployeeId
	AND (@BankTypeId IS NULL OR ba.BankType = @BankTypeId)
	AND (@IsActive IS NULL OR ba.IsActive = @IsActive)
	ORDER BY rr.Value
END