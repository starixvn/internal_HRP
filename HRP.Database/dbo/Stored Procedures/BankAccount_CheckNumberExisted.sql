-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-02-06
-- Description:	Check if a bank account number is existed
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_CheckNumberExisted]
	@Id int,
	@AccountNumber nvarchar(30),
	@EmployeeId int
AS
BEGIN
	SELECT 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [AccountNumber] = @AccountNumber
	AND [EmployeeId] = @EmployeeId
	AND [Id] <> @Id
END