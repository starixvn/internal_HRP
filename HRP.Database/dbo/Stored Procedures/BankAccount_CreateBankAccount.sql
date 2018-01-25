-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-10
-- Description:	Create new bank account
-- Updated on: 2018-01-25
-- Update reason:
---- Add @Id
---- Check if Account name and Account number is already exist
---- Add Error Code and Error Message
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_CreateBankAccount]
	@Id int = 0,
	@EmployeeId int,
	@BankType int = 1,
	@BankName nvarchar(70),
	@BankBranch nvarchar(70),
	@BankAddress nvarchar(500),
	@BankSwiftCode varchar(15),
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
			AND [AccountNumber] = @AccountNumber
			AND [EmployeeId] = @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Account Name and Account number must be unique for an Employee'

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