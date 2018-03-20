namespace HRP.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IPaymentProcessService
    {
        Task<SelectList> GetCompanySelectListAsync();

        Task<MemoryStream> CopyFileToMemoryStreamAsync(string filePath);

        Task<bool> IsPayrollGeneratedAsync(PaymentProcess process);

        Task<PaymentProcess> GetPaymentProcessAsync(int companyId, DateTime month);

        CommandResult CheckValidExcelFile(IFormFile file);

        Task<CommandResult> UploadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel);

        Task<UploadAttendanceAndSalaryViewModel> GetUploadingFilesResultAsync(int companyId, DateTime month);

        Task<CommandResult> UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel);

        Task<CommandResult> SubmitUploadingFilesAsync(int companyId, DateTime month);

        Task<CommandResult> GenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId);
    }
}