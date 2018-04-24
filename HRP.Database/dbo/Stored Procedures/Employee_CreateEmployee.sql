
CREATE PROCEDURE [dbo].[Employee_CreateEmployee] @EmployeeId INT OUTPUT
	,@FullName NVARCHAR(70)
	,@Nickname VARCHAR(30)
	,@Gender TINYINT
	,@DateOfBirth DATE
	,@PlaceOfBirth NVARCHAR(100) = NULL
	,@Nationality SMALLINT
	,@NRICNo VARCHAR(15)
	,@MaritalStatus TINYINT
	,@NoOfChildren TINYINT
	,@OverseasPhoneNo VARCHAR(15)
	,@LocalPhoneNo VARCHAR(15) = NULL
	,@OtherPhoneNo VARCHAR(15) = NULL
	,@PassportNo VARCHAR(15)
	,@DateIssue DATE
	,@ExpiredDate DATE
	,@PlaceOfIssue NVARCHAR(100) = NULL
	,@Skype VARCHAR(32) = NULL
	,@PersonalEmail VARCHAR(254) = NULL
	,@PermanentAddress NVARCHAR(500)
	,@ResidenceAddress NVARCHAR(500)
	,@MailingAddress NVARCHAR(500) = NULL
	,@PictureExtension VARCHAR(6) = NULL
	,@ECName NVARCHAR(70) = NULL
	,@ECRelationship NVARCHAR(70) = NULL
	,@ECAddress NVARCHAR(500) = NULL
	,@ECPhoneNo1 VARCHAR(15) = NULL
	,@ECPhoneNo2 NVARCHAR(15) = NULL
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
	,@WorkPermitCompanyId SMALLINT = NULL
	,@Type1 TINYINT = NULL
	,@StartDateType1 DATE = NULL
	,@EndDateType1 DATE = NULL
	,@Type2 TINYINT = NULL
	,@StartDateType2 DATE = NULL
	,@EndDateType2 DATE = NULL
	,@FullNamePFileAdmin NVARCHAR(70)
	,@NickNamePFileAdmin VARCHAR(30) = NULL
	,@FullNameBVISalary NVARCHAR(70)
	,@NickNameBVISalary VARCHAR(30) = NULL
	,@FullNameBVIAllowance NVARCHAR(70)
	,@NickNameBVIAllowance VARCHAR(30) = NULL
	,@FullNameLocalSalary NVARCHAR(70) = NULL
	,@NickNameLocalSalary VARCHAR(30) = NULL
	,@FullNameAdministrator NVARCHAR(70)
	,@NickNameAdministrator VARCHAR(30) = NULL
	,@FullNameEvaluator1 NVARCHAR(70) = NULL
	,@NickNameEvaluator1 VARCHAR(30) = NULL
	,@FullNameEvaluator2 NVARCHAR(70) = NULL
	,@NickNameEvaluator2 VARCHAR(30) = NULL
	,@FullNameReviewer NVARCHAR(70) = NULL
	,@NickNameReviewer VARCHAR(30) = NULL
	,@FullNameManagement NVARCHAR(70) = NULL
	,@NickNameManagement VARCHAR(30) = NULL
	,@FullNameIncrement NVARCHAR(70) = NULL
	,@NickNameIncrement VARCHAR(30) = NULL
	,@FullNameBVIBonus NVARCHAR(70) = NULL
	,@NickNameBVIBonus VARCHAR(30) = NULL
	,@FullNameLocalBonus NVARCHAR(70) = NULL
	,@NickNameLocalBonus VARCHAR(30) = NULL
	,@PDFPassword VARCHAR(2000)
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180103@Raymond.Le
Task: create employee
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
		,@xml XML

	-- Check permission
	IF dbo.fnc_GetFunctionPermissionOnCompany(@CreatedBy, @CompanyId, @functionKey) <> 2
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You do not have permission to create employee.'

		RETURN
	END

	-- check ID No.
	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
			WHERE e.CompanyId = @CompanyId
				AND e.EmployeeCode = @EmployeeCode
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Emp. Code must be unique. Please choose another Emp. Code.'

		RETURN
	END

	-- check nickname
	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
			WHERE e.CompanyId = @CompanyId
				AND emp.Nickname = @Nickname
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Nickname must be unique in 1 company.'

		RETURN
	END

	SET @startRun = GETDATE()

	INSERT INTO dbo.Employee (
		FullName
		,Nickname
		,Gender
		,DateOfBirth
		,PlaceOfBirth
		,Nationality
		,NRICNo
		,MaritalStatus
		,NoOfChildren
		,OverseasPhoneNo
		,LocalPhoneNo
		,OtherPhoneNo
		,PassportNo
		,DateIssue
		,ExpiredDate
		,PlaceOfIssue
		,Skype
		,PersonalEmail
		,PermanentAddress
		,ResidenceAddress
		,MailingAddress
		)
	VALUES (
		@FullName
		,@Nickname
		,@Gender
		,@DateOfBirth
		,@PlaceOfBirth
		,@Nationality
		,@NRICNo
		,@MaritalStatus
		,@NoOfChildren
		,@OverseasPhoneNo
		,@LocalPhoneNo
		,@OtherPhoneNo
		,@PassportNo
		,@DateIssue
		,@ExpiredDate
		,@PlaceOfIssue
		,@Skype
		,@PersonalEmail
		,@PermanentAddress
		,@ResidenceAddress
		,@MailingAddress
		)

	SET @EmployeeId = SCOPE_IDENTITY()

	--Update Profile picture link.
	IF (@PictureExtension IS NOT NULL)
	BEGIN
		UPDATE dbo.Employee WITH (UPDLOCK, ROWLOCK)
		SET ProfilePic = CAST(@EmployeeId AS VARCHAR(20)) + @PictureExtension
		WHERE EmployeeId = @EmployeeId
	END

	INSERT INTO dbo.EmergencyContact (
		EmployeeId
		,[Name]
		,Relationship
		,[Address]
		,PhoneNo1
		,PhoneNo2
		)
	VALUES (
		@EmployeeId
		,@ECName
		,@ECRelationship
		,@ECAddress
		,@ECPhoneNo1
		,@ECPhoneNo2
		)

	EXEC dbo.Employment_Create @EmployeeId = @EmployeeId
		,@CompanyId = @CompanyId
		,@CompanyChangeDate = @CompanyChangeDate
		,@CompanyReasonId = @CompanyReasonId
		,@LocalCompanyId = @LocalCompanyId
		,@LocalCompanyChangeDate = @LocalCompanyChangeDate
		,@LocalCompanyReasonId = @LocalCompanyReasonId
		,@JobTitle = @JobTitle
		,@JobTitleChangeDate = @JobTitleChangeDate
		,@JobTitleReasonId = @JobTitleReasonId
		,@LocalJobTitleId = @LocalJobTitleId
		,@LocalJobTitleChangeDate = @LocalJobTitleChangeDate
		,@LocalJobTitleReasonId = @LocalJobTitleReasonId
		,@EmployeeCode = @EmployeeCode
		,@IDNoChangeDate = @IDNoChangeDate
		,@IDNoReasonId = @IDNoReasonId
		,@EmploymentType = @EmploymentType
		,@EmploymentStatus = @EmploymentStatus
		,@EmploymentStatusChangeDate = @EmploymentStatusChangeDate
		,@JobCategoryId = @JobCategoryId
		,@JobCategoryChangeDate = @JobCategoryChangeDate
		,@JobCategoryReasonId = @JobCategoryReasonId
		,@JobGradeId = @JobGradeId
		,@GradeLevelId = @GradeLevelId
		,@JobGradeChangeDate = @JobGradeChangeDate
		,@JobGradeReasonId = @JobGradeReasonId
		,@DivisionId = @DivisionId
		,@DivisionChangeDate = @DivisionChangeDate
		,@DivisionReasonId = @DivisionReasonId
		,@DepartmentId = @DepartmentId
		,@DepartmentChangeDate = @DepartmentChangeDate
		,@DepartmentReasonId = @DepartmentReasonId
		,@SectionId = @SectionId
		,@SectionChangeDate = @SectionChangeDate
		,@SectionReasonId = @SectionReasonId
		,@CostCenterId = @CostCenterId
		,@CostCenterChangeDate = @CostCenterChangeDate
		,@CostCenterReasonId = @CostCenterReasonId
		,@GroupStartDate = @GroupStartDate
		,@GroupEndDate = @GroupEndDate
		,@GroupDateReasonId = @GroupDateReasonId
		,@CompanyStartDate = @CompanyStartDate
		,@CompanyEndDate = @CompanyEndDate
		,@CompanyDateReasonId = @CompanyDateReasonId
		,@Email = @Email
		,@CountryId = @CountryId
		,@CityId = @CityId
		,@OfficeId = @OfficeId
		,@DST = @DST
		,@DBT = @DBT
		,@TaxIDNo = @TaxIDNo
		,@SSSNo = @SSSNo
		,@HealthInsNo = @HealthInsNo
		,@HDMFNo = @HDMFNo
		,@WorkPermitCompanyId = @WorkPermitCompanyId
		,@Type1 = @Type1
		,@StartDateType1 = @StartDateType1
		,@EndDateType1 = @EndDateType1
		,@Type2 = @Type2
		,@StartDateType2 = @StartDateType2
		,@EndDateType2 = @EndDateType2
		,@FullNamePFileAdmin = @FullNamePFileAdmin
		,@NickNamePFileAdmin = @NickNamePFileAdmin
		,@FullNameBVISalary = @FullNameBVISalary
		,@NickNameBVISalary = @NickNameBVISalary
		,@FullNameBVIAllowance = @FullNameBVIAllowance
		,@NickNameBVIAllowance = @NickNameBVIAllowance
		,@FullNameLocalSalary = @FullNameLocalSalary
		,@NickNameLocalSalary = @NickNameLocalSalary
		,@FullNameAdministrator = @FullNameAdministrator
		,@NickNameAdministrator = @NickNameAdministrator
		,@FullNameEvaluator1 = @FullNameEvaluator1
		,@NickNameEvaluator1 = @NickNameEvaluator1
		,@FullNameEvaluator2 = @FullNameEvaluator2
		,@NickNameEvaluator2 = @NickNameEvaluator2
		,@FullNameReviewer = @FullNameReviewer
		,@NickNameReviewer = @NickNameReviewer
		,@FullNameManagement = @FullNameManagement
		,@NickNameManagement = @NickNameManagement
		,@FullNameIncrement = @FullNameIncrement
		,@NickNameIncrement = @NickNameIncrement
		,@FullNameBVIBonus = @FullNameBVIBonus
		,@NickNameBVIBonus = @NickNameBVIBonus
		,@FullNameLocalBonus = @FullNameLocalBonus
		,@NickNameLocalBonus = @NickNameLocalBonus
		,@PDFPassword = @PDFPassword
		,@CreatedBy = @CreatedBy
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	--Write Log
	SET @xml = (
			SELECT e.EmployeeId AS '@ei'
				,e.FullName AS '@fn'
				,e.Nickname AS '@nn'
				,e.Gender AS '@ge'
				,e.DateOfBirth AS '@dob'
				,e.PlaceOfBirth AS '@pob'
				,e.Nationality AS '@na'
				,e.NRICNo AS '@nr'
				,e.MaritalStatus AS '@ms'
				,e.NoOfChildren AS '@noc'
				,e.OverseasPhoneNo AS '@op'
				,e.LocalPhoneNo AS '@lp'
				,e.OtherPhoneNo AS '@otp'
				,e.PassportNo AS '@pn'
				,e.DateIssue AS '@di'
				,e.ExpiredDate AS '@ed'
				,e.PlaceOfIssue AS '@poi'
				,e.Skype AS '@sk'
				,e.PersonalEmail AS '@pe'
				,e.PermanentAddress AS '@pa'
				,e.ResidenceAddress AS '@ra'
				,e.MailingAddress AS '@ma'
				,e.ProfilePic AS '@pp'
			FROM dbo.[Employee] AS e WITH (NOLOCK)
			WHERE e.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @CreatedBy
		,@SPName = 'Employee_CreateEmployee'
		,@ObjectId = @EmployeeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = NULL
		,@Duration = @Duration
END