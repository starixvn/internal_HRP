namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Payslip;
    using HRP.Models;
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

        public async Task<IEnumerable<Company>> GetCompanyAsync()
        {
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = sessionService.UserId,
                FunctionKey = FunctionKey.Payslip
            };

            return await baseService.GetDataAsync<Company>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PayslipModel>> GetPayslipAsync(DateTime month, int companyId, bool? isSent)
        {
            var criteria = new GetPayslipCriteria
            {
                UserId = sessionService.UserId,
                Month = month,
                CompanyId = companyId,
                IsSent = isSent
            };

            return await baseService.GetDataAsync<PayslipModel>(criteria).ConfigureAwait(false);
        }
    }
}