namespace HRP.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly IBaseService baseService;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ISessionService sessionService;
        private readonly ICertificateService certificateService;
        private readonly IEmployeeManagementRepository employeeRepository;

        public EmployeeManagementService(
            IHostingEnvironment hostingEnvironment,
            IBaseService baseService,
            ICertificateService certificateService,
            ISessionService sessionService,
            IEmployeeManagementRepository employeeRepository = null)
        {
            this.baseService = baseService;
            this.hostingEnvironment = hostingEnvironment;
            this.certificateService = certificateService;
            this.sessionService = sessionService;
            this.employeeRepository = employeeRepository ?? new EmployeeManagementRepository();
        }

        public async Task<CommandResult> CreateEmployeeAsync(CreateEmploymentViewModel employment)
        {
            var pictureExtension = employment.ProfilePicture != null ? Path.GetExtension(employment.ProfilePicture.FileName) : null;
            var criteria = new CreateEmployeeCriteria
            {
                FullName = employment.FullName,
                NickName = employment.NickName,
                Gender = employment.Gender,
                DateOfBirth = employment.DateOfBirth,
                PlaceOfBirth = employment.PlaceOfBirth,
                Nationality = employment.Nationality,
                NRICNo = employment.NRICNo,
                MaritalStatus = employment.MaritalStatus,
                NoOfChildren = employment.NoOfChildren,
                OverseasPhoneNo = employment.OverseasPhoneNo,
                LocalPhoneNo = employment.LocalPhoneNo,
                OtherPhoneNo = employment.OtherPhoneNo,
                PassportNo = employment.PassportNo,
                DateIssue = employment.DateIssue,
                ExpiredDate = employment.ExpiredDate,
                PlaceOfIssue = employment.PlaceOfIssue,
                Skype = employment.Skype,
                PersonalEmail = employment.PersonalEmail,
                PermanentAddress = employment.PermanentAddress,
                ResidenceAddress = employment.ResidenceAddress,
                MailingAddress = employment.MailingAddress,
                PictureExtension = pictureExtension,
                ECName = employment.EmergencyContact.Name,
                ECAddress = employment.EmergencyContact.Address,
                ECRelationship = employment.EmergencyContact.Relationship,
                ECPhoneNo1 = employment.EmergencyContact.PhoneNo1,
                ECPhoneNo2 = employment.EmergencyContact.PhoneNo2,
                CompanyId = employment.CompanyId,
                CompanyChangeDate = employment.CompanyChangeDate,
                CompanyReasonId = employment.CompanyReasonId,
                LocalCompanyId = employment.LocalCompanyId,
                LocalCompanyChangeDate = employment.LocalCompanyChangeDate,
                LocalCompanyReasonId = employment.LocalCompanyReasonId,
                JobTitle = employment.JobTitle,
                JobTitleChangeDate = employment.JobTitleChangeDate,
                JobTitleReasonId = employment.JobTitleReasonId,
                LocalJobTitleId = employment.LocalJobTitleId,
                LocalJobTitleChangeDate = employment.LocalJobTitleChangeDate,
                LocalJobTitleReasonId = employment.LocalJobTitleReasonId,
                CompanyCode = employment.CompanyCode,
                EmployeeCode = employment.EmployeeCode,
                IDNoChangeDate = employment.IDNoChangeDate,
                IDNoReasonId = employment.IDNoReasonId,
                EmploymentType = employment.EmploymentType,
                EmploymentStatus = employment.EmploymentStatus,
                EmploymentStatusChangeDate = employment.EmploymentStatusChangeDate,
                JobCategoryId = employment.JobCategoryId,
                JobCategoryChangeDate = employment.JobCategoryChangeDate,
                JobCategoryReasonId = employment.JobCategoryReasonId,
                JobGradeId = employment.JobGradeId,
                GradeLevelId = employment.GradeLevelId,
                JobGradeChangeDate = employment.JobGradeChangeDate,
                JobGradeReasonId = employment.JobGradeReasonId,
                DivisionId = employment.DivisionId,
                DivisionChangeDate = employment.DivisionChangeDate,
                DivisionReasonId = employment.DivisionReasonId,
                DepartmentId = employment.DepartmentId,
                DepartmentChangeDate = employment.DepartmentChangeDate,
                DepartmentReasonId = employment.DepartmentReasonId,
                SectionId = employment.SectionId,
                SectionChangeDate = employment.SectionChangeDate,
                SectionReasonId = employment.SectionReasonId,
                CostCenterId = employment.CostCenterId,
                CostCenterChangeDate = employment.CostCenterChangeDate,
                CostCenterReasonId = employment.CostCenterReasonId,
                GroupStartDate = employment.GroupStartDate,
                GroupEndDate = employment.GroupEndDate,
                GroupDateReasonId = employment.GroupDateReasonId,
                CompanyStartDate = employment.CompanyStartDate,
                CompanyEndDate = employment.CompanyEndDate,
                CompanyDateReasonId = employment.CompanyDateReasonId,
                Email = employment.Email,
                CountryId = employment.CountryId,
                CityId = employment.CountryId,
                OfficeId = employment.OfficeId,
                DST = employment.DST,
                DBT = employment.DBT,
                TaxIDNo = employment.TaxIDNo,
                SSSNo = employment.SSSNo,
                HealthInsNo = employment.HealthInsNo,
                HDMFNo = employment.HDMFNo,
                WorkPermitCompanyId = employment.WorkPermit.LocalCompanyId,
                Type1 = employment.WorkPermit.Type1,
                StartDateType1 = employment.WorkPermit.StartDateType1,
                EndDateType1 = employment.WorkPermit.EndDateType1,
                Type2 = employment.WorkPermit.Type2,
                StartDateType2 = employment.WorkPermit.StartDateType2,
                EndDateType2 = employment.WorkPermit.EndDateType2,
                FullNameBVISalary = employment.Paymaster.FullNameBVISalary,
                NickNameBVISalary = employment.Paymaster.NickNameBVISalary,
                FullNameBVIAllowance = employment.Paymaster.FullNameBVIAllowance,
                NickNameBVIAllowance = employment.Paymaster.NickNameBVIAllowance,
                FullNameLocalSalary = employment.Paymaster.FullNameLocalSalary,
                NickNameLocalSalary = employment.Paymaster.NickNameLocalSalary,
                FullNameAdministrator = employment.EPA.FullNameAdministrator,
                NickNameAdministrator = employment.EPA.NickNameAdministrator,
                FullNameEvaluator1 = employment.EPA.FullNameEvaluator1,
                NickNameEvaluator1 = employment.EPA.NickNameEvaluator1,
                FullNameEvaluator2 = employment.EPA.FullNameEvaluator2,
                NickNameEvaluator2 = employment.EPA.NickNameEvaluator2,
                FullNameReviewer = employment.EPA.FullNameReviewer,
                NickNameReviewer = employment.EPA.NickNameReviewer,
                FullNameManagement = employment.EPA.FullNameManagement,
                NickNameManagement = employment.EPA.NickNameManagement,
                FullNameIncrement = employment.EPA.FullNameIncrement,
                NickNameIncrement = employment.EPA.NickNameIncrement,
                FullNameBVIBonus = employment.EPA.FullNameBVIBonus,
                NickNameBVIBonus = employment.EPA.NickNameBVIBonus,
                FullNameLocalBonus = employment.EPA.FullNameLocalBonus,
                NickNameLocalBonus = employment.EPA.NickNameLocalBonus,
                FullNamePFileAdmin = employment.FullNamePFileAdmin,
                NickNamePFileAdmin = employment.NickNamePFileAdmin,
                CreatedBy = sessionService.UserId
            };
            criteria.PDFPassword = certificateService.Encrypt(employment.PDFPassword);

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                await SaveProfilePictureAsync(employment.ProfilePicture, criteria.EmployeeId.ToString()).ConfigureAwait(false);
                commandResult.Data = criteria.EmployeeId;
            }

            return commandResult;
        }

        public async Task<CommandResult> EditEmployeeAsync(CreateEmployeeViewModel employee)
        {
            var criteria = new UpdateEmployeeCriteria
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                NickName = employee.NickName,
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
                ProfilePic = employee.ProfilePic,
                UpdatedBy = sessionService.UserId
            };

            if (employee.ProfilePicture != null)
            {
                criteria.ProfilePic = employee.EmployeeId + Path.GetExtension(employee.ProfilePicture.FileName);
                await SaveProfilePictureAsync(employee.ProfilePicture, employee.EmployeeId.ToString()).ConfigureAwait(false);
            }

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public ListCompanyDivisionDepartment GetAllCompanyDivisonDepartment()
        {
            var criteria = new GetAllCompanyDivsionDepartmentCriteria { UserId = sessionService.UserId };

            if (criteria.IsValid())
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var multi = db.QueryMultiple(criteria))
                    {
                        var companies = multi.Read<Company>();
                        var divisions = multi.Read<Division>();
                        var departments = multi.Read<Department>();
                        var listDepartment = new ListCompanyDivisionDepartment();
                        if (companies != null && divisions != null && departments != null)
                        {
                            listDepartment.Companies = companies;
                            listDepartment.Divisions = divisions;
                            listDepartment.Departments = departments;
                        }
                        return listDepartment;
                    }
                }
            }
            return null;
        }

        public CreateEmployeeViewModel GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId,
            };

            if (criteria.IsValid())
            {
                using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
                {
                    using (var multi = db.QueryMultiple(criteria))
                    {
                        var employee = multi.Read<CreateEmployeeViewModel>().FirstOrDefault();
                        var emergencyContact = multi.Read<EmergencyContact>().FirstOrDefault();
                        var genderList = multi.Read<ReferenceResourceModel>();
                        var nationalityList = multi.Read<ReferenceResourceModel>();
                        var maritalStatusList = multi.Read<ReferenceResourceModel>();
                        var noOfChildrenList = multi.Read<ReferenceResourceModel>();

                        if (employee != null)
                        {
                            employee.EmergencyContact = emergencyContact;
                            employee.GenderList = GetSelectList(genderList);
                            employee.NationalityList = GetSelectList(nationalityList);
                            employee.MaritalStatusList = GetSelectList(maritalStatusList);
                            employee.NoOfChildrenList = GetSelectList(noOfChildrenList);
                            employee.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeProfile, Core.Enumerations.Policy.Host);
                        }

                        return employee;
                    }
                }
            }

            return null;
        }

        public async Task<CreateEmployeeViewModel> InitDataForNewEmployeeAsync()
        {
            var refNames = string.Format(
                 "{0},{1},{2},{3}",
                 ReferenceResource.Gender,
                 ReferenceResource.Nationality,
                 ReferenceResource.MaritalStatus,
                 ReferenceResource.NoOfChildren);
            var refResources = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refNames }).ConfigureAwait(false);

            var viewModel = new CreateEmployeeViewModel
            {
                GenderList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Gender))),
                NationalityList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.Nationality))),
                MaritalStatusList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.MaritalStatus))),
                NoOfChildrenList = GetSelectList(refResources.Where(x => x.RefName.Equals(ReferenceResource.NoOfChildren))),
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.EmployeeProfile, Core.Enumerations.Policy.Host)
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

        public UpdateEmploymentViewModel GetEmploymentToUpdate(int employeeId)
        {
            var criteria = new GetEmploymentByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            UpdateEmploymentViewModel employment = employeeRepository.GetEmploymentToUpdate(criteria);
            employment.OldCompanyId = employment.CompanyId;
            employment.OldLocalCompanyId = employment.LocalCompanyId;
            employment.OldJobTitle = employment.JobTitle;
            employment.OldLocalJobTitleId = employment.LocalJobTitleId;
            employment.OldGroupStartDate = employment.GroupStartDate;
            employment.OldGroupEndDate = employment.GroupEndDate;
            employment.OldCompanyStartDate = employment.CompanyStartDate;
            employment.OldCompanyEndDate = employment.CompanyEndDate;
            employment.OldCompanyCode = employment.CompanyCode;
            employment.OldEmployeeCode = employment.EmployeeCode;
            employment.OldEmploymentStatus = employment.EmploymentStatus;
            employment.OldJobCategoryId = employment.JobCategoryId;
            employment.OldJobGradeId = employment.JobGradeId;
            employment.OldGradeLevelId = employment.GradeLevelId;
            employment.OldDivisionId = employment.DivisionId;
            employment.OldDepartmentId = employment.DepartmentId;
            employment.OldSectionId = employment.SectionId;
            employment.OldCostCenterId = employment.CostCenterId;
            employment.PDFPassword = certificateService.Decrypt(employment.PDFPassword);
            employment.IsHost = sessionService.HasPermissionOnCompany(employment.CompanyId, FunctionKey.EmployeeProfile, Core.Enumerations.Policy.Host);

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
                CityId = viewModel.CountryId,
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
                NickNameBVISalary = viewModel.Paymaster.NickNameBVISalary,
                FullNameBVIAllowance = viewModel.Paymaster.FullNameBVIAllowance,
                NickNameBVIAllowance = viewModel.Paymaster.NickNameBVIAllowance,
                FullNameLocalSalary = viewModel.Paymaster.FullNameLocalSalary,
                NickNameLocalSalary = viewModel.Paymaster.NickNameLocalSalary,
                FullNameAdministrator = viewModel.EPA.FullNameAdministrator,
                NickNameAdministrator = viewModel.EPA.NickNameAdministrator,
                FullNameEvaluator1 = viewModel.EPA.FullNameEvaluator1,
                NickNameEvaluator1 = viewModel.EPA.NickNameEvaluator1,
                FullNameEvaluator2 = viewModel.EPA.FullNameEvaluator2,
                NickNameEvaluator2 = viewModel.EPA.NickNameEvaluator2,
                FullNameReviewer = viewModel.EPA.FullNameReviewer,
                NickNameReviewer = viewModel.EPA.NickNameReviewer,
                FullNameManagement = viewModel.EPA.FullNameManagement,
                NickNameManagement = viewModel.EPA.NickNameManagement,
                FullNameIncrement = viewModel.EPA.FullNameIncrement,
                NickNameIncrement = viewModel.EPA.NickNameIncrement,
                FullNameBVIBonus = viewModel.EPA.FullNameBVIBonus,
                NickNameBVIBonus = viewModel.EPA.NickNameBVIBonus,
                FullNameLocalBonus = viewModel.EPA.FullNameLocalBonus,
                NickNameLocalBonus = viewModel.EPA.NickNameLocalBonus,
                FullNamePFileAdmin = viewModel.FullNamePFileAdmin,
                NickNamePFileAdmin = viewModel.NickNamePFileAdmin,
                CreatedBy = sessionService.UserId
            };

            criteria.PDFPassword = certificateService.Encrypt(viewModel.PDFPassword);

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