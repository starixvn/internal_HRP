
CREATE PROCEDURE [dbo].[BankAccount_GetBankAccountsByEmpId]
	@EmployeeId int
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
	ba.Id
	, ba.EmployeeId
	, e.FullName
	, emp.CompanyId
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
	, ba.IsActive
	FROM [dbo].[BankAccount] AS ba WITH (NOLOCK)
	INNER JOIN [dbo].Employee AS e WITH (NOLOCK) ON ba.EmployeeId = e.EmployeeId
	INNER JOIN [dbo].Employment AS emp WITH (NOLOCK) ON e.EmployeeId = emp.EmployeeId
	INNER JOIN dbo.RefResource AS rr WITH (NOLOCK) ON rr.ResourceId = ba.BankType AND rr.RefName = 'BankType'
	INNER JOIN dbo.RefResource AS rr1 WITH (NOLOCK) ON rr1.ResourceId = ba.AccountType AND rr1.RefName = 'BankAccountType'
	WHERE ba.EmployeeId = @EmployeeId
	ORDER BY rr.Value

	SELECT CompanyId
	FROM [dbo].Employment WITH(NOLOCK)
	WHERE EmployeeId = @EmployeeId

	SELECT FullName
	FROM [dbo].Employee WITH(NOLOCK)
	WHERE EmployeeId = @EmployeeId
END