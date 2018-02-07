-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-10
-- Description:	Create new bank account
-- Update:
---- 2018-01-25: Add @Id
---- 2018-02-05: Check if Account name and Account number is already exist
---- 2018-02-05: Add Error Code and Error Message
---- 2018-02-07: Allow Swift code to be null
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_CreateBankAccount]
	@Id int = 0,
	@EmployeeId int,
	@BankType int = 1,
	@BankName nvarchar(70),
	@BankBranch nvarchar(70),
	@BankAddress nvarchar(500),
	@BankSwiftCode varchar(15) = NULL,
	@AccountName nvarchar(30),
	@AccountNumber varchar(30),
	@AccountType int = 1,
	@BeneficiaryName nvarchar(70),
	@IsActive bit = 1,
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
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
			)
	BEGIN
		SET @ErrCode = 2
		SET @ErrMsg = N'Account Number must be unique for an Employee'

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