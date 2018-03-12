namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    public class PaymentProcessService : IPaymentProcessService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IPaymentProcessRepository paymentProcessRepository;
        private readonly IObjectStream fileStream;
        private readonly ITemplateReader templateReader;

        public PaymentProcessService(IBaseService baseService,
            ISessionService sessionService,
            IPaymentProcessRepository paymentProcessRepository = null,
            IObjectStream fileStream = null,
            ITemplateReader templateReader = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.paymentProcessRepository = paymentProcessRepository ?? new PaymentProcessRepository();
            this.fileStream = fileStream ?? new ObjectStream();
            this.templateReader = templateReader ?? new TemplateReader();
        }

        public async Task<MemoryStream> CopyFileToMemoryStreamAsync(string filePath)
        {
            var memory = new MemoryStream();
            using (var stream = fileStream.CreateInstance(filePath))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }

            memory.Position = 0;

            return memory;
        }

        public async Task<bool> IsPayrollGeneratedAsync(PaymentProcess process)
        {
            var isExisted = await baseService.GetDataEntityAsync<bool>(new CheckPayrollGeneratedCriteria { CompanyId = process.CompanyId, Month = process.Month }).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<UploadAttendanceAndSalaryViewModel> GetDataForUploadAttendanceAndSalary(int companyId, DateTime month)
        {
            var company = await baseService.GetDataEntityAsync<Company>(new GetCompanyByIdCriteria { CompanyId = companyId }).ConfigureAwait(false);

            var viewModel = new UploadAttendanceAndSalaryViewModel
            {
                CompanyId = companyId,
                CompanyCode = company.Code,
                SelectedTime = month
            };

            return viewModel;
        }

        public CommandResult CheckValidExcelFile(IFormFile file)
        {
            var result = new CommandResult { Code = 0 };

            if (file != null)
            {
                var fileExt = Path.GetExtension(file.FileName);
                if (file.Length / (1024 * 1024) > 5)
                {
                    result.Code = 1;
                    result.Message = Message.FileTooLarge;
                }
                else if (!(fileExt.Equals(".xls") || fileExt.Equals(".xlsx")))
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

            return result;
        }

        public async Task<CommandResult> UploadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var commandResult = new CommandResult { Code = 0 };
            if (viewModel.AttendanceFile != null)
            {
                var employeeCodeList = await GetDataForReadAttendanceFileAsync(viewModel.CompanyId).ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.AttendanceFile.OpenReadStream());
                var readResult = templateReader.ReadAttendance(sheet, employeeCodeList, viewModel.CompanyCode);
                if (readResult is Success<IEnumerable<AttendanceUploading>> success)
                {
                    var process = new PaymentProcess
                    {
                        CompanyId = viewModel.CompanyId,
                        Month = viewModel.SelectedTime,
                        AttendanceFileName = viewModel.AttendanceFile.FileName
                    };
                    commandResult = await CreateAttendanceUploadingAsync(success.Value, templateReader.ValidationResults, process).ConfigureAwait(false);

                    return commandResult;
                }

                var failure = (Failure<IEnumerable<AttendanceUploading>>)readResult;
                commandResult.Code = 1;
                commandResult.Message = failure.Message;
            }

            return commandResult;
        }

        private async Task<IEnumerable<string>> GetDataForReadAttendanceFileAsync(int companyId)
        {
            var employeeCodeList = await baseService.GetDataAsync<string>(new GetAllEmployeeCodeByCompanyIdCriteria { CompanyId = companyId }).ConfigureAwait(false);

            return employeeCodeList;
        }

        public async Task<ValidationFilesResultViewModel> GetValidationFilesResultAsync(int companyId, DateTime month)
        {
            var criteria = new GetValidationFilesResultCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            ValidationFilesResultViewModel result = null;
            if (criteria.IsValid())
            {
                result = await paymentProcessRepository.GetValidationFilesResultAsync(criteria).ConfigureAwait(false);
            }

            return result ?? new ValidationFilesResultViewModel();
        }

        public CommandResult UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            return new CommandResult { Code = 0 };
        }

        private async Task<CommandResult> CreateAttendanceUploadingAsync(IEnumerable<AttendanceUploading> records, IEnumerable<ValidationResult> results, PaymentProcess process)
        {
            var criteria = new CreateAttendanceUploadingCriteria
            {
                SessionId = Guid.NewGuid().ToString(),
                CompanyId = process.CompanyId,
                Month = process.Month,
                FileName = process.AttendanceFileName,
                UploadingXMLDoc = records.ToXML().ToString(),
                ResultXMLDoc = results.ToXML().ToString(),
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }
    }
}