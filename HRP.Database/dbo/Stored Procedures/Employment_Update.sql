
CREATE PROCEDURE [dbo].[Employment_Update] @EmployeeId INT
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
	,@IsPDFPasswordChanged BIT = 0
	,@UpdatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180110@Tim.Trinh
Task: update employment
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @functionKey VARCHAR(50) = 'EmployeeProfile'
		,@startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@nickname VARCHAR(30)
		,@xml XML
		,@idoc INT

	-- Check permission
	IF dbo.fnc_GetFunctionPermissionOnCompany(@UpdatedBy, @CompanyId, @functionKey) <> 2
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You do not have permission to update employment.'

		RETURN
	END

	-- check ID No.
	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
			WHERE e.EmployeeCode = @EmployeeCode
				AND e.CompanyId = @CompanyId
				AND emp.EmployeeId <> @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Emp. Code must be unique. Please choose another Emp. Code.'

		RETURN
	END

	-- check nickname
	SELECT @nickname = emp.Nickname
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
	WHERE emp.EmployeeId = @EmployeeId

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
			WHERE emp.Nickname = @nickname
				AND e.CompanyId = @CompanyId
				AND emp.EmployeeId <> @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Nickname must be unique in 1 company.'

		RETURN
	END

	-- data isn't change => return
	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
				INNER JOIN dbo.EmploymentHistory AS his WITH (NOLOCK) ON em.EmployeeId = his.EmployeeId
				INNER JOIN dbo.EmploymentWorkPermit AS wp WITH (NOLOCK) ON em.EmployeeId = wp.EmployeeId
				INNER JOIN dbo.EmploymentPaymaster AS p WITH (NOLOCK) ON em.EmployeeId = p.EmployeeId
				INNER JOIN dbo.EmploymentEPA AS epa WITH (NOLOCK) ON em.EmployeeId = epa.EmployeeId
			WHERE e.EmployeeId = @EmployeeId
				AND em.CompanyId = @CompanyId
				AND em.LocalCompanyId = @LocalCompanyId
				AND em.JobTitle = @JobTitle
				AND em.LocalJobTitleId = @LocalJobTitleId
				AND em.EmployeeCode = @EmployeeCode
				AND em.EmploymentType = @EmploymentType
				AND em.EmploymentStatus = @EmploymentStatus
				AND em.JobCategoryId = @JobCategoryId
				AND em.JobGradeId = @JobGradeId
				AND em.GradeLevelId = @GradeLevelId
				AND em.DivisionId = @DivisionId
				AND em.DepartmentId = @DepartmentId
				AND em.SectionId = @SectionId
				AND em.CostCenterId = @CostCenterId
				AND em.GroupStartDate = @GroupStartDate
				AND em.GroupEndDate = @GroupEndDate
				AND em.CompanyStartDate = @CompanyStartDate
				AND em.CompanyEndDate = @CompanyEndDate
				AND em.Email = @Email
				AND em.CountryId = @CountryId
				AND em.CityId = @CityId
				AND em.OfficeId = @OfficeId
				AND em.DST = @DST
				AND em.DBT = @DBT
				AND em.TaxIDNo = @TaxIDNo
				AND em.SSSNo = @SSSNo
				AND em.HealthInsNo = @HealthInsNo
				AND em.HDMFNo = @HDMFNo
				AND em.FullNamePFileAdmin = @FullNamePFileAdmin
				AND em.NicknamePFileAdmin = @NicknamePFileAdmin
				AND wp.LocalCompanyId = @WorkPermitCompanyId
				AND wp.Type1 = @Type1
				AND wp.StartDateType1 = @StartDateType1
				AND wp.EndDateType1 = @EndDateType1
				AND wp.Type2 = @Type2
				AND wp.StartDateType2 = @StartDateType2
				AND wp.EndDateType2 = @EndDateType2
				AND p.FullNameBVISalary = @FullNameBVISalary
				AND p.NicknameBVISalary = @NicknameBVISalary
				AND p.FullNameBVIAllowance = @FullNameBVIAllowance
				AND p.NicknameBVIAllowance = @NicknameBVIAllowance
				AND p.FullNameLocalSalary = @FullNameLocalSalary
				AND p.NicknameLocalSalary = @NicknameLocalSalary
				AND epa.FullNameAdministrator = @FullNameAdministrator
				AND epa.NicknameAdministrator = @NicknameAdministrator
				AND epa.FullNameEvaluator1 = @FullNameEvaluator1
				AND epa.NicknameEvaluator1 = @NicknameEvaluator1
				AND epa.FullNameEvaluator2 = @FullNameEvaluator2
				AND epa.NicknameEvaluator2 = @NicknameEvaluator2
				AND epa.FullNameReviewer = @FullNameReviewer
				AND epa.NicknameReviewer = @NicknameReviewer
				AND epa.FullNameManagement = @FullNameManagement
				AND epa.NicknameManagement = @NicknameManagement
				AND epa.FullNameIncrement = @FullNameIncrement
				AND epa.NicknameIncrement = @NicknameIncrement
				AND epa.FullNameBVIBonus = @FullNameBVIBonus
				AND epa.NicknameBVIBonus = @NicknameBVIBonus
				AND epa.FullNameLocalBonus = @FullNameLocalBonus
				AND epa.NicknameLocalBonus = @NicknameLocalBonus
				AND @IsPDFPasswordChanged = 1
			)
	BEGIN
		RETURN
	END

	-- update
	SET @startRun = GETDATE()
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

	UPDATE dbo.Employment WITH (ROWLOCK, UPDLOCK)
	SET CompanyId = @CompanyId
		,LocalCompanyId = @LocalCompanyId
		,JobTitle = @JobTitle
		,LocalJobTitleId = @LocalJobTitleId
		,EmployeeCode = @EmployeeCode
		,EmploymentType = @EmploymentType
		,EmploymentStatus = @EmploymentStatus
		,JobCategoryId = @JobCategoryId
		,JobGradeId = @JobGradeId
		,GradeLevelId = @GradeLevelId
		,DivisionId = @DivisionId
		,DepartmentId = @DepartmentId
		,SectionId = @SectionId
		,CostCenterId = @CostCenterId
		,GroupStartDate = @GroupStartDate
		,GroupEndDate = @GroupEndDate
		,CompanyStartDate = @CompanyStartDate
		,CompanyEndDate = @CompanyEndDate
		,Email = @Email
		,CountryId = @CountryId
		,CityId = @CityId
		,OfficeId = @OfficeId
		,DST = @DST
		,DBT = @DBT
		,TaxIDNo = @TaxIDNo
		,SSSNo = @SSSNo
		,HealthInsNo = @HealthInsNo
		,HDMFNo = @HDMFNo
		,PDFPassword = @PDFPassword
		,FullNamePFileAdmin = @FullNamePFileAdmin
		,NicknamePFileAdmin = @NicknamePFileAdmin
	WHERE EmployeeId = @EmployeeId

	UPDATE dbo.EmploymentWorkPermit WITH (ROWLOCK, UPDLOCK)
	SET LocalCompanyId = @WorkPermitCompanyId
		,Type1 = @Type1
		,StartDateType1 = @StartDateType1
		,EndDateType1 = @EndDateType1
		,Type2 = @Type2
		,StartDateType2 = @StartDateType2
		,EndDateType2 = @EndDateType2
	WHERE EmployeeId = @EmployeeId

	UPDATE dbo.EmploymentPaymaster WITH (ROWLOCK, UPDLOCK)
	SET FullNameBVISalary = @FullNameBVISalary
		,NicknameBVISalary = @NicknameBVISalary
		,FullNameBVIAllowance = @FullNameBVIAllowance
		,NicknameBVIAllowance = @NicknameBVIAllowance
		,FullNameLocalSalary = @FullNameLocalSalary
		,NicknameLocalSalary = @NicknameLocalSalary
	WHERE EmployeeId = @EmployeeId

	UPDATE dbo.EmploymentEPA WITH (ROWLOCK, UPDLOCK)
	SET FullNameAdministrator = @FullNameAdministrator
		,NicknameAdministrator = @NicknameAdministrator
		,FullNameEvaluator1 = @FullNameEvaluator1
		,NicknameEvaluator1 = @NicknameEvaluator1
		,FullNameEvaluator2 = @FullNameEvaluator2
		,NicknameEvaluator2 = @NicknameEvaluator2
		,FullNameReviewer = @FullNameReviewer
		,NicknameReviewer = @NicknameReviewer
		,FullNameManagement = @FullNameManagement
		,NicknameManagement = @NicknameManagement
		,FullNameIncrement = @FullNameIncrement
		,NicknameIncrement = @NicknameIncrement
		,FullNameBVIBonus = @FullNameBVIBonus
		,NicknameBVIBonus = @NicknameBVIBonus
		,FullNameLocalBonus = @FullNameLocalBonus
		,NicknameLocalBonus = @NicknameLocalBonus
	WHERE EmployeeId = @EmployeeId

	UPDATE dbo.EmploymentHistory WITH (ROWLOCK, UPDLOCK)
	SET CompanyChangeDate = @CompanyChangeDate
		,CompanyReasonId = @CompanyReasonId
		,LocalCompanyChangeDate = @LocalCompanyChangeDate
		,LocalCompanyReasonId = @LocalCompanyReasonId
		,JobTitleChangeDate = @JobTitleChangeDate
		,JobTitleReasonId = @JobTitleReasonId
		,LocalJobTitleChangeDate = @LocalJobTitleChangeDate
		,LocalJobTitleReasonId = @LocalJobTitleReasonId
		,GroupDateReasonId = @GroupDateReasonId
		,CompanyDateReasonId = @CompanyDateReasonId
		,IDNoChangeDate = @IDNoChangeDate
		,IDNoReasonId = @IDNoReasonId
		,EmploymentStatusChangeDate = @EmploymentStatusChangeDate
		,JobCategoryChangeDate = @JobCategoryChangeDate
		,JobCategoryReasonId = @JobCategoryReasonId
		,JobGradeChangeDate = @JobGradeChangeDate
		,JobGradeReasonId = @JobGradeReasonId
		,DivisionChangeDate = @DivisionChangeDate
		,DivisionReasonId = @DivisionReasonId
		,DepartmentChangeDate = @DepartmentChangeDate
		,DepartmentReasonId = @DepartmentReasonId
		,SectionChangeDate = @SectionChangeDate
		,SectionReasonId = @SectionReasonId
		,CostCenterChangeDate = @CostCenterChangeDate
		,CostCenterReasonId = @CostCenterReasonId
	WHERE EmployeeId = @EmployeeId

	-- write log after update	
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
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Employment_Update'
		,@ObjectId = @EmployeeId
		,@Data = @logData
		,@OldData = @logDataOld
		,@Duration = @Duration
END