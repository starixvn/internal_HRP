CREATE PROCEDURE [dbo].[Report_GetEmployeeDataForDynamicReport] 
	@EmployeeStatusIds VARCHAR(200)
	,@CompanyId INT
AS
/*
Created: 20180423@Elton.Ton
Task: Get report with selected column
DB: bodb_HRP
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmployeeId
	,e.FullName
	,e.Nickname
	,genderRef.Value AS 'Gender'
	,e.DateOfBirth
	,e.PlaceOfBirth
	,nationRef.Value AS 'Nationality'
	,e.NRICNo
	,martitalStatusRef.Value AS 'MaritalStatus'
	,noOfChildRenRef.Value AS 'NoOfChildren'
	,e.OverseasPhoneNo
	,e.LocalPhoneNo
	,e.OtherPhoneNo
	,e.PassportNo
	,e.DateIssue
	,e.ExpiredDate
	,e.PlaceOfIssue
	,e.Skype
	,e.PersonalEmail
	,e.PermanentAddress
	,e.ResidenceAddress
	,e.MailingAddress
	,ec.Name AS 'EmergencyContactName'
	,ec.Relationship AS 'EmergencyContactRelationship'
	,ec.Address AS 'EmergencyContactAddress'
	,ec.PhoneNo1 AS 'EmergencyContactPhoneNo1'
	,ec.PhoneNo2 AS 'EmergencyContactPhoneNo2'
	,c.Code AS 'Company'
	,emh.CompanyChangeDate
	,bviChangeRef.Value AS 'CompanyReason'
	,localCompany.Value AS 'LocalCompany'
	,emh.LocalCompanyChangeDate
	,localChangeRef.Value AS 'LocalCompanyReason'
	,em.JobTitle
	,emh.JobTitleChangeDate
	,jobTitleReasonRef.Value AS 'JobTitleReason'
	,localJobTitleRef.Value AS 'LocalJobTitle'
	,emh.LocalJobTitleChangeDate
	,localJobTitleReasonRef.Value AS 'LocalJobTitleReason'
	,em.GroupStartDate
	,em.GroupEndDate
	,groupDateReasonRef.Value AS 'GroupDateReason'
	,em.CompanyStartDate
	,em.CompanyEndDate
	,companyDateReasonRef.Value AS 'CompanyDateReason'
	,c.Code + em.EmployeeCode AS 'IDNo'
	,emh.IDNoChangeDate
	,idChangeReasonRef.Value AS 'IDNoChangeReason'
	,employmentTypeRef.Value AS 'EmploymentType'
	,employmentStatusRef.Value AS 'EmploymentStatus'
	,emh.EmploymentStatusChangeDate
	,jc.Name AS 'JobCategory'
	,emh.JobCategoryChangeDate
	,categoryChangeReasonRef.Value AS 'JobCategoryReason'
	,gl.Name AS 'GradeLevel'
	,jg.Name AS 'JobGrade'
	,emh.JobGradeChangeDate
	,jobGradeChangeReasonRef.Value AS 'JobGradeReason'
	,d.Code AS 'Division'
	,emh.DivisionChangeDate
	,divisionChangeReasonRef.Value AS 'DivisionReason'
	,dm.Code AS 'Department'
	,emh.DepartmentChangeDate
	,departmentChangeReasonRef.Value AS 'DepartmentReason'
	,cc.Code AS 'CostCenter'
	,emh.CostCenterChangeDate
	,costcenterChangeReasonRef.Value AS 'CostCenterReason'
	,s.Code AS 'Section'
	,emh.SectionChangeDate
	,sectionChangeReasonRef.Value AS 'SectionReason'
	,em.Email
	,country.Value AS 'Country'
	,city.Value AS 'City'
	,office.Value AS 'Office'
	,wpCompanyRef.Value AS 'WorkPermitCompany'
	,wpType1Ref.Value AS 'WorkPermitType1'
	,ewp.StartDateType1
	,ewp.EndDateType1
	,wpType2Ref.Value AS 'WorkPermitType2'
	,ewp.StartDateType2
	,ewp.EndDateType2
	,dstCompanyRef.Value AS 'DSTCompany'
	,dbtCompanyRef.Value AS 'DBTCompany'
	,em.TaxIDNo
	,em.SSSNo
	,em.HealthInsNo
	,em.HDMFNo
	,em.FullNamePFileAdmin
	,em.NicknamePFileAdmin
	,emp.FullNameBVISalary
	,emp.NickNameBVISalary
	,emp.FullNameBVIAllowance
	,emp.NickNameBVIAllowance
	,emp.FullNameLocalSalary
	,emp.NickNameLocalSalary
	,eepa.FullNameAdministrator
    ,eepa.NicknameAdministrator
    ,eepa.FullNameEvaluator1
    ,eepa.NicknameEvaluator1
    ,eepa.FullNameEvaluator2
    ,eepa.NicknameEvaluator2
    ,eepa.FullNameReviewer
    ,eepa.NicknameReviewer
    ,eepa.FullNameManagement
    ,eepa.NicknameManagement
    ,eepa.FullNameIncrement
    ,eepa.NicknameIncrement
    ,eepa.FullNameBVIBonus
    ,eepa.NicknameBVIBonus
    ,eepa.FullNameLocalBonus
    ,eepa.NicknameLocalBonus
	,em.PDFPassword
	,epbs.Salary
	,epbs.CurrencyId
	,bsc.CurrencyCode AS 'CurrencyCode'
	,epbsh.EffectiveDate
	,epbsh.Reason
	,epbs.WorkingDays
	,epbs.EbankingCurrencyId
	,bsec.CurrencyCode AS 'EbankingCurrencyCode'
	,epbs.EbankingAmount
	,ebaRef.Value AS 'EbankingAccount'
	,epbs.DirectBanking1CurrencyId
	,bsd1c.CurrencyCode AS 'DirectBanking1CurrencyCode'
	,epbs.DirectBanking1Amount
	,db1aRef.Value AS 'DirectBanking1Account'
	,epbs.DirectBanking2CurrencyId
	,bsd2c.CurrencyCode AS 'DirectBanking2CurrencyCode'
	,epbs.DirectBanking2Amount
	,db2aRef.Value AS 'DirectBanking2Account'
	,epbs.CashCurrencyId
	,bscc.CurrencyCode AS 'CashCurrencyCode'
	,epbs.CashAmount
	,caRef.Value AS 'CashAccount'
	,epbs.AutoCalculateOn
	,epa.CurrencyId AS 'AllowanceCurrencyId'
	,epac.CurrencyCode AS 'AllowanceCurrencyCode'
	,epa.DirectBanking1CurrencyId AS 'AllowanceDirectBanking1CurrencyId'
	,epad1c.CurrencyCode AS 'AllowanceDirectBanking1CurrencyCode'
	,epa.DirectBanking1Amount AS 'AllowanceDirectBanking1Amount'
	,epadb1aRef.Value AS 'AllowanceDirectBanking1Account'
	,epa.DirectBanking2CurrencyId AS 'AllowanceDirectBanking2CurrencyId'
	,epad2c.CurrencyCode AS 'AllowanceDirectBanking2CurrencyCode'
	,epa.DirectBanking2Amount AS 'AllowanceDirectBanking2Amount'
	,epadb2aRef.Value AS 'AllowanceDirectBanking2Account'
	,epa.CashCurrencyId AS 'AllowanceCashCurrencyId'
	,epacc.CurrencyCode AS 'AllowanceCashCurrencyCode'
	,epa.CashAmount AS 'AllowanceCashAmount'
	,epacaRef.Value AS 'AllowanceCashAccount'
	,epa.AutoCalculateOn AS 'AllowanceAutoCalculateOn'
	,epa.SubsistenceAmount
	,epa.SubsistenceCurrencyId
	,sstc.CurrencyCode AS 'SubsistenceCurrencyCode'
	,epa.HousingAmount
	,epa.HousingCurrencyId
	,hc.CurrencyCode AS 'HousingCurrencyCode'
	,epa.HomePassageAmount
	,epa.HomePassageCurrencyId
	,hpc.CurrencyCode AS 'HomePassageCurrencyCode'
	,epa.DutyMealAmount
	,epa.DutyMealCurrencyId
	,dmc.CurrencyCode AS 'DutyMealCurrencyCode'
	,epa.HPIAmount
	,epa.HPICurrencyId
	,hpic.CurrencyCode AS 'HPICurrencyCode'
	,epa.MobilePhoneAmount
	,epa.MobilePhoneCurrencyId
	,mpc.CurrencyCode AS 'MobilePhoneCurrencyCode'
	,epa.OthersAmount
	,epa.OthersCurrencyId
	,otc.CurrencyCode AS 'OthersCurrencyCode'
	,epa.SecondmentAmount
	,epa.SecondmentCurrencyId
	,smc.CurrencyCode AS 'SecondmentCurrencyCode'
	,epdrmc.CurrencyCode AS 'RoomingChargesCurrencyCode'
	,epd.RoomingChargesAmount
	,epdffc.CurrencyCode AS 'FitnessFirstCurrencyCode'
	,epd.FitnessFirstAmount
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.RefResource AS genderRef WITH (NOLOCK) ON e.Gender = genderRef.ResourceId AND genderRef.RefName = 'Gender'
	INNER JOIN dbo.RefResource AS nationRef WITH (NOLOCK) ON e.Nationality = nationRef.ResourceId AND nationRef.RefName = 'Nationality'
	INNER JOIN dbo.RefResource AS martitalStatusRef WITH (NOLOCK) ON e.MaritalStatus = martitalStatusRef.ResourceId AND martitalStatusRef.RefName = 'MaritalStatus'
	INNER JOIN dbo.RefResource AS noOfChildRenRef WITH (NOLOCK) ON e.NoOfChildren = noOfChildRenRef.ResourceId AND noOfChildRenRef.RefName = 'NoOfChildren'
	INNER JOIN dbo.EmergencyContact AS ec WITH (NOLOCK) ON e.EmployeeId = ec.EmployeeId
	INNER JOIN dbo.Employment AS em WITH (NOLOCK) ON e.EmployeeId = em.EmployeeId
	INNER JOIN dbo.Company AS c WITH (NOLOCK) ON c.CompanyId = em.CompanyId
	INNER JOIN dbo.JobCategory AS jc WITH (NOLOCK) ON em.JobCategoryId = jc.JobCategoryId
	INNER JOIN dbo.JobGrade AS jg WITH (NOLOCK) ON em.JobGradeId = jg.JobGradeId
	INNER JOIN dbo.GradeLevel AS gl WITH (NOLOCK) ON em.GradeLevelId = gl.GradeLevelId
	INNER JOIN dbo.RefResource AS country WITH (NOLOCK) ON em.CountryId = country.ResourceId AND country.RefName = 'Country'
	INNER JOIN dbo.RefResource AS city WITH (NOLOCK) ON em.CityId = city.ResourceId AND city.RefName = 'City'
	INNER JOIN dbo.RefResource AS office WITH (NOLOCK) ON em.OfficeId = office.ResourceId AND office.RefName = 'Office'
	INNER JOIN dbo.Division AS d WITH (NOLOCK) on em.DivisionId = d.DivisionId
	INNER JOIN dbo.CostCenter AS cc WITH (NOLOCK) on em.CostCenterId = cc.CostCenterId
	INNER JOIN dbo.Department AS dm WITH (NOLOCK) ON em.DepartmentId = dm.DepartmentId
	INNER JOIN dbo.Section AS s WITH (NOLOCK) ON em.SectionId = s.SectionId
	INNER JOIN dbo.RefResource AS localCompany WITH (NOLOCK) ON em.LocalCompanyId = localCompany.ResourceId AND localCompany.RefName = 'LocalCompany'
	INNER JOIN dbo.RefResource AS localJobTitleRef WITH (NOLOCK) ON em.LocalJobTitleId = localJobTitleRef.ResourceId AND localJobTitleRef.RefName = 'LocalJobTitle'
	INNER JOIN dbo.RefResource AS employmentTypeRef WITH (NOLOCK) ON em.EmploymentType = employmentTypeRef.ResourceId AND employmentTypeRef.RefName = 'EmploymentType'
	INNER JOIN dbo.RefResource AS employmentStatusRef WITH (NOLOCK) ON em.EmploymentStatus = employmentStatusRef.ResourceId AND employmentStatusRef.RefName = 'EmploymentStatus'
	LEFT JOIN dbo.RefResource AS dstCompanyRef WITH (NOLOCK) ON em.DST = dstCompanyRef.ResourceId AND dstCompanyRef.RefName = 'LocalCompany'
	LEFT JOIN dbo.RefResource AS dbtCompanyRef WITH (NOLOCK) ON em.DBT = dbtCompanyRef.ResourceId AND dbtCompanyRef.RefName = 'LocalCompany'

	LEFT JOIN dbo.EmploymentHistory AS emh WITH (NOLOCK) ON em.EmployeeId = emh.EmployeeId
	LEFT JOIN dbo.RefResource AS bviChangeRef WITH (NOLOCK) ON emh.CompanyReasonId = bviChangeRef.ResourceId AND bviChangeRef.RefName = 'CompanyBVIChangeReason'
	LEFT JOIN dbo.RefResource AS localChangeRef WITH (NOLOCK) ON emh.LocalCompanyReasonId = localChangeRef.ResourceId AND localChangeRef.RefName = 'CompanyLocalChangeReason'
	LEFT JOIN dbo.RefResource AS jobTitleReasonRef WITH (NOLOCK) ON emh.JobTitleReasonId = jobTitleReasonRef.ResourceId AND jobTitleReasonRef.RefName = 'JobTitleBVIChangeReason'
	LEFT JOIN dbo.RefResource AS localJobTitleReasonRef WITH (NOLOCK) ON emh.LocalJobTitleReasonId = localJobTitleReasonRef.ResourceId AND localJobTitleReasonRef.RefName = 'JobTitleLocalChangeReason'
	LEFT JOIN dbo.RefResource AS groupDateReasonRef WITH (NOLOCK) ON emh.GroupDateReasonId = groupDateReasonRef.ResourceId AND groupDateReasonRef.RefName = 'DateCommencementGroupChangeReason'
	LEFT JOIN dbo.RefResource AS companyDateReasonRef WITH (NOLOCK) ON emh.CompanyDateReasonId = companyDateReasonRef.ResourceId AND companyDateReasonRef.RefName = 'DateCommencementCompanyChangeReason'
	LEFT JOIN dbo.RefResource AS idChangeReasonRef WITH (NOLOCK) ON emh.IDNoReasonId = idChangeReasonRef.ResourceId AND idChangeReasonRef.RefName = 'EmployeeIDChangeReason'
	LEFT JOIN dbo.RefResource AS categoryChangeReasonRef WITH (NOLOCK) ON emh.JobCategoryReasonId = categoryChangeReasonRef.ResourceId AND categoryChangeReasonRef.RefName = 'JobCategoryChangeReason'
	LEFT JOIN dbo.RefResource AS jobGradeChangeReasonRef WITH (NOLOCK) ON emh.JobGradeReasonId = jobGradeChangeReasonRef.ResourceId AND jobGradeChangeReasonRef.RefName = 'JobGradeChangeReason'
	LEFT JOIN dbo.RefResource AS divisionChangeReasonRef WITH (NOLOCK) ON emh.DivisionReasonId = divisionChangeReasonRef.ResourceId AND divisionChangeReasonRef.RefName = 'DivisionChangeReason'
	LEFT JOIN dbo.RefResource AS departmentChangeReasonRef WITH (NOLOCK) ON emh.DepartmentReasonId = departmentChangeReasonRef.ResourceId AND departmentChangeReasonRef.RefName = 'DepartmentChangeReason'
	LEFT JOIN dbo.RefResource AS costcenterChangeReasonRef WITH (NOLOCK) ON emh.CostCenterReasonId = costcenterChangeReasonRef.ResourceId AND costcenterChangeReasonRef.RefName = 'CostCenterChangeReason'
	LEFT JOIN dbo.RefResource AS sectionChangeReasonRef WITH (NOLOCK) ON emh.SectionReasonId = sectionChangeReasonRef.ResourceId AND sectionChangeReasonRef.RefName = 'SectionChangeReason'

	LEFT JOIN dbo.EmploymentWorkPermit AS ewp WITH (NOLOCK) ON e.EmployeeId = ewp.EmployeeId
	LEFT JOIN dbo.RefResource AS wpCompanyRef WITH (NOLOCK) ON ewp.LocalCompanyId = wpCompanyRef.ResourceId AND wpCompanyRef.RefName = 'LocalCompany'
	LEFT JOIN dbo.RefResource AS wpType1Ref WITH (NOLOCK) ON ewp.Type1 = wpType1Ref.ResourceId AND wpType1Ref.RefName = 'WorkPermitType1'
	LEFT JOIN dbo.RefResource AS wpType2Ref WITH (NOLOCK) ON ewp.Type2 = wpType2Ref.ResourceId AND wpType2Ref.RefName = 'WorkPermitType2'

	LEFT JOIN dbo.EmploymentPaymaster AS emp WITH (NOLOCK) ON e.EmployeeId = emp.EmployeeId
	LEFT JOIN dbo.EmploymentEPA AS eepa WITH (NOLOCK) ON e.EmployeeId = eepa.EmployeeId

	LEFT JOIN dbo.EmployeePayrollBasicSalary AS epbs WITH (NOLOCK) ON e.EmployeeId = epbs.EmployeeId
	LEFT JOIN dbo.Currency AS bsc WITH (NOLOCK) ON epbs.CurrencyId = bsc.CurrencyId
	LEFT JOIN dbo.Currency AS bsec WITH (NOLOCK) ON epbs.EbankingCurrencyId = bsec.CurrencyId
	LEFT JOIN dbo.Currency AS bsd1c WITH (NOLOCK) ON epbs.DirectBanking1CurrencyId = bsd1c.CurrencyId
	LEFT JOIN dbo.Currency AS bsd2c WITH (NOLOCK) ON epbs.DirectBanking2CurrencyId = bsd2c.CurrencyId
	LEFT JOIN dbo.Currency AS bscc WITH (NOLOCK) ON epbs.CashCurrencyId = bscc.CurrencyId
	LEFT JOIN dbo.BankAccount AS eba WITH (NOLOCK) ON epbs.EbankingAccountId = eba.Id
	LEFT JOIN dbo.RefResource ebaRef WITH (NOLOCK) ON eba.PaymentTransactionId = ebaRef.ResourceId AND ebaRef.RefName = 'PaymentTransactionEbanking'
	LEFT JOIN dbo.BankAccount AS db1a WITH (NOLOCK) ON epbs.DirectBanking1AccountId = db1a.Id
	LEFT JOIN dbo.RefResource db1aRef WITH (NOLOCK) ON db1a.PaymentTransactionId = db1aRef.ResourceId AND db1aRef.RefName = 'PaymentTransactionDirectBanking'
	LEFT JOIN dbo.BankAccount AS db2a WITH (NOLOCK) ON DirectBanking2AccountId = db2a.Id
	LEFT JOIN dbo.RefResource db2aRef WITH (NOLOCK) ON db2a.PaymentTransactionId = db2aRef.ResourceId AND db2aRef.RefName = 'PaymentTransactionDirectBanking'
	LEFT JOIN dbo.RefResource caRef WITH (NOLOCK) ON epbs.CashAccountId = caRef.ResourceId AND caRef.RefName = 'PaymentTransactionCash'	
	OUTER APPLY (
		SELECT TOP 1 EffectiveDate
		,rr.Value AS 'Reason'
		FROM dbo.EmployeePayrollBasicSalaryHistory epbsh WITH (NOLOCK)
		LEFT JOIN dbo.RefResource AS rr WITH (NOLOCK) ON epbsh.ReasonId = rr.ResourceId AND rr.RefName = 'BasicSalaryChangeReason'
		WHERE EmployeeId = epbs.EmployeeId
		ORDER BY HistoryId DESC
	) AS epbsh

	LEFT JOIN dbo.EmployeePayrollAllowance AS epa WITH (NOLOCK) ON e.EmployeeId = epa.EmployeeId
	LEFT JOIN dbo.Currency AS epac WITH (NOLOCK) ON epa.CurrencyId = epac.CurrencyId
	LEFT JOIN dbo.Currency AS epad1c WITH (NOLOCK) ON epa.DirectBanking1CurrencyId = epad1c.CurrencyId
	LEFT JOIN dbo.Currency AS epad2c WITH (NOLOCK) ON epa.DirectBanking2CurrencyId = epad2c.CurrencyId
	LEFT JOIN dbo.Currency AS epacc WITH (NOLOCK) ON epa.CashCurrencyId = epacc.CurrencyId
	LEFT JOIN dbo.Currency AS sstc WITH (NOLOCK) ON epa.SubsistenceCurrencyId = sstc.CurrencyId
	LEFT JOIN dbo.Currency AS hc WITH (NOLOCK) ON epa.HousingCurrencyId = hc.CurrencyId
	LEFT JOIN dbo.Currency AS hpc WITH (NOLOCK) ON epa.HomePassageCurrencyId = hpc.CurrencyId
	LEFT JOIN dbo.Currency AS dmc WITH (NOLOCK) ON epa.DutyMealCurrencyId = dmc.CurrencyId
	LEFT JOIN dbo.Currency AS hpic WITH (NOLOCK) ON epa.HPICurrencyId = hpic.CurrencyId
	LEFT JOIN dbo.Currency AS mpc WITH (NOLOCK) ON epa.MobilePhoneCurrencyId = mpc.CurrencyId
	LEFT JOIN dbo.Currency AS otc WITH (NOLOCK) ON epa.OthersCurrencyId = otc.CurrencyId
	LEFT JOIN dbo.Currency AS smc WITH (NOLOCK) ON epa.SecondmentCurrencyId = smc.CurrencyId
	LEFT JOIN dbo.BankAccount AS epadb1a WITH (NOLOCK) ON epa.DirectBanking1AccountId = epadb1a.Id
	LEFT JOIN dbo.RefResource epadb1aRef WITH (NOLOCK) ON epadb1a.PaymentTransactionId = epadb1aRef.ResourceId AND epadb1aRef.RefName = 'PaymentTransactionDirectBanking'
	LEFT JOIN dbo.BankAccount AS epadb2a WITH (NOLOCK) ON epa.DirectBanking2AccountId = epadb2a.Id
	LEFT JOIN dbo.RefResource epadb2aRef WITH (NOLOCK) ON epadb2a.PaymentTransactionId = epadb2aRef.ResourceId AND epadb2aRef.RefName = 'PaymentTransactionDirectBanking'
	LEFT JOIN dbo.RefResource epacaRef WITH (NOLOCK) ON epa.CashAccountId = epacaRef.ResourceId AND epacaRef.RefName = 'PaymentTransactionCash'

	LEFT JOIN dbo.EmployeePayrollDeduction epd WITH (NOLOCK) on e.EmployeeId = epd.EmployeeId
	LEFT JOIN dbo.Currency AS epdrmc WITH (NOLOCK) ON epd.RoomingChargesCurrencyId = epdrmc.CurrencyId
	LEFT JOIN dbo.Currency AS epdffc WITH (NOLOCK) ON epd.FitnessFirstCurrencyId = epdffc.CurrencyId
	WHERE em.CompanyId = @CompanyId 
	AND EXISTS (
			SELECT 1
			FROM dbo.strSplit(@EmployeeStatusIds, ',') AS s
			WHERE em.EmploymentStatus = s.val
		)
END
