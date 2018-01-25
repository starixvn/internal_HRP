
CREATE PROCEDURE [dbo].[Employment_Create] @EmployeeId INT
	,@CompanyId SMALLINT
	,@CompanyChangeDate DATE = NULL
	,@CompanyReasonId TINYINT = NULL
	,@LocalCompanyId SMALLINT
	,@LocalCompanyChangeDate DATE = NULL
	,@LocalCompanyReasonId TINYINT = NULL
	,@JobTitle NVARCHAR(50)
	,@JobTitleChangeDate DATE = NULL
	,@JobTitleReasonId TINYINT = NULL
	,@LocalJobTitleId SMALLINT
	,@LocalJobTitleChangeDate DATE = NULL
	,@LocalJobTitleReasonId TINYINT = NULL
	,@CompanyCode VARCHAR(10)
	,@EmployeeCode VARCHAR(15)
	,@IDNoChangeDate DATE = NULL
	,@IDNoReasonId TINYINT = NULL
	,@EmploymentType TINYINT
	,@EmploymentStatus TINYINT
	,@EmploymentStatusChangeDate DATE = NULL
	,@JobCategoryId SMALLINT
	,@JobCategoryChangeDate DATE = NULL
	,@JobCategoryReasonId TINYINT = NULL
	,@JobGradeId SMALLINT
	,@GradeLevelId SMALLINT
	,@JobGradeChangeDate DATE = NULL
	,@JobGradeReasonId TINYINT = NULL
	,@DivisionId SMALLINT
	,@DivisionChangeDate DATE = NULL
	,@DivisionReasonId TINYINT = NULL
	,@DepartmentId SMALLINT
	,@DepartmentChangeDate DATE = NULL
	,@DepartmentReasonId TINYINT = NULL
	,@SectionId SMALLINT
	,@SectionChangeDate DATE = NULL
	,@SectionReasonId TINYINT = NULL
	,@CostCenterId SMALLINT
	,@CostCenterChangeDate DATE = NULL
	,@CostCenterReasonId TINYINT = NULL
	,@GroupStartDate DATE
	,@GroupEndDate DATE = NULL
	,@GroupDateReasonId TINYINT = NULL
	,@CompanyStartDate DATE = NULL
	,@CompanyEndDate DATE = NULL
	,@CompanyDateReasonId TINYINT = NULL
	,@Email VARCHAR(254)
	,@CountryId SMALLINT
	,@CityId SMALLINT
	,@OfficeId SMALLINT
	,@DST TINYINT = NULL
	,@DBT TINYINT = NULL
	,@TaxIDNo VARCHAR(50) = NULL
	,@SSSNo VARCHAR(50) = NULL
	,@HealthInsNo VARCHAR(50) = NULL
	,@HDMFNo VARCHAR(50) = NULL
	,@WorkPermitCompanyId SMALLINT
	,@Type1 TINYINT
	,@StartDateType1 DATE
	,@EndDateType1 DATE
	,@Type2 TINYINT = NULL
	,@StartDateType2 DATE = NULL
	,@EndDateType2 DATE = NULL
	,@FullNamePFileAdmin NVARCHAR(70)
	,@NicknamePFileAdmin VARCHAR(30) = NULL
	,@FullNameBVISalary NVARCHAR(70)
	,@NicknameBVISalary VARCHAR(30) = NULL
	,@FullNameBVIAllowance NVARCHAR(70)
	,@NicknameBVIAllowance VARCHAR(30) = NULL
	,@FullNameLocalSalary NVARCHAR(70) = NULL
	,@NicknameLocalSalary VARCHAR(30) = NULL
	,@FullNameAdministrator NVARCHAR(70)
	,@NicknameAdministrator VARCHAR(30) = NULL
	,@FullNameEvaluator1 NVARCHAR(70) = NULL
	,@NicknameEvaluator1 VARCHAR(30) = NULL
	,@FullNameEvaluator2 NVARCHAR(70) = NULL
	,@NicknameEvaluator2 VARCHAR(30) = NULL
	,@FullNameReviewer NVARCHAR(70) = NULL
	,@NicknameReviewer VARCHAR(30) = NULL
	,@FullNameManagement NVARCHAR(70) = NULL
	,@NicknameManagement VARCHAR(30) = NULL
	,@FullNameIncrement NVARCHAR(70) = NULL
	,@NicknameIncrement VARCHAR(30) = NULL
	,@FullNameBVIBonus NVARCHAR(70) = NULL
	,@NicknameBVIBonus VARCHAR(30) = NULL
	,@FullNameLocalBonus NVARCHAR(70) = NULL
	,@NicknameLocalBonus VARCHAR(30) = NULL
	,@PDFPassword VARCHAR(2000)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180105@Tim.Trinh
