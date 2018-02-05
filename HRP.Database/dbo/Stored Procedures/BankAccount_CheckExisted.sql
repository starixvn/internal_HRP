-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-02-01
-- Description:	Check if a bank account name and number is existed
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_CheckExisted]
	@Id int,
	@AccountName nvarchar(30),
	@AccountNumber nvarchar(30),
	@EmployeeId int
AS
BEGIN
	IF(@Id = 0)
	SELECT 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [AccountName] = @AccountName 
	AND [AccountNumber] = @AccountNumber 
	AND [EmployeeId] = @EmployeeId
END