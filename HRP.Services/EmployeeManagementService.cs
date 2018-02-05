namespace HRP.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IBaseService baseService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IEmployeeManagementRepository employeeRepository;

        public EmployeeManagementService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ICryptoService cryptoService,
            ISessionService sessionService,
            IEmployeeManagementRepository employeeRepository = null)
        {
            this.baseService = baseService;
            this.hostingEnvironment = hostingEnvironment;
            this.cryptoService = cryptoService;
            this.sessionService = sessionService;
            this.employeeRepository = employeeRepository ?? new EmployeeManagementRepository();
        }

        public EmployeeManagementViewModel GetDataFilterForIndexPage(string departmentIds, string employmentStatusIds)
        {
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = sessionService.UserId,
                DepartmentIds = departmentIds,
                EmploymentStatusIds = employmentStatusIds
            };
            var viewModel = new EmployeeManagementViewModel();

            if (criteria.IsValid())
            {
                viewModel = employeeRepository.GetDataFilterForIndexPage(criteria);
            }

            return viewModel;
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
                CompanyCode = employee.CompanyCode,
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

        public CreateEmployeeViewModel GetPersonalDetails(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId,
                UserId = sessionService.UserId,
                FunctionKey = FunctionKey.EmployeeProfile
            };
            CreateEmployeeViewModel employee = null;

            if (criteria.IsValid())
            {
                employee = employeeRepository.GetPersonalDetails(criteria);
            }

            return employee;
        }

        public async Task<CreateEmployeeViewModel> GetDataToCreateEmployeeAsync()
        {
            var refNames = string.Format(
                 "{0},{1},{2},{3}",
                 ReferenceResource.Gender,
                 ReferenceResource.Nationality,
                 ReferenceResource.MaritalStatus,
                 ReferenceResource.NoOfChildren);
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames }).ConfigureAwait(false);
            var listGender = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Gender)));
            var listNational = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Nationality)));
            var listMaritalStatus = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.MaritalStatus)));
            var listNoOfChildren = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.NoOfChildren)));
            var viewModel = new CreateEmployeeViewModel
            {
                GenderList = listGender,
                NationalityList = listNational,
                MaritalStatusList = listMaritalStatus,
                NoOfChildrenList = listNoOfChildren
            };

            return viewModel;
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
                    result.Message = Message.InvalidImageType;
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

        public CreateEmploymentViewModel GetDataToCreateEmployment()
        {
            var viewModel = new CreateEmploymentViewModel();
            var criteria = new GetDataToCreateEmploymentCriteria
            {
                UserId = sessionService.UserId
            };

            if (criteria.IsValid())
            {
                viewModel = employeeRepository.GetDataToCreateEmployment(criteria);
            }

            return viewModel;
        }

        public UpdateEmploymentViewModel GetDataToUpdateEmployment(int employeeId)
        {
            var criteria = new GetDataToUpdateEmploymentCriteria
            {
                UserId = sessionService.UserId,
                EmployeeId = employeeId
            };
            UpdateEmploymentViewModel employment = null;

            if (criteria.IsValid())
            {
                employment = employeeRepository.GetDataToUpdateEmployment(criteria);

                if (employment != null)
                {
                    employment.PDFPassword = cryptoService.Decrypt(employment.PDFPassword);
                    employment.OldPDFPassword = employment.PDFPassword;
                    employment.IsHost = sessionService.HasPermissionOnCompany(employment.CompanyId, FunctionKey.EmployeeProfile, Core.Enumerations.Policy.Host);
                }
            }

            return employment;
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
                CompanyCode = viewModel.CompanyCode,
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

        private static SelectList GetSelectList(IEnumerable<ReferenceResourceModel> refResource)
        {
            return new SelectList(refResource, "ResourceId", "Value");
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