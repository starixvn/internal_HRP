namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.Payslip;

    public interface IPayslipRepository
    {
        Task<IEnumerable<PayslipDetailModel>> GetPayslipDetailsAsync(ICriteria criteria);
    }
}