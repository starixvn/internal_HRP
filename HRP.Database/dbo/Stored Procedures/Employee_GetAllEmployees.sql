
CREATE PROCEDURE [dbo].[Employee_GetAllEmployees] @DepartmentIds VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
	,@EmployeeName VARCHAR(70) = ''
	,@UserId INT
	,@EmploymentStatusIds VARCHAR(MAX) = NULL
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

	CREATE TABLE #tblDepartmentId (Id SMALLINT)

	CREATE TABLE #tblEmployeeStatusId (Id TINYINT)

	INSERT INTO #tblDepartmentId (Id)
	SELECT val
	FROM dbo.strSplit(@DepartmentIds, ',')

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
		INNER JOIN #tblEmployeeStatusId AS es WITH (NOLOCK) ON em.EmploymentStatus = es.Id
		LEFT JOIN dbo.RefResource AS ref WITH (NOLOCK) ON es.Id = ref.ResourceId
		INNER JOIN #tblDepartmentId AS dl ON dl.Id = em.DepartmentId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON em.CompanyId = c.CompanyId
		INNER JOIN dbo.Division AS di WITH (NOLOCK) ON em.DivisionId = di.DivisionId
		INNER JOIN dbo.Department AS de WITH (NOLOCK) ON em.DepartmentId = de.DepartmentId
		LEFT JOIN dbo.UserFunction AS editprofile WITH (NOLOCK) ON c.CompanyId = editprofile.CompanyId
		AND editprofile.FunctionId = 6
		AND editprofile.UserId = @UserId
		LEFT JOIN dbo.UserFunction AS editpayroll WITH (NOLOCK) ON c.CompanyId = editpayroll.CompanyId
		AND editpayroll.FunctionId = 7
		AND editpayroll.UserId = @UserId
		LEFT JOIN dbo.UserFunction AS editbank WITH (NOLOCK) ON c.CompanyId = editbank.CompanyId
		AND editbank.FunctionId = 14
		AND editbank.UserId = @UserId
	WHERE (
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
		AND ref.RefName = 'EmploymentStatus'
	ORDER BY c.Code
		,em.EmployeeCode

	DROP TABLE #tblDepartmentId

	DROP TABLE #tblEmployeeStatusId
END