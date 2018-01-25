-- =============================================
-- Author:		Hunter.Tran
-- Create date: 2018-01-16
-- Description:	Update bank account
-- =============================================
CREATE PROCEDURE BankAccount_Update
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
	@IsActive bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

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