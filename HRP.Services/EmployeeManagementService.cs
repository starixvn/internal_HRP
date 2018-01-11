namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using HRP.Core.Model;
    using HRP.Core.Services;
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

        public async Task<CommandResult> CreateEmployeeAsync(CreateEmployeeViewModel employee)
        {
            var pictureExtension = employee.ProfilePicture != null ? Path.GetExtension(employee.ProfilePicture.FileName) : null;
            var criteria = new CreateEmployeeCriteria
            {
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
                PictureExtension = pictureExtension,
                CreatedBy = sessionService.UserId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            await SaveProfilePictureAsync(employee.ProfilePicture, criteria.EmployeeId.ToString()).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditEmployeeAsync(CreateEmployeeViewModel employee)
        {
            var criteria = new EditEmployeeCriteria
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

        public CreateEmployeeViewModel GetEmployeeById(int employeeId)
        {
            var currentDate = DateTime.Now;
            var result = new CreateEmployeeViewModel
            {
                DateOfBirth = currentDate,
                DateIssue = currentDate,
                ExpiredDate = currentDate
            };

            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId,
            };

            if (criteria.IsValid())
            {
                using (var db = new ObjectDb(criteria.GetSettingKey()))
                {
                    using (var multi = db.QueryMultiple(criteria))
                    {
                        var employee = multi.Read<CreateEmployeeViewModel>().FirstOrDefault();
                        var genderList = multi.Read<ReferenceResourceModel>();
                        var nationalityList = multi.Read<ReferenceResourceModel>();
                        var maritalStatusList = multi.Read<ReferenceResourceModel>();
                        var noOfChildrenList = multi.Read<ReferenceResourceModel>();

                        result = employee ?? result;
                        result.GenderList = GetSelectList(genderList);
                        result.NationalityList = GetSelectList(nationalityList);
                        result.MaritalStatusList = GetSelectList(maritalStatusList);
                        result.NoOfChildrenList = GetSelectList(noOfChildrenList);
                    }
                }
            }

            return result;
        }

        public CommandResult CheckValidPicture(IFormFile file)
        {
            var result = new CommandResult();

            if (file != null)
            {
                if (file.Length / (1024 * 1024) > 5)
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

        public int MyProperty { get; set; }

        public async Task<UpdateEmploymentViewModel> GetEmploymentToUpdateAsync(int employeeId)
        {
            var criteria = new GetEmploymentByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            UpdateEmploymentViewModel employment = await employeeRepository.GetEmploymentToUpdateAsync(criteria).ConfigureAwait(false);
            employment.PDFPassword = certificateService.Decrypt(employment.PDFPassword);

            return employment;
        }

        public async Task<CommandResult> CreateEmploymentAsync(CreateEmploymentViewModel viewModel)
        {
            var criteria = new CreateEmploymentCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                LocalCompanyId = viewModel.LocalCompanyId,
                JobTitle = viewModel.JobTitle,
                LocalJobTitleId = viewModel.LocalJobTitleId,
                CompanyCode = viewModel.CompanyCode,
                EmployeeCode = viewModel.EmployeeCode,
                EmploymentType = viewModel.EmploymentType,
                EmploymentStatus = viewModel.EmploymentStatus,
                JobCategoryId = viewModel.JobCategoryId,
                GradeId = viewModel.GradeId,
                GradeLevelId = viewModel.GradeLevelId,
                DivisionId = viewModel.DivisionId,
                DepartmentId = viewModel.DepartmentId,
                SectionId = viewModel.SectionId,
                CostCenterId = viewModel.CostCenterId,
                GroupStartDate = viewModel.GroupStartDate,
                CompanyStartDate = viewModel.CompanyStartDate,
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

        public async Task<CommandResult> UpdateEmploymentAsync(UpdateEmploymentViewModel viewModel)
        {
            var criteria = new UpdateEmploymentCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                CompanyId = viewModel.CompanyId,
                LocalCompanyId = viewModel.LocalCompanyId,
                JobTitle = viewModel.JobTitle,
                LocalJobTitleId = viewModel.LocalJobTitleId,
                CompanyCode = viewModel.CompanyCode,
                EmployeeCode = viewModel.EmployeeCode,
                EmploymentType = viewModel.EmploymentType,
                EmploymentStatus = viewModel.EmploymentStatus,
                JobCategoryId = viewModel.JobCategoryId,
                GradeId = viewModel.GradeId,
                GradeLevelId = viewModel.GradeLevelId,
                DivisionId = viewModel.DivisionId,
                DepartmentId = viewModel.DepartmentId,
                SectionId = viewModel.SectionId,
                CostCenterId = viewModel.CostCenterId,
                GroupStartDate = viewModel.GroupStartDate,
                CompanyStartDate = viewModel.CompanyStartDate,
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