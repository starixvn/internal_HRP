namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using Microsoft.AspNetCore.Http;

    public interface IPaymentProcessService
    {
        Task<bool> IsPayrollGeneratedAsync(PaymentProcess process);

        Task<UploadAttendanceAndSalaryViewModel> GetDataForUploadAttendanceAndSalary(int companyId, DateTime month);

        CommandResult CheckValidExcelFile(IFormFile file);

        Task<CommandResult> UploadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel);

        IEnumerable<ValidationResult> GetAttendanceUploadingResult(int companyId, DateTime month);

        CommandResult UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel);
    }
}