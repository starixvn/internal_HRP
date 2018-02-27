
CREATE PROCEDURE [dbo].[Employment_GetDataToCreate] @UserId INT
AS
/*
Created: 20180105@Tim.Trinh
Task: get data to create employment
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @functionKey VARCHAR(50) = 'EmployeeProfile'

	SELECT c.CompanyId
		,c.Code
		,c.[Name]
	FROM dbo.[User] AS u WITH (NOLOCK)
		INNER JOIN dbo.UserFunction AS uf WITH (NOLOCK) ON u.UserId = uf.UserId
		AND uf.IsHost = 1
		INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
		AND f.FunctionKey = @functionKey
		AND f.IsActive = 1
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON uf.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE u.UserId = @UserId

	SELECT JobCategoryId
		,[Name]
		,[Description]
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE IsActive = 1

	SELECT CountryId
		,Code
		,[Name]
	FROM dbo.Country WITH (NOLOCK)
	WHERE IsActive = 1

	SELECT RefName
		,ResourceId
		,Value
		,[Description]
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName IN ('LocalCompany', 'LocalJobTitle', 'EmploymentType', 'EmploymentStatus', 'WorkPermitType1', 'WorkPermitType2')
		AND IsActive = 1
END