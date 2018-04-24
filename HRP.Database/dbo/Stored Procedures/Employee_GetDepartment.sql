
CREATE PROCEDURE [dbo].[Employee_GetDepartment] @UserId INT
AS
/*
Created: 20180410@Quinn.Vo
Task: get department data
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'EmployeeManagement'

	CREATE TABLE #tblDivision (
		DivisionId SMALLINT
		,Code NVARCHAR(50)
		,CompanyId SMALLINT
		)

	CREATE TABLE #tblDepartment (
		DepartmentId SMALLINT
		,Code NVARCHAR(50)
		,DivisionId SMALLINT
		)

	INSERT INTO #tblDepartment (
		DepartmentId
		,Code
		,DivisionId
		)
	SELECT dept.DepartmentId
		,dept.Code
		,dept.DivisionId
	FROM dbo.Department AS dept WITH (NOLOCK)
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON dept.DivisionId = di.DivisionId
		AND di.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON di.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON c.CompanyId = uf.CompanyId
		AND uf.UserId = @UserId
		AND uf.IsRead = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.IsActive = 1
		AND f.FunctionKey = @functionKey
	WHERE dept.IsActive = 1

	INSERT INTO #tblDivision (
		DivisionId
		,Code
		,CompanyId
		)
	SELECT di.DivisionId
		,di.Code
		,di.CompanyId
	FROM dbo.Division AS di WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT DivisionId
		FROM #tblDepartment WITH (NOLOCK)
		) AS tbl ON tbl.DivisionId = di.DivisionId
	WHERE di.IsActive = 1

	-- get company
	SELECT c.CompanyId AS Id
		,c.Code
		,0 AS ParentId
		,0 AS CategoryId
	FROM dbo.Company AS c WITH (NOLOCK)
		INNER JOIN (
		SELECT DISTINCT CompanyId
		FROM #tblDivision WITH (NOLOCK)
		) AS tbl ON tbl.CompanyId = c.CompanyId
	WHERE c.IsActive = 1
	UNION
	-- get division
	SELECT DivisionId AS Id
		,Code
		,CompanyId AS ParentId
		,1 AS CategoryId
	FROM #tblDivision WITH (NOLOCK)
	UNION
	-- get department
	SELECT DepartmentId AS Id
		,Code
		,DivisionId AS ParentId
		,2 AS CategoryId
	FROM #tblDepartment WITH (NOLOCK)

	DROP TABLE #tblDepartment

	DROP TABLE #tblDivision
END