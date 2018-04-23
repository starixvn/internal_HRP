
CREATE PROCEDURE [dbo].[BankAccount_Update] @Id INT
	,@EmployeeId INT
	,@BankType INT = 1
	,@BankName NVARCHAR(70)
	,@BankBranch NVARCHAR(70) = NULL
	,@BankAddress NVARCHAR(500) = NULL
	,@BankSwiftCode VARCHAR(15) = NULL
	,@PaymentTransactionId INT
	,@AccountNumber VARCHAR(30)
	,@AccountType INT = 1
	,@BeneficiaryName NVARCHAR(70)
	,@IsActive BIT = 1
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
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
		,@logOldData NVARCHAR(MAX)
		,@logData NVARCHAR(MAX)
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT

	DECLARE @CompanyId INT
	SELECT @CompanyId = CompanyId
	FROM Employment
	WHERE EmployeeId = @EmployeeId

	-- Check Permission on Company
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UpdatedBy, @CompanyId, 'EmployeeBankDetails') <> 2 -- don't have host permission
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'You don''t have permission to update bank account for this employee'
		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [PaymentTransactionId] = @PaymentTransactionId
				AND [EmployeeId] = @EmployeeId
				AND [BankType] = @BankType
				AND [IsActive] = @IsActive
				AND [Id] <> @Id
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Payment Transaction already exists. Please choose another value.'

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

	--DECLARE @totalActiveAccountInBankType INT = 0
	--	,@isAccountChangeBankType BIT = 0
	--	,@isAccountChangeStatus BIT = 0

	--SELECT @totalActiveAccountInBankType = COUNT(Id)
	--FROM [dbo].[BankAccount] WITH (NOLOCK)
	--WHERE [EmployeeId] = @EmployeeId
	--	AND [BankType] = @BankType
	--	AND [IsActive] = 1

	--SELECT @isAccountChangeBankType = 1
	--FROM [dbo].[BankAccount] WITH (NOLOCK)
	--WHERE [Id] = @Id
	--	AND [BankType] <> @BankType

	--SELECT @isAccountChangeStatus = 1
	--FROM [dbo].[BankAccount] WITH (NOLOCK)
	--WHERE [Id] = @Id
	--	AND [IsActive] <> @IsActive

	--IF (
	--		@totalActiveAccountInBankType >= 3
	--		AND @IsActive = 1
	--		AND (
	--			@isAccountChangeBankType = 1
	--			OR @isAccountChangeStatus = 1
	--			)
	--		)
	--BEGIN
	--	SET @ErrCode = 3
	--	SET @ErrMsg = N'This employee has reached the limit of number bank accounts'

	--	RETURN
	--END

	SET @xml = (
			SELECT Id AS '@id'
				,EmployeeId AS '@eid'
				,BankType AS '@bt'
				,BankName AS '@bn'
				,BankBranch AS '@bb'
				,BankAddress AS '@ba'
				,BankSwiftCode AS '@bs'
				,PaymentTransactionId AS '@pti'
				,AccountNumber AS '@anum'
				,AccountType AS '@at'
				,BeneficiaryName AS '@bene'
				,IsActive AS '@ia'
			FROM [dbo].[BankAccount]
			WHERE Id = @Id
			FOR XML PATH('r')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].[BankAccount] WITH (ROWLOCK, UPDLOCK)
	SET EmployeeId = @EmployeeId
		,BankType = @BankType
		,BankName = @BankName
		,BankBranch = @BankBranch
		,BankAddress = @BankAddress
		,BankSwiftCode = @BankSwiftCode
		,PaymentTransactionId = @PaymentTransactionId
		,AccountNumber = @AccountNumber
		,AccountType = @AccountType
		,BeneficiaryName = @BeneficiaryName
		,IsActive = @IsActive
	WHERE Id = @Id

	--Write Log
	SET @xml = (
			SELECT Id AS '@id'
				,EmployeeId AS '@eid'
				,BankType AS '@bt'
				,BankName AS '@bn'
				,BankBranch AS '@bb'
				,BankAddress AS '@ba'
				,BankSwiftCode AS '@bs'
				,PaymentTransactionId AS '@pti'
				,AccountNumber AS '@anum'
				,AccountType AS '@at'
				,BeneficiaryName AS '@bene'
				,IsActive AS '@ia'
			FROM [dbo].[BankAccount]
			WHERE Id = @Id
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'BankAccount_Update'
		,@ObjectId = @BankAccountId
		,@Data = @logData
		,@OldData = @logOldData
		,@Duration = @Duration
END