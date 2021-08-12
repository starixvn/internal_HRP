namespace HRP.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;

    public interface IPaymentProcessService
    {
        Task<PaymentProcess> GetCurrentDuration();

        Task<MemoryStream> CopyFileToMemoryStreamAsync(string filePath);

        Task<PaymentProcess> GetPaymentProcessAsync(int companyId, DateTime month);

        Task<Result<AttendanceTemplate>> ReadAttendanceFileAsync(UploadFilesViewModel viewModel);

        Task<Result<DSTNetSalaryTemplate>> ReadDSTNetSalaryFileAsync(UploadFilesViewModel viewModel);

        Task<Result<AdditionDeductionTemplate>> ReadAdditionDeductionFileAsync(UploadFilesViewModel viewModel);

        Task<CommandResult> UploadFilesAsync(
            int companyId,
            DateTime month,
            AttendanceTemplate attendanceTemplate,
            DSTNetSalaryTemplate dstNetSalaryTemplate,
            AdditionDeductionTemplate additionDeductionTemplate);

        Task<UploadFilesViewModel> GetUploadingFilesResultAsync(int companyId, DateTime month);

        Task<CommandResult> DeleteUploadedAttendance(int companyId, DateTime month);

        Task<CommandResult> DeleteUploadedDSTNetSalary(int companyId, DateTime month);

        Task<CommandResult> DeleteUploadedAdditionDeductionAsync(int companyId, DateTime month);

        Task<CommandResult> SubmitUploadingFilesAsync(int companyId, DateTime month);

        Task<CommandResult> GenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId);
    }
}