-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-11
-- Description:	Get Bank Accounts of an employee by EmployeeId
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_GetBankAccountsByEmpId]
	-- Add the parameters for the stored procedure here
	@EmployeeId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM BankAccount
	WHERE EmployeeId = @EmployeeId
END