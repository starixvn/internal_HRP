CREATE PROCEDURE [dbo].[BankAccount_CheckLimitReached]
	@EmployeeId INT
AS
/*
Created: 20180222@Hunter.Tran
Task : Check if employee has reach limit number of bank account
DB: bodb_HRP
Original:
Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	declare @totalActiveAccountInBankType INT = 0;

	SELECT @totalActiveAccountInBankType = (
	SELECT TOP 1 COUNT(Id) as IdNumbers
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [EmployeeId] = @EmployeeId
		AND [IsActive] = 1
	GROUP BY [BankType]
	ORDER BY IdNumbers ASC)

	SELECT 1
	WHERE @totalActiveAccountInBankType >= 3
END