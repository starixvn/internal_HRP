/*
Created: 201805030@Hunter.Tran
Task: Insert data for Payment Transaction Reports in Menu table
DB: bodb_HRP
Original:
Revisions:
Param's Explanation:
*/

DECLARE @ReportFunctionId INT,
		@ReportParentMenuId INT,
		@FunctionKey NVARCHAR(10) = 'Report';

SET @ReportFunctionId =
(
SELECT [Function].FunctionId
FROM [Function]
WHERE [Function].FunctionKey = @FunctionKey
)

-- Update Report parent link to NULL
UPDATE Menu
SET Menu.Link = NULL, Menu.FunctionId = @ReportFunctionId
WHERE Menu.Name = @FunctionKey

-- Get Report Parent menu id
SET @ReportParentMenuId = (
SELECT Menu.MenuId
FROM Menu
WHERE Menu.ParentId IS NULL AND Menu.[Name] = @FunctionKey)

-- Check if Payment transaction Reports existed
IF EXISTS(
		SELECT Menu.MenuId
		FROM Menu
		WHERE Menu.[Name] IN ('Direct Banking/Cash Report', 'E-Banking Report'))
	BEGIN
		-- Direct Banking/Cash Report and E-Banking Report existed
		UPDATE Menu
		SET Menu.ParentId = @ReportParentMenuId, Menu.FunctionId = @ReportFunctionId
		WHERE Menu.[Name] IN ('Direct Banking/Cash Report', 'E-Banking Report')
	END
ELSE
	BEGIN
		-- Insert Payment transaction Reports
		INSERT INTO Menu VALUES (N'Direct Banking/Cash Report',NULL,N'/Report/DirectBanking',@ReportParentMenuId,@ReportFunctionId,3,1)
		INSERT INTO Menu VALUES (N'E-Banking Report',NULL,N'/Report/Ebanking',@ReportParentMenuId,@ReportFunctionId,4,1)
	END