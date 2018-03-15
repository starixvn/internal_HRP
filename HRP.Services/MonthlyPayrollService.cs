namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;

    public class MonthlyPayrollService : IMonthlyPayrollService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlyPayrollService(
            ISessionService sessionService,
            IBaseService baseService,
            ICryptoService cryptoService,
            IMonthlyPayrollRepository monthlyPayrollRepository = null)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.monthlyPayrollRepository = monthlyPayrollRepository ?? new MonthlyPayrollRepository();
        }

        public async Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(int companyId, string costCenterIds)
        {
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                CostCenterIds = costCenterIds
            };
            var viewModel = new EmployeeMonthlyPayrollViewModel();

            if (criteria.IsValid())
            {
                viewModel = await monthlyPayrollRepository.GetDataFilterForIndexPageAsync(criteria).ConfigureAwait(false);
            }

            return viewModel;
        }

        public async Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel)
        {
            IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls = new List<MonthlyPayrollEmployeeInfoViewModel>();
            var criteria = new GetMonthlyPayrollCriteria
            {
                UserId = sessionService.UserId,
                Month = viewModel.Month,
                CompanyId = viewModel.CompanyId,
                CostCenterIds = viewModel.CostCenterIds,
                EmployeeId = viewModel.EmployeeId,
                WorkingDays = viewModel.WorkingDays
            };

            if (criteria.IsValid())
            {
                monthlyPayrolls = await monthlyPayrollRepository.GetMonthlyPayrollAsync(criteria).ConfigureAwait(false);
                DecryptMonthlyPayroll(monthlyPayrolls);
            }

            return monthlyPayrolls;
        }

        public async Task<EditMonthlySalaryViewModel> GetMonthlySalaryToEditAsync(int payrollId)
        {
            EditMonthlySalaryViewModel viewModel = null;
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                PayrollId = payrollId,
                UserId = sessionService.UserId
            };

            if (criteria.IsValid())
            {
                viewModel = await monthlyPayrollRepository.GetMonthlySalaryToEditAsync(criteria).ConfigureAwait(false);

                if (viewModel != null)
                {
                    DecryptMonthlySalary(viewModel);
                }
            }

            return viewModel;
        }

        public Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name)
        {
            var criteria = new GetEmployeeSuggestionByNameCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                Name = name
            };

            return baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);
        }

        public async Task<CommandResult> ResetMonthlySalaryAsync(int payrollId, int companyId)
        {
            var criteria = new ResetMonthlySalaryCriteria
            {
                PayrollId = payrollId,
                CompanyId = companyId,
                UserId = sessionService.UserId
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel)
        {
            EncryptMonthlySalaryAddition(viewModel.Additions);
            EncryptMonthlySalaryDeduction(viewModel.Deductions);
            var criteria = new EditMonthlySalaryCriteria
            {
                PayrollId = viewModel.PayrollId,
                CompanyId = viewModel.CompanyId,
                UserId = sessionService.UserId,
                CurrencyId = viewModel.MonthlySalary.CurrencyId,
                Salary = cryptoService.Encrypt(viewModel.MonthlySalary.Salary),
                EBankingCurrencyId = viewModel.MonthlySalary.EBankingCurrencyId,
                EBankingAmount = cryptoService.Encrypt(viewModel.MonthlySalary.EBankingAmount),
                EBankingAccountId = viewModel.MonthlySalary.EBankingAccountId,

                DirectBanking1CurrencyId = viewModel.MonthlySalary.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking1Amount),
                DirectBanking1BankTypeId = viewModel.MonthlySalary.DirectBanking1BankTypeId,
                DirectBanking1AccountId = viewModel.MonthlySalary.DirectBanking1AccountId,

                DirectBanking2CurrencyId = viewModel.MonthlySalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking2Amount),
                DirectBanking2BankTypeId = viewModel.MonthlySalary.DirectBanking2BankTypeId,
                DirectBanking2AccountId = viewModel.MonthlySalary.DirectBanking2AccountId,

                CashCurrencyId = viewModel.MonthlySalary.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.MonthlySalary.CashAmount),
                CashAccountId = viewModel.MonthlySalary.CashAccountId,

                AdditionXml = viewModel.Additions.ToXML().ToString(),
                DeductionXml = viewModel.Deductions.ToXML().ToString()
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<MonthlyAllowancesViewModel> GetMonthlyAllowancesAsync(int payrollId)
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = payrollId };
            var result = await monthlyPayrollRepository.GetMonthlyAllowancesAsync(criteria).ConfigureAwait(false);
            var model = new MonthlyAllowancesViewModel
            {
                Allowance = result.Item1,
                Additions = result.Item2,
                Deductions = result.Item3
            };

            DecryptMonthlyAllowance(model.Allowance);
            DecryptMonthlyAllowanceAdditions(model.Additions);
            DecryptMonthlyAllowanceDeductions(model.Deductions);

            return model;
        }

        public async Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesViewModel viewModel, int userId)
        {
            var allowanceAdditionXml = ConvertAllowanceAdditionToXml(viewModel.Additions);
            var allowanceDeductionXml = ConvertAllowanceAdditionToXml(viewModel.Deductions);

            var criteria = new EditMonthlyAllowancesCriteria
            {
                PayrollId = viewModel.Allowance.PayrollId,
                CurrencyId = viewModel.Allowance.CurrencyId,
                TotalAmount = viewModel.Allowance.TotalAmount,
                DirectBanking1CurrencyId = viewModel.Allowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking1Amount),
                DirectBanking1AccountId = viewModel.Allowance.DirectBanking1AccountId,
                DirectBanking1BankTypeId = viewModel.Allowance.DirectBanking1BankTypeId,
                DirectBanking2CurrencyId = viewModel.Allowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking2Amount),
                DirectBanking2AccountId = viewModel.Allowance.DirectBanking2AccountId,
                DirectBanking2BankTypeId = viewModel.Allowance.DirectBanking2BankTypeId,
                CashCurrencyId = viewModel.Allowance.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.Allowance.CashAmount),
                CashAccountId = viewModel.Allowance.CashAccountId,
                AutoCalculateOn = viewModel.Allowance.AutoCalculateOn,
                AllowanceAddition = allowanceAdditionXml,
                AllowanceDeduction = allowanceDeductionXml,
                UserId = userId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private void DecryptMonthlyPayroll(IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls)
        {
            foreach (var monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.EBankingAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.EBankingAmount);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking1Amount);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking2Amount);
                monthlyPayroll.BasicSalary.CashAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.CashAmount);

                monthlyPayroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking1Amount);
                monthlyPayroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking2Amount);
                monthlyPayroll.Allowance.CashAmount = cryptoService.Decrypt(monthlyPayroll.Allowance.CashAmount);
            }
        }

        private void DecryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
        }

        private void DecryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);
                addition.TotalAmount = cryptoService.Decrypt(addition.TotalAmount);
            }
        }

        private void DecryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);
                deduction.TotalAmount = cryptoService.Decrypt(deduction.TotalAmount);
            }
        }

        private void EncryptMonthlySalaryAddition(IEnumerable<MonthlySalaryAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
            }
        }

        private void EncryptMonthlySalaryDeduction(IEnumerable<MonthlySalaryDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.NetSalary = cryptoService.Encrypt(deduction.NetSalary);
            }
        }

        private string ConvertAllowanceAdditionToXml(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            var xml = new XElement("Root");

            foreach (var addition in additions)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(addition.PayrollId), addition.PayrollId);
                row.SetAttributeValue(nameof(addition.Type), addition.Type);
                row.SetAttributeValue(nameof(addition.NoOfDays), addition.NoOfDays);
                row.SetAttributeValue(nameof(addition.UnitAmount), cryptoService.Encrypt(addition.UnitAmount));
                row.SetAttributeValue(nameof(addition.TotalAmount), cryptoService.Encrypt(addition.TotalAmount));
                row.SetAttributeValue(nameof(addition.NetAllowance), cryptoService.Encrypt(addition.NetAllowance));
                row.SetAttributeValue(nameof(addition.CurrencyId), addition.CurrencyId);

                xml.Add(row);
            }

            return xml.ToString();
        }

        private string ConvertAllowanceAdditionToXml(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            var xml = new XElement("Root");

            foreach (var deduction in deductions)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(deduction.PayrollId), deduction.PayrollId);
                row.SetAttributeValue(nameof(deduction.Type), deduction.Type);
                row.SetAttributeValue(nameof(deduction.NoOfDays), deduction.NoOfDays);
                row.SetAttributeValue(nameof(deduction.UnitAmount), cryptoService.Encrypt(deduction.UnitAmount));
                row.SetAttributeValue(nameof(deduction.TotalAmount), cryptoService.Encrypt(deduction.TotalAmount));
                row.SetAttributeValue(nameof(deduction.NetAllowance), cryptoService.Encrypt(deduction.NetAllowance));
                row.SetAttributeValue(nameof(deduction.CurrencyId), deduction.CurrencyId);

                xml.Add(row);
            }

            return xml.ToString();
        }

        private void DecryptMonthlySalary(EditMonthlySalaryViewModel viewModel)
        {
            viewModel.MonthlyBasicSalary.Salary = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.Salary);
            viewModel.MonthlyBasicSalary.EBankingAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.EBankingAmount);
            viewModel.MonthlyBasicSalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking1Amount);
            viewModel.MonthlyBasicSalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking2Amount);
            viewModel.MonthlyBasicSalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.CashAmount);

            viewModel.MonthlySalary.Salary = cryptoService.Decrypt(viewModel.MonthlySalary.Salary);
            viewModel.MonthlySalary.EBankingAmount = cryptoService.Decrypt(viewModel.MonthlySalary.EBankingAmount);
            viewModel.MonthlySalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking1Amount);
            viewModel.MonthlySalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking2Amount);
            viewModel.MonthlySalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlySalary.CashAmount);

            foreach (MonthlySalaryAdditionModel addition in viewModel.Additions)
            {
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);
            }

            foreach (MonthlySalaryDeductionModel deduction in viewModel.Deductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);
            }
        }
    }
}