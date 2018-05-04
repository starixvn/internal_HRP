/*
Created: 201805030@Hunter.Tran
Task: Insert data for Payment Transaction Reports in Funtion table
DB: bodb_HRP
Original:
Revisions:
Param's Explanation:
*/

DECLARE @ReportParentFunctionId INT,
		@FunctionKey NVARCHAR(10) = 'Report',
		@DirectBankingFunctionKey NVARCHAR(50) = 'DirectBankingReport',
		@EbankingFunctionKey NVARCHAR(50) = 'EbankingReport';

SET @ReportParentFunctionId =
(
SELECT [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @FunctionKey
)

-- Check if Payment transaction Reports Function existed
IF EXISTS(
		SELECT [Function].FunctionId
		FROM [Function]
		WHERE [Function].[FunctionKey] IN (@DirectBankingFunctionKey, @EbankingFunctionKey))
	BEGIN
		-- Direct Banking/Cash Report and E-Banking Report existed
		UPDATE [Function]
		SET [Function].ParentId = @ReportParentFunctionId
		WHERE [Function].[FunctionKey] IN (@DirectBankingFunctionKey, @EbankingFunctionKey)
	END
ELSE
	BEGIN
		-- Insert Payment transaction Reports
		INSERT INTO [Function] VALUES (@DirectBankingFunctionKey,N'Direct Banking',@ReportParentFunctionId,0,1,0)
		INSERT INTO [Function] VALUES (@EbankingFunctionKey,N'E-Banking',@ReportParentFunctionId,0,1,0)
	END

-- Update permission for all system admin
CREATE TABLE #CompanyIds (CompanyId INT)
INSERT INTO #CompanyIds
SELECT Company.CompanyId
FROM Company
WHERE Company.IsActive = 1

CREATE TABLE #SystemAdminIds(UserId INT)
INSERT INTO #SystemAdminIds
SELECT [User].UserId
FROM [User]
WHERE [User].IsSystemAdmin = 1

--Insert for DirectBankingReport function
DECLARE @DirectBankingReportFunctionId INT;
SELECT @DirectBankingReportFunctionId = [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @DirectBankingFunctionKey

IF(@DirectBankingReportFunctionId IS NOT NULL)
BEGIN
	INSERT UserFunction (UserId, FunctionId, CompanyId, IsRead, IsHost)
	SELECT #SystemAdminIds.UserId,@DirectBankingReportFunctionId AS FunctionId,#CompanyIds.CompanyId,1 AS IsRead, 1 as IsHost
	FROM #SystemAdminIds, #CompanyIds
END

--Insert for EbankingReport function
DECLARE @EbankingReportFunctionId INT;
SELECT @EbankingReportFunctionId = [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @EbankingFunctionKey

IF(@EbankingReportFunctionId IS NOT NULL)
BEGIN
	INSERT UserFunction (UserId, FunctionId, CompanyId, IsRead, IsHost)
	SELECT #SystemAdminIds.UserId,@EbankingReportFunctionId AS FunctionId,#CompanyIds.CompanyId,1 AS IsRead, 1 as IsHost
	FROM #SystemAdminIds, #CompanyIds
END

DROP TABLE #CompanyIds
DROP TABLE #SystemAdminIds