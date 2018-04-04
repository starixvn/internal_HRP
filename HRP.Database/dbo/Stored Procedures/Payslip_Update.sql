
CREATE PROCEDURE [dbo].[Payslip_Update] @UserId INT
	,@CompanyId SMALLINT = 0
	,@PayrollIds VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180328@Tim.Trinh
Task: update IsSent
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'Payslip'

	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @CompanyId, @functionKey) <> 2
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You do not have permission.'

		RETURN
	END

	CREATE TABLE #tblPayrollId (Id SMALLINT)

	INSERT INTO #tblPayrollId (Id)
	SELECT val
	FROM dbo.strSplit(@PayrollIds, ',')

	INSERT INTO dbo.Payslip (
		PayrollId
		,IsSent
		)
	SELECT tbl.Id
		,1
	FROM #tblPayrollId AS tbl WITH (NOLOCK)
		LEFT JOIN dbo.Payslip AS p WITH (NOLOCK) ON p.PayrollId = tbl.Id
	WHERE p.PayrollId IS NULL

	DROP TABLE #tblPayrollId
END