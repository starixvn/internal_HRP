namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Currency;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories;
    using HRP.Repositories.Interfaces;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PaymentProcessService : IPaymentProcessService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollService monthlyPayrollService;
        private readonly IPaymentProcessRepository paymentProcessRepository;
        private readonly IObjectStream fileStream;
        private readonly ITemplateReader templateReader;

        public PaymentProcessService(IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            IMonthlyPayrollService monthlyPayrollService,
            ITemplateReader templateReader,
            IPaymentProcessRepository paymentProcessRepository = null,
            IObjectStream fileStream = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.monthlyPayrollService = monthlyPayrollService;
            this.templateReader = templateReader;
            this.paymentProcessRepository = paymentProcessRepository ?? new PaymentProcessRepository();
            this.fileStream = fileStream ?? new ObjectStream();
        }

        public async Task<SelectList> GetCompanySelectListAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetCompaniesForPaymentProcessCriteria { UserId = sessionService.UserId }).ConfigureAwait(false);

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        public async Task<MemoryStream> CopyFileToMemoryStreamAsync(string filePath)
        {
            var memory = new MemoryStream();
            using (var stream = fileStream.CreateInstance(filePath))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }

            memory.Position = 0;

            return memory;
        }

        public async Task<bool> IsPayrollGeneratedAsync(PaymentProcess process)
        {
            var isExisted = await baseService.GetDataEntityAsync<bool>(new CheckPayrollGeneratedCriteria { CompanyId = process.CompanyId, Month = process.Month }).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<PaymentProcess> GetPaymentProcessAsync(int companyId, DateTime month)
        {
            var criteria = new GetPaymentProcessCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var process = await baseService.GetDataEntityAsync<PaymentProcess>(criteria).ConfigureAwait(false);

            return process;
        }

        public CommandResult CheckValidExcelFile(IFormFile file)
        {
            var result = new CommandResult { Code = 0 };

            if (file != null)
            {
                var fileExt = Path.GetExtension(file.FileName);
                if (file.Length / (1024 * 1024) > 5)
                {
                    result.Code = 1;
                    result.Message = Message.FileTooLarge;
                }
                else if (!(fileExt.Equals(".xls") || fileExt.Equals(".xlsx")))
                {
                    result.Code = 1;
                    result.Message = Message.InvalidFileType;
                }
                else
                {
                    result.Code = 0;
                    result.Message = string.Empty;
                }
            }

            return result;
        }

        public async Task<CommandResult> UploadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var commandResult = new CommandResult { Code = 0 };
            if (viewModel.AttendanceFile != null)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.AttendanceFile.OpenReadStream());
                var template = new AttendanceTemplate
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyCode = viewModel.CompanyCode,
                    Month = viewModel.SelectedTime,
                    FileName = viewModel.AttendanceFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList
                };

                var readResult = templateReader.ReadAttendance(template);
                if (readResult is Success<IEnumerable<AttendanceUploading>> success)
                {
                    template.Records = success.Value;
                    template.ValidationResults = templateReader.AttendanceValidationResults;
                    commandResult = await CreateAttendanceUploadingAsync(template).ConfigureAwait(false);

                    return commandResult;
                }

                var failure = (Failure<IEnumerable<AttendanceUploading>>)readResult;
                commandResult.Code = 1;
                commandResult.Message = failure.Message;
            }

            return commandResult;
        }

        public async Task<CommandResult> UploadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            var commandResult = new CommandResult { Code = 0 };
            if (viewModel.DSTNetSalaryFile != null)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var currencies = await GetAllActiveCurrencyAsync().ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.DSTNetSalaryFile.OpenReadStream());
                var template = new DSTNetSalaryTemplate
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyCode = viewModel.CompanyCode,
                    Month = viewModel.SelectedTime,
                    FileName = viewModel.DSTNetSalaryFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList,
                    Currencies = currencies
                };

                var readResult = templateReader.ReadDSTNetSalary(template);
                if (readResult is Success<IEnumerable<DSTNetSalaryUploading>> success)
                {
                    template.Records = success.Value;
                    template.ValidationResults = templateReader.DSTNetSalaryValidationResults;
                    commandResult = await CreateDSTNetSalaryUploadingAsync(template).ConfigureAwait(false);

                    return commandResult;
                }

                var failure = (Failure<IEnumerable<DSTNetSalaryUploading>>)readResult;
                commandResult.Code = 1;
                commandResult.Message = failure.Message;
            }

            return commandResult;
        }

        public async Task<UploadAttendanceAndSalaryViewModel> GetUploadingFilesResultAsync(int companyId, DateTime month)
        {
            var criteria = new GetUploadingFilesResultCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            UploadAttendanceAndSalaryViewModel result = null;
            if (criteria.IsValid())
            {
                result = await paymentProcessRepository.GetUploadingFilesResultAsync(criteria).ConfigureAwait(false);
            }

            return result ?? new UploadAttendanceAndSalaryViewModel();
        }

        private async Task<IEnumerable<string>> GetAllEmployeeCodeByCompanyIdAsync(int companyId)
        {
            var employeeCodeList = await baseService.GetDataAsync<string>(new GetAllEmployeeCodeByCompanyIdCriteria { CompanyId = companyId }).ConfigureAwait(false);

            return employeeCodeList;
        }

        private async Task<IEnumerable<Currency>> GetAllActiveCurrencyAsync()
        {
            var currencies = await baseService.GetDataAsync<Currency>(new GetAllCurrencyCriteria { IsActive = true }).ConfigureAwait(false);

            return currencies;
        }

        private async Task<CommandResult> CreateAttendanceUploadingAsync(AttendanceTemplate template)
        {
            var criteria = new CreateAttendanceUploadingCriteria
            {
                SessionId = Guid.NewGuid().ToString(),
                CompanyId = template.CompanyId,
                Month = template.Month,
                FileName = template.FileName,
                UploadingXMLDoc = template.Records.ToXML().ToString(),
                ResultXMLDoc = template.ValidationResults.ToXML().ToString(),
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> CreateDSTNetSalaryUploadingAsync(DSTNetSalaryTemplate template)
        {
            var criteria = new CreateDSTNetSalaryUploadingCriteria
            {
                SessionId = Guid.NewGuid().ToString(),
                CompanyId = template.CompanyId,
                Month = template.Month,
                FileName = template.FileName,
                UploadingXMLDoc = template.Records.ToXML().ToString(),
                ResultXMLDoc = template.ValidationResults.ToXML().ToString(),
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        #region GenerateMonthlyPayroll

        public async Task<CommandResult> GenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId)
        {
            var dataModel = await GetDataForGenerateMonthlyPayroll(companyId, month, employeeId).ConfigureAwait(false);
            var numMonth = month.Month;
            var numYear = month.Year;
            var monthlySalaryList = new List<MonthlySalaryModel>();
            var monthlySalaryAdditionList = new List<MonthlySalaryAdditionModel>();
            var monthlyAllowanceList = new List<MonthlyAllowanceModel>();
            var monthlyAllowanceAdditionList = new List<MonthlyAllowanceAdditionModel>();
            var monthlyAllowanceDeductionList = new List<MonthlyAllowanceDeductionModel>();

            foreach (var payroll in dataModel.MonthlyPayroll)
            {
                //// Calculate Salary
                var monthlyBasicSalary = dataModel.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                if (monthlyBasicSalary != null)
                {
                    var backPay = new MonthlySalaryAdditionModel { EmployeeId = payroll.EmployeeId, Type = (int)MonthlySalaryAdditionType.BackPay };
                    var monthlySalary = new MonthlySalaryModel { EmployeeId = payroll.EmployeeId };
                    DecryptMonthlyBasicSalary(monthlyBasicSalary);

                    monthlyPayrollService.CalculateBackPayAndAfterAdjustment(dataModel.CurrencyExchangeRate, month, monthlyBasicSalary, backPay, payroll.GroupStartDate, payroll.GroupEndDate);
                    monthlyPayrollService.CalculateMonthlySalary(dataModel.CurrencyExchangeRate, monthlyBasicSalary, backPay, monthlySalary);

                    EncryptMonthlyBasicSalary(monthlyBasicSalary);
                    EncryptMonthlySalary(monthlySalary);
                    EncryptMonthlySalaryAddition(backPay);
                    monthlySalaryList.Add(monthlySalary);
                    monthlySalaryAdditionList.Add(backPay);

                    //// Calculate Allowance
                    var monthlyBasicAllowance = dataModel.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var monthlyAttendance = dataModel.MonthlyAttendance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var monthlyDSTNetSalary = dataModel.MonthlyDSTNetSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var actualWorkingDay = monthlyAttendance?.ActualWorkingDay ?? monthlyBasicSalary.WorkingDays;
                    if (monthlyDSTNetSalary != null)
                    {
                        DecryptMonthlyDSTNetSalary(monthlyDSTNetSalary);
                    }
                    DecryptMonthlyBasicAllowance(monthlyBasicAllowance);

                    monthlyPayrollService.CalculateMonthlyBasicAllowance(dataModel.CurrencyExchangeRate, monthlyBasicAllowance, payroll.GroupStartDate, payroll.GroupEndDate, numMonth, numYear);
                    var allowanceAdditions = monthlyPayrollService.InitializeAllowanceAdditions(monthlyBasicSalary.WorkingDays,
                        actualWorkingDay, monthlyBasicAllowance, dataModel.CurrencyExchangeRate);
                    var allowanceDeductions = monthlyPayrollService.InitializeAllowanceDeductions(monthlyBasicSalary.WorkingDays,
                        actualWorkingDay, monthlyBasicAllowance, dataModel.CurrencyExchangeRate);
                    var allowance = monthlyPayrollService.InitializeMonthlyAllowance(monthlyBasicAllowance, allowanceAdditions, allowanceDeductions,
                        dataModel.CurrencyExchangeRate, monthlyDSTNetSalary);

                    EncryptMonthlyBasicAllowance(monthlyBasicAllowance);
                    EncryptMonthlyAllowance(allowance);
                    EncryptMonthlyAllowanceAdditions(allowanceAdditions);
                    EncryptMonthlyAllowanceDeductions(allowanceDeductions);
                    monthlyAllowanceList.Add(allowance);
                    monthlyAllowanceAdditionList.AddRange(allowanceAdditions);
                    monthlyAllowanceDeductionList.AddRange(allowanceDeductions);
                }
            }

            var criteria = new GenerateMonthlyPayrollCriteria
            {
                CompanyId = companyId,
                Month = month,
                PayrollXMLDoc = dataModel.MonthlyPayroll.ToXML().ToString(),
                BasicSalaryXMLDoc = dataModel.MonthlyBasicSalary.ToXML().ToString(),
                SalaryXMLDoc = monthlySalaryList.ToXML().ToString(),
                SalaryAdditionXMLDoc = monthlySalaryAdditionList.ToXML().ToString(),
                BasicAllowanceXMLDoc = dataModel.MonthlyBasicAllowance.ToXML().ToString(),
                AllowanceXMLDoc = monthlyAllowanceList.ToXML().ToString(),
                AllowanceAdditionXMLDoc = monthlyAllowanceAdditionList.ToXML().ToString(),
                AllowanceDeductionXMLDoc = monthlyAllowanceDeductionList.ToXML().ToString(),
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> SubmitUploadingFilesAsync(int companyId, DateTime month)
        {
            var criteria = new SubmitUploadingFilesCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        private async Task<GeneratePayrollViewModel> GetDataForGenerateMonthlyPayroll(int companyId, DateTime month, int? employeeId = null)
        {
            var criteria = new GetDataForGeneratePayrollCriteria
            {
                CompanyId = companyId,
                Month = month,
                EmployeeId = employeeId
            };

            if (criteria.IsValid())
            {
                var result = await paymentProcessRepository.GetDataForGeneratePayroll(criteria).ConfigureAwait(false);
                return result;
            }

            return null;
        }

        private void DecryptMonthlyBasicSalary(MonthlyBasicSalaryModel basicSalary)
        {
            basicSalary.Salary = cryptoService.Decrypt(basicSalary.Salary);
            basicSalary.OldSalary = cryptoService.Decrypt(basicSalary.OldSalary);
            basicSalary.EbankingAmount = cryptoService.Decrypt(basicSalary.EbankingAmount);
            basicSalary.DirectBanking1Amount = cryptoService.Decrypt(basicSalary.DirectBanking1Amount);
            basicSalary.DirectBanking2Amount = cryptoService.Decrypt(basicSalary.DirectBanking2Amount);
            basicSalary.CashAmount = cryptoService.Decrypt(basicSalary.CashAmount);
            basicSalary.AfterAdjustmentAmount = cryptoService.Decrypt(basicSalary.AfterAdjustmentAmount);
        }

        private void EncryptMonthlyBasicSalary(MonthlyBasicSalaryModel basicSalary)
        {
            basicSalary.Salary = cryptoService.Encrypt(basicSalary.Salary);
            basicSalary.OldSalary = cryptoService.Encrypt(basicSalary.OldSalary);
            basicSalary.EbankingAmount = cryptoService.Encrypt(basicSalary.EbankingAmount);
            basicSalary.DirectBanking1Amount = cryptoService.Encrypt(basicSalary.DirectBanking1Amount);
            basicSalary.DirectBanking2Amount = cryptoService.Encrypt(basicSalary.DirectBanking2Amount);
            basicSalary.CashAmount = cryptoService.Encrypt(basicSalary.CashAmount);
            basicSalary.AfterAdjustmentAmount = cryptoService.Encrypt(basicSalary.AfterAdjustmentAmount);
        }

        private void EncryptMonthlySalary(MonthlySalaryModel salary)
        {
            salary.Salary = cryptoService.Encrypt(salary.Salary);
            salary.EbankingAmount = cryptoService.Encrypt(salary.EbankingAmount);
            salary.DirectBanking1Amount = cryptoService.Encrypt(salary.DirectBanking1Amount);
            salary.DirectBanking2Amount = cryptoService.Encrypt(salary.DirectBanking2Amount);
            salary.CashAmount = cryptoService.Encrypt(salary.CashAmount);
            salary.AdditionAmount = cryptoService.Encrypt(salary.AdditionAmount);
            salary.DeductionAmount = cryptoService.Encrypt(salary.DeductionAmount);
        }

        private void EncryptMonthlySalaryAddition(MonthlySalaryAdditionModel addition)
        {
            addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
            addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
        }

        /// <summary>
        /// Allowance Encrypt/Decrypt
        /// </summary>
        /// <param name="allowance"></param>
        private void DecryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
            allowance.SubsistenceAmount = cryptoService.Decrypt(allowance.SubsistenceAmount);
            allowance.HomePassageAmount = cryptoService.Decrypt(allowance.HomePassageAmount);
            allowance.HousingAmount = cryptoService.Decrypt(allowance.HousingAmount);
            allowance.MobilePhoneAmount = cryptoService.Decrypt(allowance.MobilePhoneAmount);
            allowance.HPIAmount = cryptoService.Decrypt(allowance.HPIAmount);
            allowance.DutyMealAmount = cryptoService.Decrypt(allowance.DutyMealAmount);
            allowance.SecondmentAmount = cryptoService.Decrypt(allowance.SecondmentAmount);
            allowance.OthersAmount = cryptoService.Decrypt(allowance.OthersAmount);
            allowance.RoomingChargesAmount = cryptoService.Decrypt(allowance.RoomingChargesAmount);
            allowance.FitnessFirstAmount = cryptoService.Decrypt(allowance.FitnessFirstAmount);
            allowance.AfterAdjustmentAmount = cryptoService.Decrypt(allowance.AfterAdjustmentAmount);
        }

        private void DecryptMonthlyDSTNetSalary(MonthlyDSTNetSalary salary)
        {
            salary.NetSalaryReceived = cryptoService.Decrypt(salary.NetSalaryReceived);
        }

        private void EncryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            basicAllowance.TotalAmount = cryptoService.Encrypt(basicAllowance.TotalAmount);
            basicAllowance.DirectBanking1Amount = cryptoService.Encrypt(basicAllowance.DirectBanking1Amount);
            basicAllowance.DirectBanking2Amount = cryptoService.Encrypt(basicAllowance.DirectBanking2Amount);
            basicAllowance.CashAmount = cryptoService.Encrypt(basicAllowance.CashAmount);
            basicAllowance.SubsistenceAmount = cryptoService.Encrypt(basicAllowance.SubsistenceAmount);
            basicAllowance.HomePassageAmount = cryptoService.Encrypt(basicAllowance.HomePassageAmount);
            basicAllowance.HousingAmount = cryptoService.Encrypt(basicAllowance.HousingAmount);
            basicAllowance.MobilePhoneAmount = cryptoService.Encrypt(basicAllowance.MobilePhoneAmount);
            basicAllowance.HPIAmount = cryptoService.Encrypt(basicAllowance.HPIAmount);
            basicAllowance.DutyMealAmount = cryptoService.Encrypt(basicAllowance.DutyMealAmount);
            basicAllowance.SecondmentAmount = cryptoService.Encrypt(basicAllowance.SecondmentAmount);
            basicAllowance.OthersAmount = cryptoService.Encrypt(basicAllowance.OthersAmount);
            basicAllowance.RoomingChargesAmount = cryptoService.Encrypt(basicAllowance.RoomingChargesAmount);
            basicAllowance.FitnessFirstAmount = cryptoService.Encrypt(basicAllowance.FitnessFirstAmount);
            basicAllowance.AfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.AfterAdjustmentAmount);
        }

        private void EncryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Encrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Encrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Encrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Encrypt(allowance.CashAmount);
            allowance.DSTNetSalaryAmount = cryptoService.Encrypt(allowance.DSTNetSalaryAmount);
            allowance.TotalNetAmount = cryptoService.Encrypt(allowance.TotalNetAmount);
        }

        private void EncryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.NetAllowance = cryptoService.Encrypt(addition.NetAllowance);
                addition.TotalAmount = cryptoService.Encrypt(addition.TotalAmount);
            }
        }

        private void EncryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.NetAllowance = cryptoService.Encrypt(deduction.NetAllowance);
                deduction.TotalAmount = cryptoService.Encrypt(deduction.TotalAmount);
            }
        }

        #endregion GenerateMonthlyPayroll
    }
}