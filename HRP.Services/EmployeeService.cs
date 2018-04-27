namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.EmergencyContact;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.Employee;
    using HRP.Repositories;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeService : IEmployeeService
    {
        private readonly IBaseService baseService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ICryptoService cryptoService,
            ISessionService sessionService,
            ICommonService commonService,
            IEmployeeRepository employeeRepository = null)
        {
            this.baseService = baseService;
            this.hostingEnvironment = hostingEnvironment;
            this.cryptoService = cryptoService;
            this.sessionService = sessionService;
            this.commonService = commonService;
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public async Task<EmployeeBasicInfo> GetEmployeeBasicInfoByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeBasicInfoByIdCriteria
            {
                EmployeeId = employeeId
            };

            return await baseService.GetDataEntityAsync<EmployeeBasicInfo>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmployeeBasicInfo>> GetEmployeesAsync(EmployeeManagementViewModel viewModel)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                DepartmentIds = viewModel.DepartmentIds,
                EmploymentStatusIds = viewModel.EmploymentStatusIds,
                EmployeeId = viewModel.EmployeeId,
                EmployeeName = viewModel.DisplayName,
                UserId = sessionService.UserId
            };

            return await baseService.GetDataAsync<EmployeeBasicInfo>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employee)
        {
            var pictureExtension = employee.ProfilePicture != null ? Path.GetExtension(employee.ProfilePicture.FileName) : null;
            var criteria = new CreateEmployeeCriteria
            {
                FullName = employee.FullName,
                Nickname = employee.Nickname,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                PlaceOfBirth = employee.PlaceOfBirth,
                Nationality = employee.Nationality,
                NRICNo = employee.NRICNo,
                MaritalStatus = employee.MaritalStatus,
                NoOfChildren = employee.NoOfChildren,
                OverseasPhoneNo = employee.OverseasPhoneNo,
                LocalPhoneNo = employee.LocalPhoneNo,
                OtherPhoneNo = employee.OtherPhoneNo,
                PassportNo = employee.PassportNo,
                DateIssue = employee.DateIssue,
                ExpiredDate = employee.ExpiredDate,
                PlaceOfIssue = employee.PlaceOfIssue,
                Skype = employee.Skype,
                PersonalEmail = employee.PersonalEmail,
                PermanentAddress = employee.PermanentAddress,
                ResidenceAddress = employee.ResidenceAddress,
                MailingAddress = employee.MailingAddress,
                PictureExtension = pictureExtension,
                ECName = employee.EmergencyContact.Name,
                ECAddress = employee.EmergencyContact.Address,
                ECRelationship = employee.EmergencyContact.Relationship,
                ECPhoneNo1 = employee.EmergencyContact.PhoneNo1,
                ECPhoneNo2 = employee.EmergencyContact.PhoneNo2,
                CompanyId = employee.CompanyId,
                LocalCompanyId = employee.LocalCompanyId,
                JobTitle = employee.JobTitle,
                LocalJobTitleId = employee.LocalJobTitleId,
                EmployeeCode = employee.EmployeeCode,
                EmploymentType = employee.EmploymentType,
                EmploymentStatus = employee.EmploymentStatus,
                JobCategoryId = employee.JobCategoryId,
                JobGradeId = employee.JobGradeId,
                GradeLevelId = employee.GradeLevelId,
                DivisionId = employee.DivisionId,
                DepartmentId = employee.DepartmentId,
                SectionId = employee.SectionId,
                CostCenterId = employee.CostCenterId,
                GroupStartDate = employee.GroupStartDate,
                GroupEndDate = employee.GroupEndDate,
                CompanyStartDate = employee.CompanyStartDate,
                CompanyEndDate = employee.CompanyEndDate,
                Email = employee.Email,
                CountryId = employee.CountryId,
                CityId = employee.CountryId,
                OfficeId = employee.OfficeId,
                DST = employee.DST,
                DBT = employee.DBT,
                TaxIDNo = employee.TaxIDNo,
                SSSNo = employee.SSSNo,
                HealthInsNo = employee.HealthInsNo,
                HDMFNo = employee.HDMFNo,
                WorkPermitCompanyId = employee.WorkPermit.LocalCompanyId,
                Type1 = employee.WorkPermit.Type1,
                StartDateType1 = employee.WorkPermit.StartDateType1,
                EndDateType1 = employee.WorkPermit.EndDateType1,
                Type2 = employee.WorkPermit.Type2,
                StartDateType2 = employee.WorkPermit.StartDateType2,
                EndDateType2 = employee.WorkPermit.EndDateType2,
                FullNameBVISalary = employee.Paymaster.FullNameBVISalary,
                NicknameBVISalary = employee.Paymaster.NicknameBVISalary,
                FullNameBVIAllowance = employee.Paymaster.FullNameBVIAllowance,
                NicknameBVIAllowance = employee.Paymaster.NicknameBVIAllowance,
                FullNameLocalSalary = employee.Paymaster.FullNameLocalSalary,
                NicknameLocalSalary = employee.Paymaster.NicknameLocalSalary,
                FullNameAdministrator = employee.EPA.FullNameAdministrator,
                NicknameAdministrator = employee.EPA.NicknameAdministrator,
                FullNameEvaluator1 = employee.EPA.FullNameEvaluator1,
                NicknameEvaluator1 = employee.EPA.NicknameEvaluator1,
                FullNameEvaluator2 = employee.EPA.FullNameEvaluator2,
                NicknameEvaluator2 = employee.EPA.NicknameEvaluator2,
                FullNameReviewer = employee.EPA.FullNameReviewer,
                NicknameReviewer = employee.EPA.NicknameReviewer,
                FullNameManagement = employee.EPA.FullNameManagement,
                NicknameManagement = employee.EPA.NicknameManagement,
                FullNameIncrement = employee.EPA.FullNameIncrement,
                NicknameIncrement = employee.EPA.NicknameIncrement,
                FullNameBVIBonus = employee.EPA.FullNameBVIBonus,
                NicknameBVIBonus = employee.EPA.NicknameBVIBonus,
                FullNameLocalBonus = employee.EPA.FullNameLocalBonus,
                NicknameLocalBonus = employee.EPA.NicknameLocalBonus,
                FullNamePFileAdmin = employee.FullNamePFileAdmin,
                NicknamePFileAdmin = employee.NicknamePFileAdmin,
                CreatedBy = sessionService.UserId
            };
            criteria.PDFPassword = cryptoService.Encrypt(employee.PDFPassword);

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful && employee.ProfilePicture != null)
            {
                await SaveProfilePictureAsync(employee.ProfilePicture, criteria.EmployeeId.ToString()).ConfigureAwait(false);
                commandResult.Data = criteria.EmployeeId;
            }

            return commandResult;
        }

        public async Task<CommandResult> UpdatePersonalDetailsAsync(CreateEmployeeViewModel employee)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Nickname = employee.Nickname,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                PlaceOfBirth = employee.PlaceOfBirth,
                Nationality = employee.Nationality,
                NRICNo = employee.NRICNo,
                MaritalStatus = employee.MaritalStatus,
                NoOfChildren = employee.NoOfChildren,
                OverseasPhoneNo = employee.OverseasPhoneNo,
                LocalPhoneNo = employee.LocalPhoneNo,
                OtherPhoneNo = employee.OtherPhoneNo,
                PassportNo = employee.PassportNo,
                DateIssue = employee.DateIssue,
                ExpiredDate = employee.ExpiredDate,
                PlaceOfIssue = employee.PlaceOfIssue,
                Skype = employee.Skype,
                PersonalEmail = employee.PersonalEmail,
                PermanentAddress = employee.PermanentAddress,
                ResidenceAddress = employee.ResidenceAddress,
                MailingAddress = employee.MailingAddress,
                ECName = employee.EmergencyContact.Name,
                ECAddress = employee.EmergencyContact.Address,
                ECRelationship = employee.EmergencyContact.Relationship,
                ECPhoneNo1 = employee.EmergencyContact.PhoneNo1,
                ECPhoneNo2 = employee.EmergencyContact.PhoneNo2,
                ProfilePic = (employee.ProfilePicture != null) ? employee.EmployeeId + Path.GetExtension(employee.ProfilePicture.FileName) : employee.ProfilePic,
                UpdatedBy = sessionService.UserId,
                FunctionKey = FunctionKey.EmployeeProfile
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful && employee.ProfilePicture != null)
            {
                await SaveProfilePictureAsync(employee.ProfilePicture, employee.EmployeeId.ToString()).ConfigureAwait(false);
            }

            return commandResult;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId,
                UserId = sessionService.UserId,
                FunctionKey = FunctionKey.EmployeeProfile
            };

            var employee = await baseService.GetDataEntityAsync<Employee>(criteria).ConfigureAwait(false);

            return employee;
        }

        public async Task<EmergencyContact> GetEmergencyContactByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetEmergencyContactByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            var contact = await baseService.GetDataEntityAsync<EmergencyContact>(criteria).ConfigureAwait(false);

            return contact;
        }

        public async Task<Tuple<SelectList, SelectList, SelectList, SelectList>> GetDataToCreateEmployeeAsync()
        {
            string[] refNames =
                {
                     ReferenceResource.Gender,
                     ReferenceResource.Nationality,
                     ReferenceResource.MaritalStatus,
                     ReferenceResource.NoOfChildren
                };
            IEnumerable<ReferenceResourceModel> refResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);
            SelectList genderSelectList = commonService.GetSelectList(commonService.FilterReferenceResources(refResources, ReferenceResource.Gender));
            SelectList nationalSelectList = commonService.GetSelectList(commonService.FilterReferenceResources(refResources, ReferenceResource.Nationality));
            SelectList maritalStatusSelectList = commonService.GetSelectList(commonService.FilterReferenceResources(refResources, ReferenceResource.MaritalStatus));
            SelectList noOfChildrenSelectList = commonService.GetSelectList(commonService.FilterReferenceResources(refResources, ReferenceResource.NoOfChildren));

            return new Tuple<SelectList, SelectList, SelectList, SelectList>(genderSelectList, nationalSelectList, maritalStatusSelectList, noOfChildrenSelectList);
        }

        public CommandResult CheckValidPicture(IFormFile file)
        {
            var result = new CommandResult();

            if (file != null)
            {
                if (file.Length / (1024 * 1024) > 2)
                {
                    result.Code = 1;
                    result.Message = Message.FileTooLarge;
                }
                else if (!file.ContentType.Contains("image"))
                {
                    result.Code = 1;
                    result.Message = Message.InvalidFileType;
                }
                else
                {
                    result.Code = 0;
                    result.Message = string.Empty;
                }
            }
            else
            {
                result.Code = 0;
                result.Message = string.Empty;
            }

            return result;
        }

        public async Task<CreateEmploymentViewModel> GetDataToCreateEmploymentAsync()
        {
            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.EmployeeProfile).ConfigureAwait(false);

            ICriteria criteria = new GetAllJobCategoryCriteria { IsActive = true };
            var jobCategories = await baseService.GetDataAsync<JobCategory>(criteria).ConfigureAwait(false);

            criteria = new GetAllCountryCriteria { IsActive = true };
            var coutries = await baseService.GetDataAsync<Country>(criteria).ConfigureAwait(false);

            string[] refNames =
                {
                    ReferenceResource.LocalCompany,
                    ReferenceResource.LocalJobTitle,
                    ReferenceResource.EmploymentType,
                    ReferenceResource.EmploymentStatus,
                    ReferenceResource.WorkPermitType1,
                    ReferenceResource.WorkPermitType2
                };
            IEnumerable<ReferenceResourceModel> refResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);

            var viewModel = new CreateEmploymentViewModel
            {
                Companies = companies,
                LocalCompanies = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalCompany),
                LocalJobTitles = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalJobTitle),
                EmploymentTypes = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentType),
                EmploymentStatuses = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentStatus),
                JobCategories = jobCategories,
                Countries = coutries,
                WorkPermitType1s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType1),
                WorkPermitType2s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType2)
            };

            return viewModel;
        }

        public async Task<Employment> GetEmploymentByEmployeeIdAsync(int employeeId)
        {
            var criteria = new GetEmploymentByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };
            Employment employment = await employeeRepository.GetEmploymentByEmployeeIdAsync(criteria).ConfigureAwait(false);

            if (employment != null)
            {
                employment.PDFPassword = cryptoService.Decrypt(employment.PDFPassword);
            }

            return employment;
        }

        public async Task<UpdateEmploymentViewModel> GetDataToUpdateEmploymentAsync(int employeeId)
        {
            Employment employment = await GetEmploymentByEmployeeIdAsync(employeeId).ConfigureAwait(false);
            if (employment == null)
            {
                return null;
            }

            IEnumerable<Company> companies = await commonService.GetCompanyByFunctionKeyAsync(FunctionKey.EmployeeProfile).ConfigureAwait(false);

            ICriteria criteria = new GetDivisionByCompanyIdCriteria { CompanyId = employment.CompanyId, IsActive = true };
            var divisions = await baseService.GetDataAsync<Division>(criteria).ConfigureAwait(false);

            criteria = new GetDepartmentByDivisionIdCriteria { DivisionId = employment.DivisionId, IsActive = true };
            var departments = await baseService.GetDataAsync<Department>(criteria).ConfigureAwait(false);

            criteria = new GetCostCenterDepartmentIdCriteria { DepartmentId = employment.DepartmentId, IsActive = true };
            var costCenters = await baseService.GetDataAsync<CostCenter>(criteria).ConfigureAwait(false);

            criteria = new GetSectionByCostCenterIdCriteria { CostCenterId = employment.CostCenterId, IsActive = true };
            var sections = await baseService.GetDataAsync<Section>(criteria).ConfigureAwait(false);

            criteria = new GetAllJobCategoryCriteria { IsActive = true };
            var jobCategories = await baseService.GetDataAsync<JobCategory>(criteria).ConfigureAwait(false);

            criteria = new GetGradeLevelByJobCategoryIdCriteria { JobCategoryId = employment.JobCategoryId, IsActive = true };
            var gradeLevels = await baseService.GetDataAsync<GradeLevel>(criteria).ConfigureAwait(true);

            criteria = new GetJobGradeByGradeLevelIdCriteria { GradeLevelId = employment.GradeLevelId, IsActive = true };
            var jobGrades = await baseService.GetDataAsync<JobGrade>(criteria).ConfigureAwait(false);

            criteria = new GetAllCountryCriteria { IsActive = true };
            var coutries = await baseService.GetDataAsync<Country>(criteria).ConfigureAwait(false);

            criteria = new GetCityByCountryIdCriteria { CountryId = employment.CountryId, IsActive = true };
            var cities = await baseService.GetDataAsync<City>(criteria).ConfigureAwait(false);

            criteria = new GetOfficeByCityIdCriteria { CityId = employment.CityId, IsActive = true };
            var offices = await baseService.GetDataAsync<Office>(criteria).ConfigureAwait(false);

            string[] refNames =
                {
                    ReferenceResource.LocalCompany,
                    ReferenceResource.LocalJobTitle,
                    ReferenceResource.EmploymentType,
                    ReferenceResource.EmploymentStatus,
                    ReferenceResource.WorkPermitType1,
                    ReferenceResource.WorkPermitType2,
                    ReferenceResource.CompanyBVIChangeReason,
                    ReferenceResource.CompanyLocalChangeReason,
                    ReferenceResource.JobTitleChangeReason,
                    ReferenceResource.DateCommencementChangeReason,
                    ReferenceResource.EmployeeIDChangeReason,
                    ReferenceResource.JobCategoryChangeReason,
                    ReferenceResource.JobGradeChangeReason,
                    ReferenceResource.DivisionChangeReason,
                    ReferenceResource.DepartmentChangeReason,
                    ReferenceResource.CostCenterChangeReason,
                    ReferenceResource.SectionChangeReason
                };
            IEnumerable<ReferenceResourceModel> refResources = await commonService.GetReferenceResourcesByNamesAsync(refNames).ConfigureAwait(false);

            var viewModel = new UpdateEmploymentViewModel
            {
                EmployeeId = employeeId,
                CompanyId = employment.CompanyId,
                OldCompanyId = employment.CompanyId,
                CompanyChangeDate = employment.CompanyChangeDate,
                CompanyReasonId = employment.CompanyReasonId,
                LocalCompanyId = employment.LocalCompanyId,
                OldLocalCompanyId = employment.LocalCompanyId,
                LocalCompanyChangeDate = employment.LocalCompanyChangeDate,
                LocalCompanyReasonId = employment.LocalCompanyReasonId,
                JobTitle = employment.JobTitle,
                OldJobTitle = employment.JobTitle,
                JobTitleChangeDate = employment.JobTitleChangeDate,
                JobTitleReasonId = employment.JobTitleReasonId,
                LocalJobTitleId = employment.LocalJobTitleId,
                OldLocalJobTitleId = employment.LocalJobTitleId,
                LocalJobTitleChangeDate = employment.LocalJobTitleChangeDate,
                LocalJobTitleReasonId = employment.LocalJobTitleReasonId,
                CompanyCode = employment.CompanyCode,
                EmployeeCode = employment.EmployeeCode,
                OldEmployeeCode = employment.EmployeeCode,
                IDNoChangeDate = employment.IDNoChangeDate,
                IDNoReasonId = employment.IDNoReasonId,
                EmploymentType = employment.EmploymentType,
                EmploymentStatus = employment.EmploymentStatus,
                OldEmploymentStatus = employment.EmploymentStatus,
                EmploymentStatusChangeDate = employment.EmploymentStatusChangeDate,
                JobCategoryId = employment.JobCategoryId,
                OldJobCategoryId = employment.JobCategoryId,
                JobCategoryChangeDate = employment.JobCategoryChangeDate,
                JobCategoryReasonId = employment.JobCategoryReasonId,
                JobGradeId = employment.JobGradeId,
                OldJobGradeId = employment.JobGradeId,
                JobGradeChangeDate = employment.JobGradeChangeDate,
                JobGradeReasonId = employment.JobGradeReasonId,
                GradeLevelId = employment.GradeLevelId,
                OldGradeLevelId = employment.GradeLevelId,
                DivisionId = employment.DivisionId,
                OldDivisionId = employment.DivisionId,
                DivisionChangeDate = employment.DivisionChangeDate,
                DivisionReasonId = employment.DivisionReasonId,
                DepartmentId = employment.DepartmentId,
                OldDepartmentId = employment.DepartmentId,
                DepartmentChangeDate = employment.DepartmentChangeDate,
                DepartmentReasonId = employment.DepartmentReasonId,
                CostCenterId = employment.CostCenterId,
                OldCostCenterId = employment.CostCenterId,
                CostCenterChangeDate = employment.CostCenterChangeDate,
                CostCenterReasonId = employment.CostCenterReasonId,
                SectionId = employment.SectionId,
                OldSectionId = employment.SectionId,
                SectionChangeDate = employment.SectionChangeDate,
                SectionReasonId = employment.SectionReasonId,
                GroupStartDate = employment.GroupStartDate,
                OldGroupStartDate = employment.GroupStartDate,
                GroupEndDate = employment.GroupEndDate,
                OldGroupEndDate = employment.GroupEndDate,
                GroupDateReasonId = employment.GroupDateReasonId,
                CompanyStartDate = employment.CompanyStartDate,
                OldCompanyStartDate = employment.CompanyStartDate,
                CompanyEndDate = employment.CompanyEndDate,
                OldCompanyEndDate = employment.CompanyEndDate,
                CompanyDateReasonId = employment.CompanyDateReasonId,
                Email = employment.Email,
                CountryId = employment.CountryId,
                CityId = employment.CityId,
                OfficeId = employment.OfficeId,
                DST = employment.DST,
                DBT = employment.DBT,
                TaxIDNo = employment.TaxIDNo,
                SSSNo = employment.SSSNo,
                HealthInsNo = employment.HealthInsNo,
                HDMFNo = employment.HDMFNo,
                PDFPassword = employment.PDFPassword,
                OldPDFPassword = employment.PDFPassword,
                FullNamePFileAdmin = employment.FullNamePFileAdmin,
                NicknamePFileAdmin = employment.NicknamePFileAdmin,
                WorkPermit = employment.WorkPermit,
                Paymaster = employment.Paymaster,
                EPA = employment.EPA,
                Companies = companies,
                Divisions = divisions,
                Departments = departments,
                CostCenters = costCenters,
                Sections = sections,
                JobCategories = jobCategories,
                GradeLevels = gradeLevels,
                JobGrades = jobGrades,
                Countries = coutries,
                Cities = cities,
                Offices = offices,
                LocalCompanies = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalCompany),
                LocalJobTitles = commonService.FilterReferenceResources(refResources, ReferenceResource.LocalJobTitle),
                EmploymentTypes = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentType),
                EmploymentStatuses = commonService.FilterReferenceResources(refResources, ReferenceResource.EmploymentStatus),
                WorkPermitType1s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType1),
                WorkPermitType2s = commonService.FilterReferenceResources(refResources, ReferenceResource.WorkPermitType2),
                CompanyChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.CompanyBVIChangeReason),
                LocalCompanyChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.CompanyLocalChangeReason),
                JobTitleChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.JobTitleChangeReason),
                DateCommencementChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.DateCommencementChangeReason),
                IDNoChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.EmployeeIDChangeReason),
                JobCategoryChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.JobCategoryChangeReason),
                JobGradeChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.JobGradeChangeReason),
                DivisionChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.DivisionChangeReason),
                DepartmentChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.DepartmentChangeReason),
                CostCenterChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.CostCenterChangeReason),
                SectionChangeReasons = commonService.FilterReferenceResources(refResources, ReferenceResource.SectionChangeReason)
            };

            return viewModel;
        }

        public async Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel)
        {
            var criteria = new UpdateEmploymentCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                CompanyChangeDate = viewModel.CompanyChangeDate,
                CompanyReasonId = viewModel.CompanyReasonId,
                LocalCompanyId = viewModel.LocalCompanyId,
                LocalCompanyChangeDate = viewModel.LocalCompanyChangeDate,
                LocalCompanyReasonId = viewModel.LocalCompanyReasonId,
                JobTitle = viewModel.JobTitle,
                JobTitleChangeDate = viewModel.JobTitleChangeDate,
                JobTitleReasonId = viewModel.JobTitleReasonId,
                LocalJobTitleId = viewModel.LocalJobTitleId,
                LocalJobTitleChangeDate = viewModel.LocalJobTitleChangeDate,
                LocalJobTitleReasonId = viewModel.LocalJobTitleReasonId,
                EmployeeCode = viewModel.EmployeeCode,
                IDNoChangeDate = viewModel.IDNoChangeDate,
                IDNoReasonId = viewModel.IDNoReasonId,
                EmploymentType = viewModel.EmploymentType,
                EmploymentStatus = viewModel.EmploymentStatus,
                EmploymentStatusChangeDate = viewModel.EmploymentStatusChangeDate,
                JobCategoryId = viewModel.JobCategoryId,
                JobCategoryChangeDate = viewModel.JobCategoryChangeDate,
                JobCategoryReasonId = viewModel.JobCategoryReasonId,
                JobGradeId = viewModel.JobGradeId,
                GradeLevelId = viewModel.GradeLevelId,
                JobGradeChangeDate = viewModel.JobGradeChangeDate,
                JobGradeReasonId = viewModel.JobGradeReasonId,
                DivisionId = viewModel.DivisionId,
                DivisionChangeDate = viewModel.DivisionChangeDate,
                DivisionReasonId = viewModel.DivisionReasonId,
                DepartmentId = viewModel.DepartmentId,
                DepartmentChangeDate = viewModel.DepartmentChangeDate,
                DepartmentReasonId = viewModel.DepartmentReasonId,
                SectionId = viewModel.SectionId,
                SectionChangeDate = viewModel.SectionChangeDate,
                SectionReasonId = viewModel.SectionReasonId,
                CostCenterId = viewModel.CostCenterId,
                CostCenterChangeDate = viewModel.CostCenterChangeDate,
                CostCenterReasonId = viewModel.CostCenterReasonId,
                GroupStartDate = viewModel.GroupStartDate,
                GroupEndDate = viewModel.GroupEndDate,
                GroupDateReasonId = viewModel.GroupDateReasonId,
                CompanyStartDate = viewModel.CompanyStartDate,
                CompanyEndDate = viewModel.CompanyEndDate,
                CompanyDateReasonId = viewModel.CompanyDateReasonId,
                Email = viewModel.Email,
                CountryId = viewModel.CountryId,
                CityId = viewModel.CityId,
                OfficeId = viewModel.OfficeId,
                DST = viewModel.DST,
                DBT = viewModel.DBT,
                TaxIDNo = viewModel.TaxIDNo,
                SSSNo = viewModel.SSSNo,
                HealthInsNo = viewModel.HealthInsNo,
                HDMFNo = viewModel.HDMFNo,
                WorkPermitCompanyId = viewModel.WorkPermit.LocalCompanyId,
                Type1 = viewModel.WorkPermit.Type1,
                StartDateType1 = viewModel.WorkPermit.StartDateType1,
                EndDateType1 = viewModel.WorkPermit.EndDateType1,
                Type2 = viewModel.WorkPermit.Type2,
                StartDateType2 = viewModel.WorkPermit.StartDateType2,
                EndDateType2 = viewModel.WorkPermit.EndDateType2,
                FullNameBVISalary = viewModel.Paymaster.FullNameBVISalary,
                NicknameBVISalary = viewModel.Paymaster.NicknameBVISalary,
                FullNameBVIAllowance = viewModel.Paymaster.FullNameBVIAllowance,
                NicknameBVIAllowance = viewModel.Paymaster.NicknameBVIAllowance,
                FullNameLocalSalary = viewModel.Paymaster.FullNameLocalSalary,
                NicknameLocalSalary = viewModel.Paymaster.NicknameLocalSalary,
                FullNameAdministrator = viewModel.EPA.FullNameAdministrator,
                NicknameAdministrator = viewModel.EPA.NicknameAdministrator,
                FullNameEvaluator1 = viewModel.EPA.FullNameEvaluator1,
                NicknameEvaluator1 = viewModel.EPA.NicknameEvaluator1,
                FullNameEvaluator2 = viewModel.EPA.FullNameEvaluator2,
                NicknameEvaluator2 = viewModel.EPA.NicknameEvaluator2,
                FullNameReviewer = viewModel.EPA.FullNameReviewer,
                NicknameReviewer = viewModel.EPA.NicknameReviewer,
                FullNameManagement = viewModel.EPA.FullNameManagement,
                NicknameManagement = viewModel.EPA.NicknameManagement,
                FullNameIncrement = viewModel.EPA.FullNameIncrement,
                NicknameIncrement = viewModel.EPA.NicknameIncrement,
                FullNameBVIBonus = viewModel.EPA.FullNameBVIBonus,
                NicknameBVIBonus = viewModel.EPA.NicknameBVIBonus,
                FullNameLocalBonus = viewModel.EPA.FullNameLocalBonus,
                NicknameLocalBonus = viewModel.EPA.NicknameLocalBonus,
                FullNamePFileAdmin = viewModel.FullNamePFileAdmin,
                NicknamePFileAdmin = viewModel.NicknamePFileAdmin,
                IsPDFPasswordChanged = viewModel.PDFPassword != viewModel.OldPDFPassword,
                UpdatedBy = sessionService.UserId
            };

            criteria.PDFPassword = cryptoService.Encrypt(viewModel.PDFPassword);

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        private async Task SaveProfilePictureAsync(IFormFile image, string fileName)
        {
            if (image != null && image.Length != 0)
            {
                var directoryPath = Path.Combine(hostingEnvironment.WebRootPath, "ProfilePicture");
                Directory.CreateDirectory(directoryPath);

                var fullFileName = fileName + Path.GetExtension(image.FileName);
                var path = Path.Combine(directoryPath, fullFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
        }
    }
}