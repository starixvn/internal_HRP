
CREATE PROCEDURE [dbo].[BankAccount_GetBankAccountById]
	@Id int
AS
/*
Created: 20180115@Hunter.Tran
Task: Get Bank Account by Id
DB: bodb_HRP
Original:
Revisions:

Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

    SELECT 
		ba.Id AS BankTypeId
		, ba.EmployeeId
		, e.Nickname
		, ba.BankType
		, ba.BankName
		, ba.BankBranch
		, ba.BankAddress
		, ba.BankSwiftCode
		, ba.PaymentTransactionId
		, ba.AccountNumber
		, ba.AccountType
		, ba.BeneficiaryName
		, ba.IsActive
	FROM [dbo].[BankAccount] AS ba WITH(NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON ba.EmployeeId = e.EmployeeId
	WHERE Id = @Id
END