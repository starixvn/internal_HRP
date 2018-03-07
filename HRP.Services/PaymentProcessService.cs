namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
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

        public async Task<CommandResult> UpdatePaymentProcessAsync(PaymentProcess process)
        {
            var criteria = new UpdatePaymentProcessCriteria
            {
                CompanyId = process.CompanyId,
                Month = process.Month,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
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

        private static string ReadAttendanceFromExcelFile(IFormFile file, IEnumerable<string> employeeCodeList, string companyName)
        {
            var reader = new AttendanceRecordReader(file.OpenReadStream());

            reader.ValidateAttendanceRecords(employeeCodeList, companyName);

            return reader.ValidationErrors.ToString();
        }

        public async Task<CommandResult> InsertAttendanceAndSalaryUploadingAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var result = new CommandResult();
            var employeeCodeList = await baseService.GetDataAsync<string>(new GetAllEmployeeCodeCriteria { CompanyId = viewModel.CompanyId }).ConfigureAwait(false);

            result.Message = ReadAttendanceFromExcelFile(viewModel.AttendanceFile, employeeCodeList, viewModel.CompanyCode);
            return result;
        }
    }
}