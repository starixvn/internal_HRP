
CREATE PROCEDURE [dbo].[BankAccount_Update]
	@Id INT,
	@EmployeeId INT,
	@BankType INT = 1,
	@BankName NVARCHAR(70),
	@BankBranch NVARCHAR(70),
	@BankAddress NVARCHAR(500),
	@BankSwiftCode varchar(15) = NULL,
	@AccountName NVARCHAR(30),
	@AccountNumber varchar(30),
	@AccountType INT = 1,
	@BeneficiaryName NVARCHAR(70),
	@IsActive BIT = 1,
	@UpdatedBy INT = 0,
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS

/*
Created: 20180116@Hunter.Tran
Task : Update bank account
DB: bodb_HRP
Original:
Revisions:
Param's Explanation:
*/

BEGIN
	SET NOCOUNT ON;

	-- For log
	DECLARE @BankAccountId INT
			,@xml XML
			,@logData NVARCHAR(MAX)
			,@startRun DATETIME
			,@endRun DATETIME
			,@duration INT

	SET @startRun = GETDATE()

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [AccountName] = @AccountName
			AND [EmployeeId] = @EmployeeId
			AND [BankType] = @BankType
			AND [Id] <> @Id
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Account Name already exists. Please enter another value.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [AccountNumber] = @AccountNumber
				AND [EmployeeId] = @EmployeeId
				AND [BankType] = @BankType
				AND [Id] <> @Id
			)
	BEGIN
		SET @ErrCode = 2
		SET @ErrMsg = N'Account Number already exists. Please enter another value.'

		RETURN
	END

	declare @totalActiveAccountInBankType INT = 0;
	declare @isAccountChangeBankType BIT = 0;
	declare @isAccountChangeStatus BIT = 0;

	SELECT @totalActiveAccountInBankType = COUNT(Id)
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [EmployeeId] = @EmployeeId
		AND [BankType] = @BankType
		AND [IsActive] = 1

	SELECT @isAccountChangeBankType = 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [Id] = @Id
		AND [BankType] <> @BankType

	SELECT @isAccountChangeStatus = 1
	FROM [dbo].[BankAccount] WITH (NOLOCK)
	WHERE [Id] = @Id
		AND [IsActive] <> @IsActive

	IF(@totalActiveAccountInBankType >= 3 and @IsActive = 1 and (@isAccountChangeBankType = 1 or @isAccountChangeStatus = 1))
	BEGIN
		SET @ErrCode = 3
		SET @ErrMsg = N'This employee has reached the limit of number bank accounts'

		RETURN
	END

	UPDATE [dbo].[BankAccount] WITH (ROWLOCK, UPDLOCK)
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

	--Write Log
	SET @xml = (
			SELECT Id as '@id',
				   EmployeeId as '@eid',
				   BankType as '@bt',
				   BankName as '@bn',
				   BankBranch as '@bb',
				   BankAddress as '@ba',
				   BankSwiftCode as '@bs',
				   AccountName as '@an',
				   AccountNumber as '@anum',
				   AccountType as '@at',
				   BeneficiaryName as '@bene',
				   IsActive as '@ia'
			FROM [dbo].[BankAccount]
			WHERE Id = @Id
			FOR XML PATH('r'),TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'BankAccount_Update'
		,@ObjectId = @BankAccountId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END