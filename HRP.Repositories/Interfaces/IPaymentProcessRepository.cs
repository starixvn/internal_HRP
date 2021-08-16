namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.PaymentProcess;

    public interface IPaymentProcessRepository
    {
        Task<(PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>)> GetUploadingFilesResultAsync(ICriteria criteria);

        Task<GeneratePayrollModel> GetDataForGeneratePayrollAsync(ICriteria criteria);
    }
}