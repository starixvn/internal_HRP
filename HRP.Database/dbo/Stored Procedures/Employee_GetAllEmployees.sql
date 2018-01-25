
CREATE PROCEDURE [dbo].[Employee_GetAllEmployees] @DepartmentIds VARCHAR(MAX) = ''
	,@EmployeeStatusIds VARCHAR(MAX) = ''
	,@EmployeeId INT = NULL
	,@EmployeeName VARCHAR (70) = ''
	,@UserId INT
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

	-- Spilit DepartmentIds
	CREATE TABLE #DepartmentList (DepartmentId INT);

	INSERT INTO #DepartmentList (DepartmentId)
	SELECT val
	FROM dbo.strSplit(@DepartmentIds, ',')

	--Spilit EmployeeStatusIds
	CREATE TABLE #EmployeeStatusList (EmployeeStatusIds INT);

	INSERT INTO #EmployeeStatusList (EmployeeStatusIds)
	SELECT val
	FROM dbo.strSplit(@EmployeeStatusIds, ',')

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
		INNER JOIN #EmployeeStatusList AS es WITH (NOLOCK) ON em.EmploymentStatus = es.EmployeeStatusIds
		LEFT JOIN dbo.RefResource AS ref WITH (NOLOCK) ON es.EmployeeStatusIds = ref.ResourceId
		INNER JOIN #DepartmentList AS dl ON dl.DepartmentId = em.DepartmentId
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
	WHERE   
		(ISNULL(@EmployeeId, em.EmployeeId) = em.EmployeeId
		AND (e.FullName LIKE 
				CASE
					WHEN (@EmployeeName = NULL AND @EmployeeId = NULL) or (@EmployeeId IS NOT NULL) THEN '%' + '' + '%'
					ELSE '%' + @EmployeeName + '%'
				END
			OR e.Nickname LIKE
				CASE
					WHEN (@EmployeeName = NULL AND @EmployeeId = NULL) or (@EmployeeId IS NOT NULL) THEN '%' + '' + '%'
					ELSE '%' + @EmployeeName + '%'
				END))
		AND ref.RefName = 'EmploymentStatus'
	ORDER BY em.CompanyCode, em.EmployeeCode

	DROP TABLE #DepartmentList

	DROP TABLE #EmployeeStatusList
END