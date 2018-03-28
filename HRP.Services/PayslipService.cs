namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.Payslip;
    using HRP.Models.Payslip;

    public class PayslipService : IPayslipService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public PayslipService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public Task<IEnumerable<PayslipModel>> GetPayslipAsync(DateTime month, int companyId, bool? isSent)
        {
            var criteria = new GetPayslipCriteria
            {
                UserId = sessionService.UserId,
                Month = month,
                CompanyId = companyId,
                IsSent = isSent
            };

            return baseService.GetDataAsync<PayslipModel>(criteria);
        }
    }
}