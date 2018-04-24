
CREATE PROCEDURE [dbo].[Employee_GetAllEmployees] @UserId INT
	,@EmployeeId INT = NULL
	,@EmployeeName VARCHAR(70) = ''
	,@DepartmentIds VARCHAR(MAX) = ''
	,@EmploymentStatusIds VARCHAR(MAX) = ''
AS
/*
Created: 20180115@Oliver.Dang
Task: get emmployees
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'EmployeeManagement'
		,@employeeProfile VARCHAR(50) = 'EmployeeProfile'
		,@employeePayroll VARCHAR(50) = 'EmployeePayroll'
		,@employeeBankDetails VARCHAR(50) = 'EmployeeBankDetails'

	CREATE TABLE #tblDepartmentId (Id SMALLINT)

	CREATE TABLE #tblEmployeeStatusId (Id TINYINT)

	IF ISNULL(@DepartmentIds, '') = ''
	BEGIN
		INSERT INTO #tblDepartmentId (Id)
		SELECT dept.DepartmentId
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
	END
	ELSE
	BEGIN
		INSERT INTO #tblDepartmentId (Id)
		SELECT val
		FROM dbo.strSplit(@DepartmentIds, ',')
	END

	INSERT INTO #tblEmployeeStatusId (Id)
	SELECT val
	FROM dbo.strSplit(@EmploymentStatusIds, ',')

	SELECT em.EmployeeId
		,em.EmployeeCode
		,e.Nickname
		,e.FullName
		,em.CompanyId
		,c.Code AS CompanyCode
		,c.[Name] AS CompanyName
		,di.Code AS DivisionCode
		,di.[Name] AS DivisionName
		,de.Code AS DepartmentCode
		,de.[Name] AS DepartmentName
		,em.JobTitle
		,ref.Value AS EmployeeStatus
		,(CASE 
			WHEN editprofile.IsHost = 1 THEN 2
			WHEN editprofile.IsRead = 1 THEN 1
			ELSE 0 END
			) AS EditProfilePolicy
		,(CASE 
			WHEN editpayroll.IsHost = 1 THEN 2
			WHEN editpayroll.IsRead = 1 THEN 1
			ELSE 0 END
			) AS EditPayRollPolicy
		,(CASE 
			WHEN editbank.IsHost = 1 THEN 2
			WHEN editbank.IsRead = 1 THEN 1
			ELSE 0 END
			) AS EditBankPolicy
	FROM dbo.Employment AS em WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON em.DivisionId = di.DivisionId
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON em.DepartmentId = de.DepartmentId
		INNER JOIN #tblDepartmentId AS deptid WITH (NOLOCK) ON de.DepartmentId = deptid.Id
		LEFT JOIN dbo.RefResource AS ref WITH (NOLOCK) ON em.EmploymentStatus = ref.ResourceId
		AND ref.RefName = 'EmploymentStatus'
		LEFT JOIN (
		SELECT DISTINCT uf.CompanyId
			,uf.IsRead
			,uf.IsHost
		FROM dbo.UserFunction AS uf WITH (NOLOCK)
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @employeeProfile
			AND f.IsActive = 1
		WHERE uf.UserId = @UserId
		) AS editprofile ON c.CompanyId = editprofile.CompanyId
		LEFT JOIN (
		SELECT DISTINCT uf.CompanyId
			,uf.IsRead
			,uf.IsHost
		FROM dbo.UserFunction AS uf WITH (NOLOCK)
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @employeePayroll
			AND f.IsActive = 1
		WHERE uf.UserId = @UserId
		) AS editpayroll ON c.CompanyId = editpayroll.CompanyId
		LEFT JOIN (
		SELECT DISTINCT uf.CompanyId
			,uf.IsRead
			,uf.IsHost
		FROM dbo.UserFunction AS uf WITH (NOLOCK)
			INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
			AND f.FunctionKey = @employeeBankDetails
			AND f.IsActive = 1
		WHERE uf.UserId = @UserId
		) AS editbank ON c.CompanyId = editbank.CompanyId
	WHERE (
			ISNULL(@EmploymentStatusIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tblEmployeeStatusId WITH (NOLOCK)
				WHERE Id = em.EmploymentStatus
				)
			)
		AND (
			ISNULL(@EmployeeId, em.EmployeeId) = em.EmployeeId
			AND (
				e.FullName LIKE CASE 
				WHEN (
						@EmployeeName = NULL
						AND @EmployeeId = NULL
						)
					OR (@EmployeeId IS NOT NULL) THEN '%' + '' + '%'
				ELSE '%' + @EmployeeName + '%' END
				OR e.Nickname LIKE CASE 
				WHEN (
						@EmployeeName = NULL
						AND @EmployeeId = NULL
						)
					OR (@EmployeeId IS NOT NULL) THEN '%' + '' + '%'
				ELSE '%' + @EmployeeName + '%' END
				)
			)
	ORDER BY c.Code
		,em.EmployeeCode

	DROP TABLE #tblDepartmentId

	DROP TABLE #tblEmployeeStatusId
END