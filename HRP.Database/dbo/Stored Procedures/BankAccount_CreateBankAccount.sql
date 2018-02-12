CREATE PROCEDURE [dbo].[BankAccount_CreateBankAccount]
	@Id INT = 0,
	@EmployeeId INT,
	@BankType INT = 1,
	@BankName NVARCHAR(70),
	@BankBranch NVARCHAR(70),
	@BankAddress NVARCHAR(500),
	@BankSwiftCode VARCHAR(15) = NULL,
	@AccountName NVARCHAR(30),
	@AccountNumber VARCHAR(30),
	@AccountType INT = 1,
	@BeneficiaryName NVARCHAR(70),
	@IsActive bit = 1,
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180110@Hunter.Tran
Task: Create new bank account
DB: bodb_HRP
Original:
Revisions:
---- 20180125: Add @Id
---- 20180205: Check if Account name and Account number is already exist
---- 20180205: Add Error Code and Error Message
---- 20180207: Allow Swift code to be null
---- 20180208: Account Number can be duplicate for different bank types
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [AccountName] = @AccountName
			AND [EmployeeId] = @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Account Name must be unique for an Employee'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [AccountNumber] = @AccountNumber
			AND [EmployeeId] = @EmployeeId
			AND [BankType] = @BankType
			)
	BEGIN
		SET @ErrCode = 2
		SET @ErrMsg = N'Account Number must be unique in each Bank Type for an Employee'

		RETURN
	END

    INSERT INTO [dbo].[BankAccount]
           ([EmployeeId]
		   ,[BankType]
           ,[BankName]
           ,[BankBranch]
           ,[BankAddress]
           ,[BankSwiftCode]
           ,[AccountName]
           ,[AccountNumber]
           ,[AccountType]
           ,[BeneficiaryName]
           ,[IsActive])
     VALUES
           (@EmployeeId
		   ,@BankType
           ,@BankName
           ,@BankBranch
           ,@BankAddress
           ,@BankSwiftCode
           ,@AccountName
           ,@AccountNumber
           ,@AccountType
           ,@BeneficiaryName
           ,@IsActive)
END