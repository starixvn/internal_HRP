
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
		ba.Id
		, ba.EmployeeId
		, ba.BankType
		, ba.BankName
		, ba.BankBranch
		, ba.BankAddress
		, ba.BankSwiftCode
		, ba.AccountName
		, ba.AccountNumber
		, ba.AccountType
		, ba.BeneficiaryName
		, ba.IsActive
	FROM [dbo].[BankAccount] AS ba WITH(NOLOCK)
	WHERE Id = @Id
END