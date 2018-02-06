
CREATE PROCEDURE [dbo].[Employee_GetDataFilterForIndexPage] @UserId INT
	,@EmploymentStatusIds VARCHAR(MAX) = NULL
	,@DepartmentIds VARCHAR(MAX) = NULL
AS
/*
Created: 20180205@Tim.Trinh
Task: get data for index page
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN	
	SET NOCOUNT ON;
	
	DECLARE @functionId SMALLINT = 5
	
	CREATE TABLE #tblDepartmentId(Id SMALLINT)
	CREATE TABLE #tblEmploymentStatusId(Id TINYINT)
	CREATE TABLE #tblEmploymentStatus(Value TINYINT, [Text] NVARCHAR(500), Checked BIT)
	CREATE TABLE #tblDivision(DivisionId SMALLINT, [Code] VARCHAR(10), CompanyId SMALLINT, Checked BIT)
	CREATE TABLE #tblDepartment(DepartmentId SMALLINT, [Code] VARCHAR(10), DivisionId SMALLINT, Checked BIT)
	
	INSERT INTO #tblDepartmentId(Id)
	SELECT val
	FROM dbo.strSplit(@DepartmentIds, ',')
	
	INSERT INTO #tblEmploymentStatusId(Id)
	SELECT val
	FROM dbo.strSplit(@EmploymentStatusIds, ',')
	
	IF EXISTS(SELECT 1 FROM #tblEmploymentStatusId WITH (NOLOCK))
	BEGIN
		INSERT INTO #tblEmploymentStatus(Value, [Text], Checked)
		SELECT ref.ResourceId AS Value
			,ref.Value AS [Text]
			,CASE WHEN tbl.Id IS NOT NULL THEN 1 ELSE 0 END AS Checked
		FROM dbo.RefResource AS ref WITH (NOLOCK)
			LEFT JOIN #tblEmploymentStatusId AS tbl WITH (NOLOCK) ON ref.ResourceId = tbl.Id
		WHERE RefName = 'EmploymentStatus'
	END
	ELSE
	BEGIN
		INSERT INTO #tblEmploymentStatus(Value, [Text], Checked)
		SELECT ResourceId AS Value
			,Value AS [Text]
			,1 AS Checked
		FROM dbo.RefResource WITH (NOLOCK)
		WHERE RefName = 'EmploymentStatus'
	END
	
	IF EXISTS (SELECT 1 FROM #tblDepartmentId WITH (NOLOCK))
	BEGIN
		INSERT INTO #tblDepartment(DepartmentId, Code, DivisionId, Checked)
		SELECT dept.DepartmentId
			,dept.Code
			,dept.DivisionId
			,CASE WHEN tbl.Id IS NOT NULL THEN 1 ELSE 0 END
		FROM dbo.Department AS dept WITH (NOLOCK)
			INNER JOIN dbo.Division AS di WITH (NOLOCK) ON dept.DivisionId = di.DivisionId
			AND di.IsActive = 1
			INNER JOIN dbo.Company AS c WITH (NOLOCK) ON di.CompanyId = c.CompanyId
			AND c.IsActive = 1
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
			AND uf.UserId = @UserId
			AND uf.FunctionId = @functionId
			AND uf.IsRead = 1
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.IsActive = 1
			LEFT JOIN #tblDepartmentId AS tbl WITH (NOLOCK) ON dept.DepartmentId = tbl.Id
		WHERE dept.IsActive = 1
	END
	ELSE
	BEGIN
		INSERT INTO #tblDepartment(DepartmentId, Code, DivisionId, Checked)
		SELECT dept.DepartmentId
			,dept.Code
			,dept.DivisionId
			,1
		FROM dbo.Department AS dept WITH (NOLOCK)
			INNER JOIN dbo.Division AS di WITH (NOLOCK) ON dept.DivisionId = di.DivisionId
			AND di.IsActive = 1
			INNER JOIN dbo.Company AS c WITH (NOLOCK) ON di.CompanyId = c.CompanyId
			AND c.IsActive = 1
			INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
			AND uf.UserId = @UserId
			AND uf.FunctionId = @functionId
			AND uf.IsRead = 1
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.IsActive = 1
		WHERE dept.IsActive = 1
	END
	
	INSERT INTO #tblDivision(DivisionId, Code, CompanyId, Checked)
	SELECT di.DivisionId
		,di.Code
		,di.CompanyId
		,CASE WHEN NOT EXISTS(SELECT 1 FROM #tblDepartment AS dept WITH (NOLOCK) WHERE dept.DivisionId = di.DivisionId AND dept.Checked = 0) THEN 1 ELSE 0 END
	FROM dbo.Division AS di WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON di.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
		AND uf.UserId = @UserId
		AND uf.FunctionId = @functionId
		AND uf.IsRead = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.IsActive = 1
	WHERE di.IsActive = 1
	
	-- get data filter
	SELECT STUFF((
				SELECT ',' + CAST(DepartmentId AS VARCHAR(20))
				FROM #tblDepartment WITH (NOLOCK)
				WHERE Checked = 1
				FOR XML PATH('')
				), 1, 1, '') AS DepartmentIds
		,STUFF((
				SELECT ',' + CAST(Value AS VARCHAR(20))
				FROM #tblEmploymentStatus WITH (NOLOCK)
				WHERE Checked = 1
				FOR XML PATH('')
				), 1, 1, '') AS EmploymentStatusIds
	
	-- get company
	SELECT c.CompanyId AS Value
		,c.Code AS [Text]
		,CASE WHEN NOT EXISTS(SELECT 1 FROM #tblDivision AS di WITH (NOLOCK) WHERE di.CompanyId = c.CompanyId AND di.Checked = 0) THEN 1 ELSE 0 END AS Checked
	FROM dbo.Company AS c WITH (NOLOCK)
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
	WHERE uf.UserId = @UserId
		AND uf.FunctionId = 5
		AND uf.IsRead = 1
		AND c.IsActive = 1

	-- get division
	SELECT DivisionId AS Value
		,Code AS [Text]
		,CompanyId AS ParentId
		,Checked
	FROM #tblDivision WITH (NOLOCK)

	-- get department
	SELECT DepartmentId AS Value
		,Code AS [Text]
		,DivisionId AS ParentId
		,Checked
	FROM #tblDepartment WITH (NOLOCK)
	
	-- get employment status
	SELECT Value
		,[Text]
		,Checked
	FROM #tblEmploymentStatus WITH (NOLOCK)
	
	DROP TABLE #tblDepartmentId
	DROP TABLE #tblEmploymentStatusId
	DROP TABLE #tblDepartment
	DROP TABLE #tblDivision
END