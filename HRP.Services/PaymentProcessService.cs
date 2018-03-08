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
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    public class PaymentProcessService : IPaymentProcessService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public PaymentProcessService(IBaseService baseService, ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
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
            var result = new CommandResult();

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
            else
            {
                result.Code = 0;
                result.Message = string.Empty;
            }

            return result;
        }

        public async Task<CommandResult> UploadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var process = new PaymentProcess
            {
                CompanyId = viewModel.CompanyId,
                Month = viewModel.SelectedTime,
                AttendanceFileName = viewModel.AttendanceFile.FileName
            };

            var result = await ReadAttendanceFromExcelFileAsync(viewModel.AttendanceFile, process).ConfigureAwait(false);
            return result;
        }

        public IEnumerable<ValidationResult> GetAttendanceUploadingResult(int companyId, DateTime month)
        {
            throw new NotImplementedException();
        }

        public CommandResult UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            return new CommandResult { Code = 0 };
        }

        private async Task<CommandResult> ReadAttendanceFromExcelFileAsync(IFormFile file, PaymentProcess process)
        {
            var commandResult = new CommandResult();
            var employeeCodeList = await baseService.GetDataAsync<string>(new GetAllEmployeeCodeByCompanyIdCriteria { CompanyId = process.CompanyId }).ConfigureAwait(false);
            var reader = new AttendanceReader(file.OpenReadStream());
            var readResult = reader.ReadAttendances(employeeCodeList, process.CompanyCode);
            if (readResult is Success<IEnumerable<AttendanceUploading>> success)
            {
                commandResult = await CreateAttendanceUploadingAsync(success.Value, reader.ValidationResults, process).ConfigureAwait(false);

                return commandResult;
            }

            var failure = (Failure<IEnumerable<AttendanceUploading>>)readResult;
            commandResult.Code = 1;
            commandResult.Message = failure.Message;
            return commandResult;
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