
CREATE PROCEDURE [dbo].[Employment_GetDataToUpdate] @UserId INT
	,@EmployeeId INT
AS
/*
Created: 20180105@Tim.Trinh
Task: get data to update
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @companyId SMALLINT
		,@divisionId SMALLINT
		,@departmentId SMALLINT
		,@sectionId SMALLINT
		,@costCenterId SMALLINT
		,@jobCategoryId SMALLINT
		,@gradeLevelId SMALLINT
		,@jobGradeId SMALLINT
		,@countryId SMALLINT
		,@cityId SMALLINT
		,@officeId SMALLINT
		,@functionKey VARCHAR(50) = 'EmployeeProfile'

	SELECT @companyId = e.CompanyId
		,@divisionId = e.DivisionId
		,@departmentId = e.DepartmentId
		,@sectionId = e.SectionId
		,@costCenterId = e.CostCenterId
		,@jobCategoryId = e.JobCategoryId
		,@gradeLevelId = e.GradeLevelId
		,@jobGradeId = e.JobGradeId
		,@countryId = e.CountryId
		,@cityId = e.CityId
		,@officeId = e.OfficeId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
	WHERE e.EmployeeId = @EmployeeId

	-- check permission
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UserId, @companyId, @functionKey) = 0
	BEGIN
		RETURN
	END

	-- get employment
	SELECT emp.EmployeeId
		,emp.CompanyId
		,emp.CompanyId AS OldCompanyId
		,ec.CompanyChangeDate
		,ec.CompanyReasonId
		,emp.LocalCompanyId
		,emp.LocalCompanyId AS OldLocalCompanyId
		,ec.LocalCompanyChangeDate
		,ec.LocalCompanyReasonId
		,emp.JobTitle
		,emp.JobTitle AS OldJobTitle
		,ec.JobTitleChangeDate
		,ec.JobTitleReasonId
		,emp.LocalJobTitleId
		,emp.LocalJobTitleId AS OldLocalJobTitleId
		,ec.LocalJobTitleChangeDate
		,ec.LocalJobTitleReasonId
		,emp.CompanyCode
		,emp.EmployeeCode
		,emp.EmployeeCode AS OldEmployeeCode
		,ec.IDNoChangeDate
		,ec.IDNoReasonId
		,emp.EmploymentType
		,emp.EmploymentStatus
		,emp.EmploymentStatus AS OldEmploymentStatus
		,ec.EmploymentStatusChangeDate
		,emp.JobCategoryId
		,emp.JobCategoryId AS OldJobCategoryId
		,ec.JobCategoryChangeDate
		,ec.JobCategoryReasonId
		,emp.JobGradeId
		,emp.JobGradeId AS OldJobGradeId
		,emp.GradeLevelId
		,emp.GradeLevelId AS OldGradeLevelId
		,ec.JobGradeChangeDate
		,ec.JobGradeReasonId
		,emp.DivisionId
		,emp.DivisionId AS OldDivisionId
		,ec.DivisionChangeDate
		,ec.DivisionReasonId
		,emp.DepartmentId
		,emp.DepartmentId AS OldDepartmentId
		,ec.DepartmentChangeDate
		,ec.DepartmentReasonId
		,emp.SectionId
		,emp.SectionId AS OldSectionId
		,ec.SectionChangeDate
		,ec.SectionReasonId
		,emp.CostCenterId
		,emp.CostCenterId AS OldCostCenterId
		,ec.CostCenterChangeDate
		,ec.CostCenterReasonId
		,emp.GroupStartDate
		,emp.GroupStartDate AS OldGroupStartDate
		,emp.GroupEndDate
		,emp.GroupEndDate AS OldGroupEndDate
		,ec.GroupDateReasonId
		,emp.CompanyStartDate
		,emp.CompanyStartDate AS OldCompanyStartDate
		,emp.CompanyEndDate
		,emp.CompanyEndDate AS OldCompanyEndDate
		,ec.CompanyDateReasonId
		,emp.Email
		,emp.CountryId
		,emp.CityId
		,emp.OfficeId
		,emp.DST
		,emp.DBT
		,emp.TaxIDNo
		,emp.SSSNo
		,emp.HealthInsNo
		,emp.HDMFNo
		,emp.PDFPassword
		,emp.FullNamePFileAdmin
		,emp.NicknamePFileAdmin
	FROM dbo.Employment AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmploymentHistory AS ec WITH (NOLOCK) ON emp.EmployeeId = ec.EmployeeId
	WHERE emp.EmployeeId = @EmployeeId

	SELECT wp.EmployeeId
		,wp.LocalCompanyId
		,wp.Type1
		,wp.StartDateType1
		,wp.EndDateType1
		,wp.Type2
		,wp.StartDateType2
		,wp.EndDateType2
	FROM dbo.EmploymentWorkPermit AS wp WITH (NOLOCK)
	WHERE wp.EmployeeId = @EmployeeId

	SELECT pm.EmployeeId
		,pm.FullNameBVISalary
		,pm.NicknameBVISalary
		,pm.FullNameBVIAllowance
		,pm.NicknameBVIAllowance
		,pm.FullNameLocalSalary
		,pm.NicknameLocalSalary
	FROM dbo.EmploymentPaymaster AS pm WITH (NOLOCK)
	WHERE pm.EmployeeId = @EmployeeId

	SELECT epa.EmployeeId
		,epa.FullNameAdministrator
		,epa.NicknameAdministrator
		,epa.FullNameEvaluator1
		,epa.NicknameEvaluator1
		,epa.FullNameEvaluator2
		,epa.NicknameEvaluator2
		,epa.FullNameReviewer
		,epa.NicknameReviewer
		,epa.FullNameManagement
		,epa.NicknameManagement
		,epa.FullNameIncrement
		,epa.NicknameIncrement
		,epa.FullNameBVIBonus
		,epa.NicknameBVIBonus
		,epa.FullNameLocalBonus
		,epa.NicknameLocalBonus
	FROM dbo.EmploymentEPA AS epa WITH (NOLOCK)
	WHERE epa.EmployeeId = @EmployeeId

	-- get form data
	-- company
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
	
	UNION
	SELECT CompanyId
		,Code
		,[Name]
	FROM dbo.Company WITH (NOLOCK)
	WHERE CompanyId = @companyId

	-- division
	SELECT DivisionId
		,[Code]
		,[Name]
	FROM dbo.Division WITH (NOLOCK)
	WHERE CompanyId = @companyId
		AND IsActive = 1
	
	UNION
	SELECT DivisionId
		,[Code]
		,[Name]
	FROM dbo.Division WITH (NOLOCK)
	WHERE DivisionId = @divisionId

	-- department
	SELECT DepartmentId
		,Code
		,[Name]
	FROM dbo.Department WITH (NOLOCK)
	WHERE DivisionId = @divisionId
		AND IsActive = 1
	
	UNION
	SELECT DepartmentId
		,Code
		,[Name]
	FROM dbo.Department WITH (NOLOCK)
	WHERE DepartmentId = @departmentId

	-- cost center
	SELECT CostCenterId
		,Code
		,[Name]
	FROM dbo.CostCenter WITH (NOLOCK)
	WHERE DepartmentId = @departmentId
		AND IsActive = 1
	
	UNION
	SELECT CostCenterId
		,Code
		,[Name]
	FROM dbo.CostCenter WITH (NOLOCK)
	WHERE CostCenterId = @costCenterId

	-- section
	SELECT SectionId
		,Code
		,[Name]
	FROM dbo.Section WITH (NOLOCK)
	WHERE CostCenterId = @costCenterId
		AND IsActive = 1
	
	UNION
	SELECT SectionId
		,Code
		,[Name]
	FROM dbo.Section WITH (NOLOCK)
	WHERE SectionId = @sectionId

	-- job category
	SELECT JobCategoryId
		,[Name]
		,[Description]
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE IsActive = 1
	
	UNION
	SELECT JobCategoryId
		,[Name]
		,[Description]
	FROM dbo.JobCategory WITH (NOLOCK)
	WHERE JobCategoryId = @jobCategoryId

	-- grade level
	SELECT GradeLevelId
		,[Name]
		,[Description]
	FROM dbo.GradeLevel WITH (NOLOCK)
	WHERE JobCategoryId = @jobCategoryId
		AND IsActive = 1
	
	UNION
	SELECT GradeLevelId
		,[Name]
		,[Description]
	FROM dbo.GradeLevel WITH (NOLOCK)
	WHERE GradeLevelId = @gradeLevelId

	-- job grade
	SELECT JobGradeId
		,[Name]
		,[Description]
	FROM dbo.JobGrade WITH (NOLOCK)
	WHERE GradeLevelId = @gradeLevelId
		AND IsActive = 1
	
	UNION
	SELECT JobGradeId
		,[Name]
		,[Description]
	FROM dbo.JobGrade WITH (NOLOCK)
	WHERE JobGradeId = @jobGradeId

	-- country
	SELECT CountryId
		,Code
		,[Name]
	FROM dbo.Country WITH (NOLOCK)
	WHERE IsActive = 1
	
	UNION
	SELECT CountryId
		,Code
		,[Name]
	FROM dbo.Country WITH (NOLOCK)
	WHERE CountryId = @countryId

	-- city
	SELECT CityId
		,Code
		,[Name]
	FROM dbo.City WITH (NOLOCK)
	WHERE CountryId = @countryId
		AND IsActive = 1
	
	UNION
	SELECT CityId
		,Code
		,[Name]
	FROM dbo.City WITH (NOLOCK)
	WHERE CityId = @cityId

	-- office
	SELECT OfficeId
		,Code
		,[Name]
	FROM dbo.Office WITH (NOLOCK)
	WHERE CityId = @cityId
		AND IsActive = 1
	
	UNION
	SELECT OfficeId
		,Code
		,[Name]
	FROM dbo.Office WITH (NOLOCK)
	WHERE OfficeId = @officeId

	-- ref resource
	SELECT RefName
		,ResourceId
		,Value
		,[Description]
	FROM dbo.RefResource WITH (NOLOCK)
	WHERE RefName IN ('LocalCompany', 'LocalJobTitle', 'EmploymentType', 'EmploymentStatus', 'WorkPermitType1', 'WorkPermitType2', 'CommonReasons', 'GroupDateReasons')
END