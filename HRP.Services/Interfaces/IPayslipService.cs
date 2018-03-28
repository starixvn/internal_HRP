namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Models.Payslip;

    public interface IPayslipService
    {
        Task<IEnumerable<PayslipModel>> GetPayslipAsync(DateTime month, int companyId, bool? isSent);
    }
}