namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models.PaymentProcess;
    using HRP.Services.Interfaces;

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
            var isExisted = await baseService.GetDataEntityAsync<bool>(new CheckPayrollGeneratedCriteria { CompanyId = process.CompanyId, PaymentMonth = process.PaymentMonth }).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> UpdatePaymentProcessAsync(PaymentProcess process)
        {
            var criteria = new UpdatePaymentProcessCriteria
            {
                CompanyId = process.CompanyId,
                PaymentMonth = process.PaymentMonth,
                CurrentStep = process.CurrentStep,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }
    }
}