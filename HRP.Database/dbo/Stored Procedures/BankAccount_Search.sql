/*
 Created: 20180117@EltonTon
 Task : Search bank accounts
 DB: bodb_HRP
 */
Create PROCEDURE [dbo].[BankAccount_Search]
	@EmployeeId INT,
	@BankTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ba.Id, ba.AccountName
	FROM BankAccount ba WITH (NOLOCK)
	WHERE ba.EmployeeId = @EmployeeId AND ba.BankType = @BankTypeId
END