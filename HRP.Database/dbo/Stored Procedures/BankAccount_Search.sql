/*
 Created: 20180117@EltonTon
 Task : Search bank accounts
 DB: bodb_HRP
 Updated: 
 ---- 20180207@Elton: Include IsActive to filter on payroll page
 ---- 20180226@Hunter: IsActive in SP parameter to reduce complexity in Controller
 */
CREATE PROCEDURE [dbo].[BankAccount_Search]
    @EmployeeId INT,
    @BankTypeId INT,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ba.Id AS BankAccountId, ba.AccountName
    FROM BankAccount ba WITH (NOLOCK)
    WHERE ba.EmployeeId = @EmployeeId
        AND ba.BankType = @BankTypeId
        AND (@IsActive IS NULL OR ba.IsActive = @IsActive)
END