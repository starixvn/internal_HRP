namespace HRP.Services.Interfaces
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;

    public interface IPaymentProcessService
    {
        Task<CommandResult> UpdatePaymentProcessAsync(PaymentProcess process);

        Task<bool> IsPayrollGeneratedAsync(PaymentProcess process);
    }
}