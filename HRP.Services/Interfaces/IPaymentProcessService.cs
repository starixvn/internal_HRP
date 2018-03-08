namespace HRP.Services
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using Microsoft.AspNetCore.Http;

    public interface IPaymentProcessService
    {
        Task<bool> IsPayrollGeneratedAsync(PaymentProcess process);

        Task<UploadAttendanceAndSalaryViewModel> GetDataForUploadAttendanceAndSalary(int companyId, DateTime month);

        CommandResult CheckValidExcelFile(IFormFile file);

        Task<CommandResult> UploadAttendanceRecordFileAsync(UploadAttendanceAndSalaryViewModel viewModel);
    }
}