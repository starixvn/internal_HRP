
CREATE PROCEDURE [dbo].[Employee_UpdateEmployee] @EmployeeId INT
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
	,@ProfilePic VARCHAR(200) = NULL
	,@ECName NVARCHAR(70) = NULL
	,@ECRelationship NVARCHAR(70) = NULL
	,@ECAddress NVARCHAR(500) = NULL
	,@ECPhoneNo1 VARCHAR(15) = NULL
	,@ECPhoneNo2 NVARCHAR(15) = NULL
	,@UpdatedBy INT
	,@FunctionKey VARCHAR(50)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20180110@Raymond.Le
Task: Update employee
DB: bodb_HRP
Original:

Revisions:
    20180124@Tim.Trinh: check nickname existed
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@oldLogData NVARCHAR(MAX)
		,@companyId SMALLINT
		,@xml XML

	SET @startRun = GETDATE()

	-- Check permission of user on Employee's company
	IF NOT EXISTS (
			SELECT 1
			FROM dbo.UserFunction AS uf WITH (NOLOCK)
				INNER JOIN dbo.[Function] AS f WITH (NOLOCK) ON uf.FunctionId = f.FunctionId
				INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON em.CompanyId = uf.CompanyId
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON em.EmployeeId = e.EmployeeId
			WHERE f.FunctionKey = @FunctionKey
				AND f.IsActive = 1
				AND uf.UserId = @UpdatedBy
				AND uf.IsHost = 1
				AND em.EmployeeId = @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = 'You do not have permission to update employee.'

		RETURN
	END

	-- check nickname
	SELECT @companyId = e.CompanyId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
	WHERE emp.EmployeeId = @EmployeeId

	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS emp WITH (NOLOCK)
				INNER JOIN dbo.Employment AS e WITH (NOLOCK) ON emp.EmployeeId = e.EmployeeId
			WHERE emp.Nickname = @Nickname
				AND e.CompanyId = @companyId
				AND emp.EmployeeId <> @EmployeeId
			)
	BEGIN
		SET @ErrCode = 1
		SET @ErrMsg = N'Nickname must be unique in 1 company.'

		RETURN
	END

	--
	IF EXISTS (
			SELECT 1
			FROM dbo.Employee AS e WITH (NOLOCK)
				INNER JOIN dbo.EmergencyContact AS ec WITH (NOLOCK) ON e.EmployeeId = ec.EmployeeId
			WHERE e.FullName = @FullName
				AND e.Nickname = @Nickname
				AND e.Gender = @Gender
				AND e.DateOfBirth = @DateOfBirth
				AND e.PlaceOfBirth = @PlaceOfBirth
				AND e.Nationality = @Nationality
				AND e.NRICNo = @NRICNo
				AND e.MaritalStatus = @MaritalStatus
				AND e.NoOfChildren = @NoOfChildren
				AND e.OverseasPhoneNo = @OverseasPhoneNo
				AND e.LocalPhoneNo = @LocalPhoneNo
				AND e.OtherPhoneNo = @OtherPhoneNo
				AND e.PassportNo = @PassportNo
				AND e.DateIssue = @DateIssue
				AND e.ExpiredDate = @ExpiredDate
				AND e.PlaceOfIssue = @PlaceOfIssue
				AND e.Skype = @Skype
				AND e.PersonalEmail = @PersonalEmail
				AND e.PermanentAddress = @PermanentAddress
				AND e.ResidenceAddress = @ResidenceAddress
				AND e.MailingAddress = @MailingAddress
				AND e.ProfilePic = @ProfilePic
				AND ec.[Name] = @ECName
				AND ec.Relationship = @ECRelationship
				AND ec.[Address] = @ECAddress
				AND ec.PhoneNo1 = @ECPhoneNo1
				AND ec.PhoneNo2 = @ECPhoneNo2
			)
	BEGIN
		RETURN
	END

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
				,ec.NAME AS '@ecn'
				,ec.Relationship AS '@ecr'
				,ec.[Address] AS '@eca'
				,ec.PhoneNo1 AS '@ecp1'
				,ec.PhoneNo2 AS '@ecp2'
			FROM dbo.[Employee] AS e WITH (NOLOCK)
				INNER JOIN dbo.EmergencyContact AS ec WITH (NOLOCK) ON e.EmployeeId = ec.EmployeeId
			WHERE e.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @oldLogData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')

	UPDATE [dbo].[Employee] WITH (ROWLOCK, UPDLOCK)
	SET FullName = @FullName
		,Nickname = @Nickname
		,Gender = @Gender
		,DateOfBirth = @DateOfBirth
		,PlaceOfBirth = @PlaceOfBirth
		,Nationality = @Nationality
		,NRICNo = @NRICNo
		,MaritalStatus = @MaritalStatus
		,NoOfChildren = @NoOfChildren
		,OverseasPhoneNo = @OverseasPhoneNo
		,LocalPhoneNo = @LocalPhoneNo
		,OtherPhoneNo = @OtherPhoneNo
		,PassportNo = @PassportNo
		,DateIssue = @DateIssue
		,ExpiredDate = @ExpiredDate
		,PlaceOfIssue = @PlaceOfIssue
		,Skype = @Skype
		,PersonalEmail = @PersonalEmail
		,PermanentAddress = @PermanentAddress
		,ResidenceAddress = @ResidenceAddress
		,MailingAddress = @MailingAddress
		,ProfilePic = @ProfilePic
	WHERE EmployeeId = @EmployeeId

	UPDATE dbo.EmergencyContact WITH (ROWLOCK, UPDLOCK)
	SET [Name] = @ECName
		,Relationship = @ECRelationship
		,[Address] = @ECAddress
		,PhoneNo1 = @ECPhoneNo1
		,PhoneNo2 = @ECPhoneNo2
	WHERE EmployeeId = @EmployeeId

	SET @ErrMsg = 'Update user successful'
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
				,ec.NAME AS '@ecn'
				,ec.Relationship AS '@ecr'
				,ec.[Address] AS '@eca'
				,ec.PhoneNo1 AS '@ecp1'
				,ec.PhoneNo2 AS '@ecp2'
			FROM dbo.[Employee] AS e WITH (NOLOCK)
				INNER JOIN dbo.EmergencyContact AS ec WITH (NOLOCK) ON e.EmployeeId = ec.EmployeeId
			WHERE e.EmployeeId = @EmployeeId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(max), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.LogDetail_Insert @UserId = @UpdatedBy
		,@SPName = 'Employee_UpdateEmployee'
		,@ObjectId = @EmployeeId
		,@ObjectTypeId = NULL
		,@Data = @logData
		,@OldData = @oldLogData
		,@Duration = @Duration
END