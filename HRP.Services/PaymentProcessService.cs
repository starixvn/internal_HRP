namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Currency;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models.Currency;
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
            ITemplateReader templateReader,
            IPaymentProcessRepository paymentProcessRepository = null,
            IObjectStream fileStream = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.templateReader = templateReader;
            this.paymentProcessRepository = paymentProcessRepository ?? new PaymentProcessRepository();
            this.fileStream = fileStream ?? new ObjectStream();
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

        public async Task<PaymentProcess> GetPaymentProcessAsync(int companyId, DateTime month)
        {
            var criteria = new GetPaymentProcessCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var process = await baseService.GetDataEntityAsync<PaymentProcess>(criteria).ConfigureAwait(false);

            return process;
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
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.AttendanceFile.OpenReadStream());
                var template = new AttendanceTemplate
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyCode = viewModel.CompanyCode,
                    Month = viewModel.SelectedTime,
                    FileName = viewModel.AttendanceFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList
                };

                var readResult = templateReader.ReadAttendance(template);
                if (readResult is Success<IEnumerable<AttendanceUploading>> success)
                {
                    template.Records = success.Value;
                    template.ValidationResults = templateReader.AttendanceValidationResults;
                    commandResult = await CreateAttendanceUploadingAsync(template).ConfigureAwait(false);

                    return commandResult;
                }

                var failure = (Failure<IEnumerable<AttendanceUploading>>)readResult;
                commandResult.Code = 1;
                commandResult.Message = failure.Message;
            }

            return commandResult;
        }

        public async Task<CommandResult> UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var commandResult = new CommandResult { Code = 0 };
            if (viewModel.DSTNetSalaryFile != null)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var currencies = await GetAllActiveCurrencyAsync().ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.DSTNetSalaryFile.OpenReadStream());
                var template = new DSTNetSalaryTemplate
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyCode = viewModel.CompanyCode,
                    Month = viewModel.SelectedTime,
                    FileName = viewModel.DSTNetSalaryFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList,
                    Currencies = currencies
                };

                var readResult = templateReader.ReadDSTNetSalary(template);
                if (readResult is Success<IEnumerable<DSTNetSalaryUploading>> success)
                {
                    template.Records = success.Value;
                    template.ValidationResults = templateReader.DSTNetSalaryValidationResults;
                    commandResult = await CreateDSTNetSalaryUploadingAsync(template).ConfigureAwait(false);

                    return commandResult;
                }

                var failure = (Failure<IEnumerable<DSTNetSalaryUploading>>)readResult;
                commandResult.Code = 1;
                commandResult.Message = failure.Message;
            }

            return commandResult;
        }

        public async Task<UploadAttendanceAndSalaryViewModel> GetUploadingFilesResultAsync(int companyId, DateTime month)
        {
            var criteria = new GetUploadingFilesResultCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            UploadAttendanceAndSalaryViewModel result = null;
            if (criteria.IsValid())
            {
                result = await paymentProcessRepository.GetUploadingFilesResultAsync(criteria).ConfigureAwait(false);
            }

            return result ?? new UploadAttendanceAndSalaryViewModel();
        }

        private async Task<IEnumerable<string>> GetAllEmployeeCodeByCompanyIdAsync(int companyId)
        {
            var employeeCodeList = await baseService.GetDataAsync<string>(new GetAllEmployeeCodeByCompanyIdCriteria { CompanyId = companyId }).ConfigureAwait(false);

            return employeeCodeList;
        }

        private async Task<IEnumerable<Currency>> GetAllActiveCurrencyAsync()
        {
            var currencies = await baseService.GetDataAsync<Currency>(new GetAllCurrencyCriteria { IsActive = true }).ConfigureAwait(false);

            return currencies;
        }

        private async Task<CommandResult> CreateAttendanceUploadingAsync(AttendanceTemplate template)
        {
            var criteria = new CreateAttendanceUploadingCriteria
            {
                SessionId = Guid.NewGuid().ToString(),
                CompanyId = template.CompanyId,
                Month = template.Month,
                FileName = template.FileName,
                UploadingXMLDoc = template.Records.ToXML().ToString(),
                ResultXMLDoc = template.ValidationResults.ToXML().ToString(),
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> CreateDSTNetSalaryUploadingAsync(DSTNetSalaryTemplate template)
        {
            var criteria = new CreateDSTNetSalaryUploadingCriteria
            {
                SessionId = Guid.NewGuid().ToString(),
                CompanyId = template.CompanyId,
                Month = template.Month,
                FileName = template.FileName,
                UploadingXMLDoc = template.Records.ToXML().ToString(),
                ResultXMLDoc = template.ValidationResults.ToXML().ToString(),
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public Task<CommandResult> GenerateMonthlyPayroll(int companyId, DateTime month, int? employeeId)
        {
            throw new NotImplementedException();
        }
    }
}