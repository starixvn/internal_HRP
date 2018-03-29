namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.Payslip;

    public interface IPayslipRepository
    {
        Task<PayslipDetailModel> GetPayslipDetailAsync(ICriteria criteria);
    }
}