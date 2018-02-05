-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-16
-- Description:	Update bank account
-- Updated:
---- 2018-02-05: Add ErrCode and ErrMsg to parameters
-- =============================================
CREATE PROCEDURE [dbo].[BankAccount_Update]
	@Id int,
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
			AND [Id] <> @Id
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Account Name and Account number must be unique for an Employee'

		RETURN
	END

	declare @total int;

	SELECT @total = COUNT(Id)
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [EmployeeId] = @EmployeeId
	AND [BankType] = @BankType
	AND [IsActive] = 1

	IF(@total >= 3 and @IsActive = 1)
	BEGIN
		SET @ErrCode = 2
		SET @ErrMsg = N'This employee has reached the limit of number bank accounts'

		RETURN
	END

	UPDATE BankAccount WITH (ROWLOCK, UPDLOCK)
	SET 
	EmployeeId = @EmployeeId,
	BankType = @BankType,
	BankName = @BankName,
	BankBranch = @BankBranch,
	BankAddress = @BankAddress,
	BankSwiftCode = @BankSwiftCode,
	AccountName = @AccountName,
	AccountNumber = @AccountNumber,
	AccountType = @AccountType,
	BeneficiaryName = @BeneficiaryName,
	IsActive = @IsActive
	WHERE Id = @Id
END