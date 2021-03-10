namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.PaymentProcess;

    public interface IPaymentProcessRepository
    {
        Task<(PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>)> GetUploadingFilesResultAsync(ICriteria criteria);

        Task<GeneratePayrollModel> GetDataForGeneratePayroll(ICriteria criteria);
    }
}