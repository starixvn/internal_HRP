
CREATE PROCEDURE [dbo].[PaymentProcess_GenerateMonthlyPayroll] @CompanyId SMALLINT
	,@Month DATE
	,@PayrollXMLDoc NVARCHAR(MAX)
	,@BasicAllowanceXMLDoc NVARCHAR(MAX)
	,@AllowanceXMLDoc NVARCHAR(MAX)
	,@AllowanceAdditionXMLDoc NVARCHAR(MAX) = NULL
	,@AllowanceDeductionXMLDoc NVARCHAR(MAX) = NULL
	,@BasicSalaryXMLDoc NVARCHAR(MAX)
	,@SalaryXMLDoc NVARCHAR(MAX)
	,@SalaryAdditionXMLDoc NVARCHAR(MAX) = NULL
	,@SalaryDeductionXMLDoc NVARCHAR(MAX) = NULL
	,@UpdatedBy INT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(500) = '' OUTPUT
AS
/*
Created: 20180315@Raymond.Le
Task: Generate Monthly Payroll
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @FirstDayOfWorkingMonth DATE

	BEGIN TRY
		SET @FirstDayOfWorkingMonth = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month), 0)

		BEGIN TRANSACTION GENERATEPAYROLL
		-- Create Monthly Payroll
		EXEC dbo.PaymentProcess_CreateMonthlyPayroll @Month = @FirstDayOfWorkingMonth
			,@PayrollXMLDoc = @PayrollXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '1. Create MonthlyPayroll - OK'

		-- Create MonthlyBasicSalary
		EXEC dbo.PaymentProcess_CreateMonthlyBasicSalary @Month = @FirstDayOfWorkingMonth
			,@BasicSalaryXMLDoc = @BasicSalaryXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '2. Create MonthlyBasicSalary - OK'

		-- Create MonthlySalary
		EXEC dbo.PaymentProcess_CreateMonthlySalary @Month = @FirstDayOfWorkingMonth
			,@SalaryXMLDoc = @SalaryXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '3. Create MonthlySalary - OK'

		-- Create MonthlySalaryAddition 
		EXEC dbo.PaymentProcess_CreateMonthlySalaryAddition @Month = @FirstDayOfWorkingMonth
			,@SalaryAdditionXMLDoc = @SalaryAdditionXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '4. Create MonthlySalaryAddition - OK'

		-- Create MonthlySalaryDeduction 
		EXEC dbo.PaymentProcess_CreateMonthlySalaryDeduction @Month = @FirstDayOfWorkingMonth
			,@SalaryDeductionXMLDoc = @SalaryDeductionXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '5. Create MonthlySalaryDeduction - OK'

		-- Create MonthlyBasicAllowance
		EXEC dbo.PaymentProcess_CreateMonthlyBasicAllowance @Month = @FirstDayOfWorkingMonth
			,@BasicAllowanceXMLDoc = @BasicAllowanceXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '6. Create MonthlyBasicAllowance - OK'

		-- Create MonthlyAllowance
		EXEC dbo.PaymentProcess_CreateMonthlyAllowance @Month = @FirstDayOfWorkingMonth
			,@AllowanceXMLDoc = @AllowanceXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '7. Create MonthlyAllowance - OK'

		-- Create MonthlyAllowanceAddition
		EXEC dbo.PaymentProcess_CreateMonthlyAllowanceAddition @Month = @FirstDayOfWorkingMonth
			,@AllowanceAdditionXMLDoc = @AllowanceAdditionXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '8. Create MonthlyAllowanceAddition - OK'
		
		-- Create MonthlyAllowanceDeduction
		EXEC dbo.PaymentProcess_CreateMonthlyAllowanceDeduction @Month = @FirstDayOfWorkingMonth
			,@AllowanceDeductionXMLDoc = @AllowanceDeductionXMLDoc
			,@UpdatedBy = @UpdatedBy
		PRINT '9. Create MonthlyAllowanceDeduction - OK'
		
		-- Update PaymentProcess
		UPDATE dbo.PaymentProcess
		WITH (ROWLOCK,UPDLOCK)
		SET IsPayrollGenerated = 1
		WHERE CompanyId = @CompanyId
			AND [Month] = @FirstDayOfWorkingMonth	
						
		-- Commit
		COMMIT TRANSACTION GENERATEPAYROLL

		PRINT 'End. GeneratePayroll - OK'
	END TRY

	BEGIN CATCH
		IF (@@TRANCOUNT > 0)
		BEGIN
			ROLLBACK TRANSACTION GENERATEPAYROLL
			PRINT 'Error-Rollback'
		END

		SET @ErrCode = 1
		SET @ErrMsg = 'ENumber: ' + CAST(ERROR_NUMBER() AS VARCHAR(50)) 
					+ ' --EProcedure: ' + ERROR_PROCEDURE()
					+ ' --ELine: ' + CAST(ERROR_LINE() AS VARCHAR(50))  
					+ ' --ErrorMessage: ' + ERROR_MESSAGE()

	END CATCH
END