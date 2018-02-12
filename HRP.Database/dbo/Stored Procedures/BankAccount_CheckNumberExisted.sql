
CREATE PROCEDURE [dbo].[BankAccount_CheckNumberExisted]
	@Id int,
	@AccountNumber nvarchar(30),
	@EmployeeId int,
	@BankType int
AS
/*
Created: 20180206@Hunter.Tran
Task: Check if a bank account number is existed
DB: bodb_HRP
Original:
Revisions:
---- 20180208@Hunter.Tran: Account Number can be duplicate for different bank types
Param's Explanation:
*/
BEGIN
	SELECT 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [AccountNumber] = @AccountNumber
		AND [EmployeeId] = @EmployeeId
		AND [BankType] = @BankType
		AND [Id] <> @Id
END