
CREATE PROCEDURE [dbo].[Employment_GetByEmployeeId] @EmployeeId INT
AS
/*
Created: 20180105@Tim.Trinh
Task: get employment by employeeid
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	SELECT emp.EmployeeId
		,emp.CompanyId
		,ec.CompanyChangeDate
		,ec.CompanyReasonId
		,emp.LocalCompanyId
		,ec.LocalCompanyChangeDate
		,ec.LocalCompanyReasonId
		,emp.JobTitle
		,ec.JobTitleChangeDate
		,ec.JobTitleReasonId
		,emp.LocalJobTitleId
		,ec.LocalJobTitleChangeDate
		,ec.LocalJobTitleReasonId
		,c.Code AS CompanyCode
		,emp.EmployeeCode
		,ec.IDNoChangeDate
		,ec.IDNoReasonId
		,emp.EmploymentType
		,emp.EmploymentStatus
		,ec.EmploymentStatusChangeDate
		,emp.JobCategoryId
		,ec.JobCategoryChangeDate
		,ec.JobCategoryReasonId
		,emp.JobGradeId
		,emp.GradeLevelId
		,ec.JobGradeChangeDate
		,ec.JobGradeReasonId
		,emp.DivisionId
		,ec.DivisionChangeDate
		,ec.DivisionReasonId
		,emp.DepartmentId
		,ec.DepartmentChangeDate
		,ec.DepartmentReasonId
		,emp.SectionId
		,ec.SectionChangeDate
		,ec.SectionReasonId
		,emp.CostCenterId
		,ec.CostCenterChangeDate
		,ec.CostCenterReasonId
		,emp.GroupStartDate
		,emp.GroupEndDate
		,ec.GroupDateReasonId
		,emp.CompanyStartDate
		,emp.CompanyEndDate
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
		LEFT JOIN dbo.EmploymentHistory AS ec WITH (NOLOCK) ON emp.EmployeeId = ec.EmployeeId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
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
END