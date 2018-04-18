CREATE PROCEDURE [dbo].[BankAccount_Search]
    @EmployeeId INT,
    @BankTypeId INT,
    @IsActive BIT = NULL
AS
/*
 Created: 20180117@EltonTon
 Task : Search bank accounts
 DB: bodb_HRP
 Updated: 
 ---- 20180207@Elton: Include IsActive to filter on payroll page
 ---- 20180226@Hunter: IsActive in SP parameter to reduce complexity in Controller
 */
BEGIN
    SET NOCOUNT ON;

	DECLARE @refName VARCHAR(50);

	IF @BankTypeId = 1
		SET @refName = 'PaymentTransactionEbanking'		
	ELSE
		SET @refName = 'PaymentTransactionDirectBanking'

	SELECT ba.Id AS BankAccountId
	, rr.Value AS PaymentTransaction
	FROM BankAccount ba WITH (NOLOCK)
	INNER JOIN dbo.RefResource rr WITH (NOLOCK) on ba.PaymentTransactionId = rr.ResourceId AND rr.RefName = @refName
	WHERE ba.EmployeeId = @EmployeeId AND ba.BankType = @BankTypeId AND ba.IsActive = 1
END