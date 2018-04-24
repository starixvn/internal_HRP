
CREATE PROCEDURE [dbo].[Employee_GetEmployeeStatus] @UserId INT
AS
/*
Created: 20180410@Quinn.Vo
Task: get data for employee status
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'EmployeeManagement'

	CREATE TABLE #tblEmploymentStatus (
		Value TINYINT
		,[Text] NVARCHAR(500)
		,Checked BIT
		)

	INSERT INTO #tblEmploymentStatus (
			Value
			,[Text]
			,Checked
			)
		SELECT ResourceId AS Value
			,Value AS [Text]
			,1 AS Checked
		FROM dbo.RefResource WITH (NOLOCK)
		WHERE RefName = 'EmploymentStatus'
			AND IsActive = 1

	-- get employment status
	SELECT Value
		,[Text]
		,Checked
	FROM #tblEmploymentStatus WITH (NOLOCK)

	DROP TABLE #tblEmploymentStatus
END