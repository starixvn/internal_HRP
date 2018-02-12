CREATE PROCEDURE [dbo].[BankAccount_CheckNameExisted]
	@Id int,
	@AccountName nvarchar(30),
	@EmployeeId int,
	@BankType int
AS
/*
Created: 20180204@Hunter.Tran
Task: Check if a bank account name is existed
DB: bodb_HRP
Original:
Revisions:
---- 20180206@Hunter.Tran: Check for edit. @Id <> 0
---- 20180212@Hunter.Tran: Account Name can be duplicate for different bank types
Param's Explanation:
*/
BEGIN
    SELECT 1
    FROM [dbo].[BankAccount] WITH (NOLOCK)
    WHERE [AccountName] = @AccountName
        AND [EmployeeId] = @EmployeeId
		AND [BankType] = @BankType
        AND [Id] <> @Id
END