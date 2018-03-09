namespace HRP.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.PaymentProcess;

    public interface IPaymentProcessRepository
    {
        Task<ValidationFilesResultViewModel> GetValidationFilesResultAsync(ICriteria criteria);
    }
}