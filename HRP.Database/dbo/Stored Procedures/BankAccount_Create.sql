
CREATE PROCEDURE [dbo].[BankAccount_Create] 
	@Id INT = 0
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
	,@CreatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180110@Hunter.Tran
Task: Create new bank account
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
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT

	DECLARE @CompanyId INT
	SELECT @CompanyId = CompanyId
	FROM Employment
	WHERE EmployeeId = @EmployeeId

	-- Check Permission on Company
	IF dbo.fnc_GetFunctionPermissionOnCompany(@CreatedBy, @CompanyId, 'EmployeeBankDetails') <> 2 -- don't have host permission
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'You don''t have permission to create bank account for this employee'
		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM [dbo].[BankAccount] WITH (NOLOCK)
			WHERE [PaymentTransactionId] = @PaymentTransactionId
				AND [EmployeeId] = @EmployeeId
				AND [BankType] = @BankType
				AND [IsActive] = @IsActive
			)
	BEGIN
		SET @ErrCode = 2
		SET @ErrMsg = N'Payment Transaction already exists. Please choose another value.'

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
		SET @ErrCode = 3
		SET @ErrMsg = N'Account Number already exists. Please enter another value.'

		RETURN
	END

	--DECLARE @totalActiveAccountInBankType INT = 0;

	--SELECT @totalActiveAccountInBankType = COUNT(Id)
	--FROM [dbo].[BankAccount] WITH (NOLOCK)
	--WHERE [EmployeeId] = @EmployeeId
	--	AND [BankType] = @BankType
	--	AND [IsActive] = 1

	--IF (
	--		@totalActiveAccountInBankType >= 3
	--		AND @IsActive = 1
	--		)
	--BEGIN
	--	SET @ErrCode = 4
	--	SET @ErrMsg = N'This employee has reached the limit of number bank accounts. You cannot create more bank account.'

	--	RETURN
	--END

	INSERT INTO [dbo].[BankAccount] (
		[EmployeeId]
		,[BankType]
		,[BankName]
		,[BankBranch]
		,[BankAddress]
		,[BankSwiftCode]
		,[PaymentTransactionId]
		,[AccountNumber]
		,[AccountType]
		,[BeneficiaryName]
		,[IsActive]
		)
	VALUES (
		@EmployeeId
		,@BankType
		,@BankName
		,@BankBranch
		,@BankAddress
		,@BankSwiftCode
		,@PaymentTransactionId
		,@AccountNumber
		,@AccountType
		,@BeneficiaryName
		,@IsActive
		)

	--Write Log
	SET @BankAccountId = SCOPE_IDENTITY()
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
			WHERE Id = @BankAccountId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'BankAccount_Create'
		,@ObjectId = @BankAccountId
		,@Data = @logData
		,@Duration = @Duration
END