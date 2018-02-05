-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-11
-- Description:	Get Bank Accounts of an employee by EmployeeId
-- Updated:     2018-01-29 - Add more info to result
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

	SELECT CompanyId
	FROM Employment
	WHERE EmployeeId = @EmployeeId

	SELECT FullName
	FROM Employee
	WHERE EmployeeId = @EmployeeId
END