-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-02-01
-- Description:	Check if a bank account name is existed
-- Update
---- 2018-02-06: Check for edit. @Id <> 0
----
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_CheckNameExisted]
	@Id int,
	@AccountName nvarchar(30),
	@EmployeeId int
AS
BEGIN
	SELECT 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [AccountName] = @AccountName
	AND [EmployeeId] = @EmployeeId
	AND [Id] <> @Id
END