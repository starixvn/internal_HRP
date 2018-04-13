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
	SELECT TOP 1 COUNT(BankAccount.BankType) AS BankTypeCount
	FROM RefResource WITH (NOLOCK) left join BankAccount WITH (NOLOCK)
	ON RefResource.ResourceId = BankAccount.BankType and BankAccount.EmployeeId = @EmployeeId
	WHERE RefResource.RefName = 'BankType'
	GROUP BY RefResource.ResourceId
	ORDER BY BankTypeCount ASC)

	SELECT 1
	WHERE @totalActiveAccountInBankType >= 3
END