/*
Created: 201805030@Hunter.Tran
Task: Insert data for Payment Transaction Reports in Menu table
DB: bodb_HRP
Original:
Revisions:
Param's Explanation:
*/

DECLARE @ReportParentFunctionId INT,
		@ReportParentMenuId INT,
		@ParentFunctionKey NVARCHAR(50) = 'Report',
		@DirectBankingFunctionKey NVARCHAR(50) = 'DirectBankingReport',
		@DirectBankingFunctionId INT,
		@EbankingFunctionKey NVARCHAR(50) = 'EbankingReport',
		@EbankingFunctionId INT;

SET @ReportParentFunctionId = (
SELECT [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @ParentFunctionKey
)

-- Update Report parent link to NULL
UPDATE Menu
SET Menu.Link = NULL, Menu.FunctionId = @ReportParentFunctionId
WHERE Menu.Name = @ParentFunctionKey

-- Get DirectBankingReport function id
SET @DirectBankingFunctionId = (
SELECT [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @DirectBankingFunctionKey
)

-- Get EbankingReport function id
SET @EbankingFunctionId = (
SELECT [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @EbankingFunctionKey
)

-- Get Report Parent menu id
SET @ReportParentMenuId = (
SELECT Menu.MenuId
FROM Menu
WHERE Menu.ParentId IS NULL AND Menu.[Name] = @ParentFunctionKey)

-- Check if Direct Banking Report menu existed
IF EXISTS(
		SELECT Menu.MenuId
		FROM Menu
		WHERE Menu.FunctionId = @DirectBankingFunctionId)
	BEGIN
		-- Direct Banking/Cash Report and E-Banking Report existed
		UPDATE Menu
		SET Menu.ParentId = @ReportParentMenuId
		WHERE Menu.FunctionId = @DirectBankingFunctionId
	END
ELSE
	BEGIN
		-- Insert Payment transaction Reports
		INSERT INTO Menu VALUES (N'Direct Banking/Cash Report',NULL,N'/Report/DirectBanking',@ReportParentMenuId,@DirectBankingFunctionId,3,1)
	END

-- Check if Ebanking Report menu existed
IF EXISTS(
		SELECT Menu.MenuId
		FROM Menu
		WHERE Menu.FunctionId = @EbankingFunctionId)
	BEGIN
		-- Direct Banking/Cash Report and E-Banking Report existed
		UPDATE Menu
		SET Menu.ParentId = @ReportParentMenuId
		WHERE Menu.FunctionId = @EbankingFunctionId
	END
ELSE
	BEGIN
		-- Insert Payment transaction Reports
		INSERT INTO Menu VALUES (N'E-Banking Report',NULL,N'/Report/Ebanking',@ReportParentMenuId,@EbankingFunctionId,4,1)
	END