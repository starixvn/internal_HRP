/*
 Created: 20180117@EltonTon
 Task : Search bank accounts
 DB: bodb_HRP
 Updated: 
 ---- 2018-02-07: Include IsActive to filter on payroll page
 */
CREATE PROCEDURE [dbo].[BankAccount_Search]
	@EmployeeId INT,
	@BankTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ba.Id AS BankAccountId, ba.AccountName, ba.IsActive
	FROM BankAccount ba WITH (NOLOCK)
	WHERE ba.EmployeeId = @EmployeeId AND ba.BankType = @BankTypeId
END