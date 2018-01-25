-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-15
-- Description:	Get Bank Account by Id
-- =============================================
CREATE PROCEDURE BankAccount_GetBankAccountById
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *
	FROM BankAccount
	WHERE Id = @Id
END