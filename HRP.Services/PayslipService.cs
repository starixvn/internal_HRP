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
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Repositories;

    public class PayslipService : IPayslipService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IPayslipRepository payslipRepository;

        public PayslipService(
            IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            IPayslipRepository payslipRepository = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.payslipRepository = payslipRepository ?? new PayslipRepository();
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

        public async Task<PayslipDetailModel> GetPayslipDetailAsync(int payrollId, int companyId)
        {
            var criteria = new GetPayslipDetailsCriteria
            {
                UserId = sessionService.UserId,
                PayrollIds = payrollId.ToString(),
                CompanyId = companyId
            };

            PayslipDetailModel payslip = await payslipRepository.GetPayslipDetailAsync(criteria).ConfigureAwait(false);

            if (payslip != null)
            {
                DescryptPayslip(payslip);
            }

            return payslip;
        }

        private void DescryptPayslip(PayslipDetailModel payslip)
        {
            payslip.SalaryAmount = cryptoService.Decrypt(payslip.SalaryAmount);
            payslip.AllowanceAmount = cryptoService.Decrypt(payslip.AllowanceAmount);

            foreach (MonthlySalaryAdditionModel addition in payslip.SalaryAdditions)
            {
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);
            }

            foreach (MonthlySalaryDeductionModel deduction in payslip.SalaryDeductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);
            }

            foreach (MonthlyAllowanceAdditionModel addition in payslip.AllowanceAdditions)
            {
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);
            }

            foreach (MonthlyAllowanceDeductionModel deduction in payslip.AllowanceDeductions)
            {
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);
            }
        }
    }
}