Task: create employment
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	-- For write log
	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML
		,@idoc INT

	SET @startRun = GETDATE()

	INSERT INTO dbo.Employment (
		EmployeeId
		,CompanyId
		,LocalCompanyId
		,JobTitle
		,LocalJobTitleId
		,CompanyCode
		,EmployeeCode
		,EmploymentType
		,EmploymentStatus
		,JobCategoryId
		,JobGradeId
		,GradeLevelId
		,DivisionId
		,DepartmentId
		,SectionId
		,CostCenterId
		,GroupStartDate
		,CompanyStartDate
		,Email
		,CountryId
		,CityId
		,OfficeId
		,DST
		,DBT
		,TaxIDNo
		,SSSNo
		,HealthInsNo
		,HDMFNo
		,PDFPassword
		,FullNamePFileAdmin
		,NicknamePFileAdmin
		)
	VALUES (
		@EmployeeId
		,@CompanyId
		,@LocalCompanyId
		,@JobTitle
		,@LocalJobTitleId
		,@CompanyCode
		,@EmployeeCode
		,@EmploymentType
		,@EmploymentStatus
		,@JobCategoryId
		,@JobGradeId
		,@GradeLevelId
		,@DivisionId
		,@DepartmentId
		,@SectionId
		,@CostCenterId
		,@GroupStartDate
		,@CompanyStartDate
		,@Email
		,@CountryId
		,@CityId
		,@OfficeId
		,@DST
		,@DBT
		,@TaxIDNo
		,@SSSNo
		,@HealthInsNo
		,@HDMFNo
		,@PDFPassword
		,@FullNamePFileAdmin
		,@NicknamePFileAdmin
		)

	INSERT INTO dbo.EmploymentWorkPermit (
		EmployeeId
		,LocalCompanyId
		,Type1
		,StartDateType1
		,EndDateType1
		,Type2
		,StartDateType2
		,EndDateType2
		)
	VALUES (
		@EmployeeId
		,@WorkPermitCompanyId
		,@Type1
		,@StartDateType1
		,@EndDateType1
		,@Type2
		,@StartDateType2
		,@EndDateType2
		)

	INSERT INTO dbo.EmploymentPaymaster (
		EmployeeId
		,FullNameBVISalary
		,NicknameBVISalary
		,FullNameBVIAllowance
		,NicknameBVIAllowance
		,FullNameLocalSalary
		,NicknameLocalSalary
		)
	VALUES (
		@EmployeeId
		,@FullNameBVISalary
		,@NicknameBVISalary
		,@FullNameBVIAllowance
		,@NicknameBVIAllowance
		,@FullNameLocalSalary
		,@NicknameLocalSalary
		)

	INSERT INTO dbo.EmploymentEPA (
		EmployeeId
		,FullNameAdministrator
		,NicknameAdministrator
		,FullNameEvaluator1
		,NicknameEvaluator1
		,FullNameEvaluator2
		,NicknameEvaluator2
		,FullNameReviewer
		,NicknameReviewer
		,FullNameManagement
		,NicknameManagement
		,FullNameIncrement
		,NicknameIncrement
		,FullNameBVIBonus
		,NicknameBVIBonus
		,FullNameLocalBonus
		,NicknameLocalBonus
		)
	VALUES (
		@EmployeeId
		,@FullNameAdministrator
		,@NicknameAdministrator
		,@FullNameEvaluator1
		,@NicknameEvaluator1
		,@FullNameEvaluator2
		,@NicknameEvaluator2
		,@FullNameReviewer
		,@NicknameReviewer
		,@FullNameManagement
		,@NicknameManagement
		,@FullNameIncrement
		,@NicknameIncrement
		,@FullNameBVIBonus
		,@NicknameBVIBonus
		,@FullNameLocalBonus
		,@NicknameLocalBonus
		)

	INSERT INTO dbo.EmploymentHistory (
		EmployeeId
		,CompanyChangeDate
		,CompanyReasonId
		,LocalCompanyChangeDate
		,LocalCompanyReasonId
		,JobTitleChangeDate
		,JobTitleReasonId
		,LocalJobTitleChangeDate
		,LocalJobTitleReasonId
		,GroupDateReasonId
		,CompanyDateReasonId
		,IDNoChangeDate
		,IDNoReasonId
		,EmploymentStatusChangeDate
		,JobCategoryChangeDate
		,JobCategoryReasonId
		,JobGradeChangeDate
		,JobGradeReasonId
		,DivisionChangeDate
		,DivisionReasonId
		,DepartmentChangeDate
		,DepartmentReasonId
		,SectionChangeDate
		,SectionReasonId
		,CostCenterChangeDate
		,CostCenterReasonId
		)
	VALUES (
		@EmployeeId
		,@CompanyChangeDate
		,@CompanyReasonId
		,@LocalCompanyChangeDate
		,@LocalCompanyReasonId
		,@JobTitleChangeDate
		,@JobTitleReasonId
		,@LocalJobTitleChangeDate
		,@LocalJobTitleReasonId
		,@GroupDateReasonId
		,@CompanyDateReasonId
		,@IDNoChangeDate
		,@IDNoReasonId
		,@EmploymentStatusChangeDate
		,@JobCategoryChangeDate
		,@JobCategoryReasonId
		,@JobGradeChangeDate
		,@JobGradeReasonId
		,@DivisionChangeDate
		,@DivisionReasonId
		,@DepartmentChangeDate
		,@DepartmentReasonId
		,@SectionChangeDate
		,@SectionReasonId
		,@CostCenterChangeDate
		,@CostCenterReasonId
		)

	-- write log
	SET @xml = (
			SELECT e.EmployeeId AS '@eid'
				,e.CompanyId AS '@cid'
				,ec.CompanyChangeDate AS '@ccd'
				,ec.CompanyReasonId AS '@crid'
				,e.LocalCompanyId AS '@lcid'
				,ec.LocalCompanyChangeDate AS '@lccd'
				,ec.LocalCompanyReasonId AS '@lcrid'
				,e.JobTitle AS '@jt'
				,ec.JobTitleChangeDate AS '@jtcd'
				,ec.JobTitleReasonId AS '@jtrid'
				,e.LocalJobTitleId AS '@ljtid'
				,ec.LocalJobTitleChangeDate AS '@ljtcd'
				,ec.LocalJobTitleReasonId AS '@ljtrid'
				,e.GroupStartDate AS '@grsd'
				,e.GroupEndDate AS '@gred'
				,ec.GroupDateReasonId AS '@grdrid'
				,e.CompanyStartDate AS '@csd'
				,e.CompanyEndDate AS '@ced'
				,ec.CompanyDateReasonId AS '@cdrid'
				,e.CompanyCode AS '@ccode'
				,e.EmployeeCode AS '@ecode'
				,ec.IDNoChangeDate AS '@idnocd'
				,ec.IDNoReasonId AS '@idnorid'
				,e.JobGradeId AS '@jgid'
				,e.GradeLevelId AS '@glid'
				,ec.JobGradeChangeDate AS '@jgcd'
				,ec.JobGradeReasonId AS '@jgrid'
				,e.DivisionId AS '@did'
				,ec.DivisionChangeDate AS '@dcd'
				,ec.DivisionReasonId AS '@drid'
				,e.DepartmentId AS '@deptid'
				,ec.DepartmentChangeDate AS '@deptcd'
				,ec.DepartmentReasonId AS '@deptrid'
				,e.SectionId AS '@sid'
				,ec.SectionChangeDate AS '@scd'
				,ec.SectionReasonId AS '@srid'
				,e.CostCenterId AS '@ccid'
				,ec.CostCenterChangeDate AS '@cccd'
				,ec.CostCenterReasonId AS '@ccrid'
				,e.Email AS '@mail'
				,e.CountryId AS '@countryid'
				,e.CityId AS '@cityid'
				,e.OfficeId AS '@officeid'
				,wp.LocalCompanyId AS '@wplcid'
				,wp.Type1 AS '@wpt1'
				,wp.StartDateType1 AS '@wpsdt1'
				,wp.EndDateType1 AS '@wpedt1'
				,wp.StartDateType2 AS '@wpsdt2'
				,wp.EndDateType2 AS '@wpedt2'
				,e.DST AS '@dst'
				,e.DBT AS '@dbt'
				,e.TaxIDNo AS '@taxidno'
				,e.SSSNo AS '@sssno'
				,e.HealthInsNo '@hino'
				,e.HDMFNo AS '@hdmfno'
				,e.FullNamePFileAdmin AS '@fnpfile'
				,e.NicknamePFileAdmin AS '@npfile'
				,p.FullNameBVISalary AS '@fnbvis'
				,p.NicknameBVISalary AS '@nbvis'
				,p.FullNameBVIAllowance AS '@fnbvia'
				,p.NicknameBVIAllowance AS '@nbvia'
				,p.FullNameLocalSalary AS '@fnls'
				,p.NicknameLocalSalary AS '@nls'
				,epa.FullNameAdministrator AS '@fnadmin'
				,epa.NicknameAdministrator AS '@nadmin'
				,epa.FullNameEvaluator1 AS '@fneva1'
				,epa.NicknameEvaluator1 AS '@neva1'
				,epa.FullNameEvaluator2 AS '@fneva2'
				,epa.NicknameEvaluator2 AS '@neva2'
				,epa.FullNameReviewer AS '@fnrev'
				,epa.NicknameReviewer AS '@nrev'
				,epa.FullNameManagement AS '@fnmgt'
				,epa.NicknameManagement AS '@nmgt'
				,epa.FullNameIncrement AS '@fnincre'
				,epa.NicknameIncrement AS '@nincre'
				,epa.FullNameBVIBonus AS '@fnbvibonus'
				,epa.NicknameBVIBonus AS '@nbvibonus'
				,epa.FullNameLocalBonus AS '@fnlbonus'
				,epa.NicknameLocalBonus AS '@nlbonus'
				,e.PDFPassword AS '@pdfpwd'
			FROM dbo.Employment AS e WITH (NOLOCK)
				INNER JOIN dbo.EmploymentWorkPermit AS wp WITH (NOLOCK) ON e.EmployeeId = wp.EmployeeId
				INNER JOIN dbo.EmploymentPaymaster AS p WITH (NOLOCK) ON e.EmployeeId = p.EmployeeId
				INNER JOIN dbo.EmploymentEPA AS epa WITH (NOLOCK) ON e.EmployeeId = epa.EmployeeId
				INNER JOIN dbo.EmploymentHistory AS ec WITH (NOLOCK) ON e.EmployeeId = ec.EmployeeId
			WHERE e.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Employment_Create'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END