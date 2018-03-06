namespace HRP.Services
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
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
                if (file.Length / (1024 * 1024) > 5)
                {
                    result.Code = 1;
                    result.Message = Message.FileTooLarge;
                }
                else if (!file.ContentType.Equals(MimeType.XLS) && !file.ContentType.Equals(MimeType.XLSX))
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

        private void ReadAttendanceFromExcelFile(int companyId, DateTime month, IFormFile file)
        {
            return;
        }

        public CommandResult InsertAttendanceAndSalaryUploading(UploadAttendanceAndSalaryViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}