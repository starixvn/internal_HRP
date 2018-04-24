
CREATE PROCEDURE [dbo].[Employee_GetByEmployeeId] @EmployeeId INT = 0
	,@UserId INT = 0
	,@FunctionKey VARCHAR(50) = NULL
AS
/*
Created: 20180105@Raymond.Le
Task: Get Employee personal detail by EmpId
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	CREATE TABLE #tblCompanyPermission (
		EmployeeId INT
		,CompanyId SMALLINT
		,FunctionId SMALLINT
		,IsRead BIT
		,IsHost BIT
		)

	-- Check permission of user on Employee's company
	INSERT INTO #tblCompanyPermission (
		EmployeeId
		,CompanyId
		,FunctionId
		,IsRead
		,IsHost
		)
	SELECT @EmployeeId
		,uf.CompanyId
		,f.FunctionId
		,uf.IsRead
		,uf.IsHost
	FROM dbo.UserFunction AS uf WITH (NOLOCK)
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON em.CompanyId = uf.CompanyId
	WHERE f.FunctionKey = @FunctionKey
		AND f.IsActive = 1
		AND uf.UserId = @UserId
		AND em.EmployeeId = @EmployeeId

	SELECT e.[EmployeeId]
		,[NickName]
		,[FullName]
		,[Gender]
		,[DateOfBirth]
		,[PlaceOfBirth]
		,[Nationality]
		,[NRICNo]
		,[MaritalStatus]
		,[NoOfChildren]
		,[OverseasPhoneNo]
		,[LocalPhoneNo]
		,[OtherPhoneNo]
		,[PassportNo]
		,[DateIssue]
		,[ExpiredDate]
		,[PlaceOfIssue]
		,[Skype]
		,[PersonalEmail]
		,[PermanentAddress]
		,[ResidenceAddress]
		,[MailingAddress]
		,[ProfilePic]
		,cp.IsHost
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN #tblCompanyPermission AS cp WITH (NOLOCK) ON e.EmployeeId = cp.EmployeeId
	WHERE e.EmployeeId = @EmployeeId

	DROP TABLE #tblCompanyPermission
END