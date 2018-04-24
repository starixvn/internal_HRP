
CREATE PROCEDURE [dbo].[Report_GetDataForDynamicReport] @CompanyId SMALLINT
	,@Month DATE
	,@IsHasIDNo BIT = 0
	,@IsHasFullName BIT = 0
	,@IsHasNickName BIT = 0
	,@IsHasGender BIT = 0
	,@IsHasDateOfBirth BIT = 0
	,@IsHasPlaceOfBirth BIT = 0
	,@IsHasNationality BIT = 0
AS
/*
Created: 20180412@Raymond.Le
Task: Get data for dymanic Report
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	--SELECT (
	--		CASE 
	--			WHEN @IsHasFullName = 1
	--				THEN FullName
	--			ELSE NULL
	--			END
	--		) AS FullName
	--	,(
	--		CASE 
	--			WHEN @IsHasNickName = 1
	--				THEN Nickname
	--			ELSE NULL
	--			END
	--		) AS Nickname
	--	,e.Gender
	--	,NULL AS [DateOfBirth]
	--	,NULL AS [PlaceOfBirth]
	--	,NULL AS [Nationality]
	--	,NULL AS [NRICNo]
	--	,NULL AS [MaritalStatus]
	--	,NULL AS [NoOfChildren]
	--	,NULL AS [OverseasPhoneNo]
	--	,NULL AS [LocalPhoneNo]
	--	,NULL AS [OtherPhoneNo]
	--	,NULL AS [PassportNo]
	--	,NULL AS [DateIssue]
	--	,NULL AS [ExpiredDate]
	--	,NULL AS [PlaceOfIssue]
	--	,NULL AS [Skype]
	--	,NULL AS [PersonalEmail]
	--	,NULL AS [PermanentAddress]
	--	,NULL AS [ResidenceAddress]
	--	,NULL AS [MailingAddress]
	--	,NULL AS [ProfilePic]
	--FROM dbo.Employee AS e WITH (NOLOCK)
	--INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON e.EmployeeId = ep.EmployeeId
	--WHERE ep.CompanyId = @CompanyId

	CREATE TABLE #tblEmployee ([EmployeeId] INT)

	CREATE TABLE #tblEmployment (
		[EmployeeId] INT
		,[CompanyId] INT
		,[LocalCompanyId] INT
		)

	DECLARE @maxLength INT = 1
		,@i INT = 0

	WHILE @i <= @maxLength
	BEGIN
		INSERT INTO #tblEmployee (EmployeeId)
		SELECT e.EmployeeId
		FROM dbo.Employee AS e WITH (NOLOCK)

		SET @i = @i + 1
	END

	SELECT t.EmployeeId
		,FullName
		,Nickname
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
	FROM #tblEmployee AS t
	LEFT JOIN #tblEmployment AS te ON t.EmployeeId = te.EmployeeId
	LEFT JOIN dbo.Employee AS e WITH (NOLOCK) ON t.EmployeeId = e.EmployeeId
	INNER JOIN dbo.Employment AS ep WITH (NOLOCK) ON e.EmployeeId = ep.EmployeeId


	--WHERE ep.CompanyId = @CompanyId
	DROP TABLE #tblEmployee

	DROP TABLE #tblEmployment
END