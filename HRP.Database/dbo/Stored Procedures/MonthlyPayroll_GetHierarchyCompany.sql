
CREATE PROCEDURE [dbo].[MonthlyPayroll_GetHierarchyCompany] @UserId INT
	,@CompanyId SMALLINT
	,@CostCenterIds VARCHAR(MAX) = NULL
AS
/*
Created: 20180423@Tim.Trinh
Task: get hierarchy company
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'PaymentProcess'

	CREATE TABLE #tblCostCenterId (Id SMALLINT)

	CREATE TABLE #tblDepartment (
		DepartmentId SMALLINT
		,Code NVARCHAR(50)
		,DivisionId SMALLINT
		,Checked BIT
		)

	CREATE TABLE #tblCostCenter (
		CostCenterId SMALLINT
		,Code NVARCHAR(50)
		,DepartmentId SMALLINT
		,Checked BIT
		)

	INSERT INTO #tblCostCenterId (Id)
	SELECT val
	FROM dbo.strSplit(@CostCenterIds, ',')

	IF EXISTS (
			SELECT 1
			FROM #tblCostCenterId WITH (NOLOCK)
			)
	BEGIN
		INSERT INTO #tblCostCenter (
			CostCenterId
			,Code
			,DepartmentId
			,Checked
			)
		SELECT DISTINCT cc.CostCenterId
			,cc.Code
			,cc.DepartmentId
			,CASE 
				WHEN ISNULL(tbl.Id, 0) > 0 THEN 1
				ELSE 0
				END
		FROM dbo.CostCenter AS cc WITH (NOLOCK)
			INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON cc.DepartmentId = dept.DepartmentId
			AND dept.IsActive = 1
			INNER JOIN dbo.Division AS div WITH (NOLOCK) ON dept.DivisionId = div.DivisionId
			AND div.IsActive = 1
			INNER JOIN dbo.Company AS c WITH (NOLOCK) ON div.CompanyId = c.CompanyId
			AND c.CompanyId = @CompanyId
			AND c.IsActive = 1
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
			AND uf.UserId = @UserId
			AND uf.IsRead = 1
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @functionKey
			AND f.IsActive = 1
			LEFT JOIN #tblCostCenterId AS tbl WITH (NOLOCK) ON cc.CostCenterId = tbl.Id
		WHERE cc.IsActive = 1
	END
	ELSE
	BEGIN
		INSERT INTO #tblCostCenter (
			CostCenterId
			,Code
			,DepartmentId
			,Checked
			)
		SELECT DISTINCT cc.CostCenterId
			,cc.Code
			,cc.DepartmentId
			,1
		FROM dbo.CostCenter AS cc WITH (NOLOCK)
			INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON cc.DepartmentId = dept.DepartmentId
			AND dept.IsActive = 1
			INNER JOIN dbo.Division AS div WITH (NOLOCK) ON dept.DivisionId = div.DivisionId
			AND div.IsActive = 1
			INNER JOIN dbo.Company AS c WITH (NOLOCK) ON div.CompanyId = c.CompanyId
			AND c.CompanyId = @CompanyId
			AND c.IsActive = 1
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
			AND uf.UserId = @UserId
			AND uf.IsRead = 1
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @functionKey
			AND f.IsActive = 1
		WHERE cc.IsActive = 1
	END

	INSERT INTO #tblDepartment (
		DepartmentId
		,Code
		,DivisionId
		,Checked
		)
	SELECT dept.DepartmentId
		,dept.Code
		,dept.DivisionId
		,CASE 
			WHEN NOT EXISTS (
					SELECT 1
					FROM #tblCostCenter AS cc WITH (NOLOCK)
					WHERE cc.DepartmentId = dept.DepartmentId
						AND cc.Checked = 0
					) THEN 1
			ELSE 0
			END
	FROM dbo.Department AS dept WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT DepartmentId
		FROM #tblCostCenter WITH (NOLOCK)
		) AS data ON dept.DepartmentId = data.DepartmentId
	WHERE dept.IsActive = 1

	-- get division
	SELECT div.DivisionId AS Value
		,div.Code AS [Text]
		,(CASE 
			WHEN NOT EXISTS (
					SELECT 1
					FROM #tblDepartment AS dept WITH (NOLOCK)
					WHERE dept.DivisionId = div.DivisionId
						AND dept.Checked = 0
					) THEN 1
			ELSE 0 END
			) AS Checked
	FROM dbo.Division AS div WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT DivisionId
		FROM #tblDepartment WITH (NOLOCK)
		) AS data ON div.DivisionId = data.DivisionId
	WHERE div.IsActive = 1

	-- get department
	SELECT DepartmentId AS Value
		,Code AS [Text]
		,DivisionId AS ParentId
		,Checked
	FROM #tblDepartment WITH (NOLOCK)

	-- get cost center
	SELECT CostCenterId AS Value
		,Code AS [Text]
		,DepartmentId AS ParentId
		,Checked
	FROM #tblCostCenter WITH (NOLOCK)

	DROP TABLE #tblCostCenterId

	DROP TABLE #tblDepartment

	DROP TABLE #tblCostCenter
END