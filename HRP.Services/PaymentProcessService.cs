namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
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
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = sessionService.UserId,
                FunctionKey = FunctionKey.PaymentProcess
            };
            var companies = await baseService.GetDataAsync<Company>(criteria).ConfigureAwait(false);

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        public async Task<PaymentProcess> GetCurrentDuration()
        {
            var criteria = new GetCurrentPaymentProcessDurationCriteria();

            var result = await baseService.GetDataEntityAsync<PaymentProcess>(criteria).ConfigureAwait(false);

            return result;
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

        private CommandResult CheckValidExcelFile(IFormFile file)
        {
            var fileExt = Path.GetExtension(file.FileName);
            if (file.Length / (1024 * 1024) > 5)
            {
                return new CommandResult
                {
                    Code = 1,
                    Message = Message.FileTooLarge
                };
            }
            else if (!(fileExt.Equals(".xls") || fileExt.Equals(".xlsx")))
            {
                return new CommandResult
                {
                    Code = 1,
                    Message = Message.InvalidFileType
                };
            }
            else
            {
                return new CommandResult
                {
                    Code = 0,
                    Message = string.Empty
                };
            }
        }

        public async Task<Result<AttendanceTemplate>> ReadAttendanceFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            if (viewModel.AttendanceFile == null)
            {
                return new Success<AttendanceTemplate>(null);
            }

            var checkValidFile = CheckValidExcelFile(viewModel.AttendanceFile);
            if (checkValidFile.IsSuccessful)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.AttendanceFile.OpenReadStream());
                var template = new AttendanceTemplate
                {
                    CompanyCode = viewModel.CompanyCode,
                    FileName = viewModel.AttendanceFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList
                };

                var readResult = templateReader.ReadAttendance(template);

                return readResult;
            }

            return new Failure<AttendanceTemplate>(checkValidFile.Message);
        }

        public async Task<Result<DSTNetSalaryTemplate>> ReadDSTNetSalaryFileAsync(UploadAttendanceAndSalaryViewModel viewModel)
        {
            if (viewModel.DSTNetSalaryFile == null)
            {
                return new Success<DSTNetSalaryTemplate>(null);
            }

            var checkValidFile = CheckValidExcelFile(viewModel.DSTNetSalaryFile);
            if (checkValidFile.IsSuccessful)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId).ConfigureAwait(false);
                var currencies = await GetAllActiveCurrencyAsync().ConfigureAwait(false);
                var sheet = templateReader.CreateSheet(viewModel.DSTNetSalaryFile.OpenReadStream());
                var template = new DSTNetSalaryTemplate
                {
                    CompanyCode = viewModel.CompanyCode,
                    FileName = viewModel.DSTNetSalaryFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList,
                    Currencies = currencies
                };

                var readResult = templateReader.ReadDSTNetSalary(template);

                return readResult;
            }

            return new Failure<DSTNetSalaryTemplate>(checkValidFile.Message);
        }

        public async Task<CommandResult> UploadAttendanceAndDSTNetSalaryAsync(int companyId, DateTime month, AttendanceTemplate attendanceTemplate, DSTNetSalaryTemplate dstNetSalaryTemplate)
        {
            var criteria = new UploadAttendanceAndDSTNetSalaryCriteria
            {
                CompanyId = companyId,
                Month = month,
                CreatedBy = sessionService.UserId
            };

            if (attendanceTemplate != null)
            {
                criteria.AttendanceSessionId = Guid.NewGuid().ToString();
                criteria.AttendanceFileName = attendanceTemplate.FileName;
                criteria.AttendanceUploadingXMLDoc = attendanceTemplate.Records.ToXML().ToString();
                criteria.AttendanceResultXMLDoc = attendanceTemplate.ValidationResults.ToXML().ToString();
            }

            if (dstNetSalaryTemplate != null)
            {
                criteria.DSTNetSalarySessionId = Guid.NewGuid().ToString();
                criteria.DSTNetSalaryFileName = dstNetSalaryTemplate.FileName;
                criteria.DSTNetSalaryUploadingXMLDoc = dstNetSalaryTemplate.Records.ToXML().ToString();
                criteria.DSTNetSalaryResultXMLDoc = dstNetSalaryTemplate.ValidationResults.ToXML().ToString();
            }

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

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

        public async Task<CommandResult> DeleteUploadedAttendance(int companyId, DateTime month)
        {
            var criteria = new DeleteUploadedAttendanceCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> DeleteUploadedDSTNetSalary(int companyId, DateTime month)
        {
            var criteria = new DeleteUploadedDSTNetSalaryCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
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

        #region GenerateMonthlyPayroll

        public async Task<CommandResult> GenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId)
        {
            var dataModel = await GetDataForGenerateMonthlyPayroll(companyId, month, employeeId).ConfigureAwait(false); ////Check if dataModel null
            var numMonth = month.Month;
            var numYear = month.Year;
            var monthlySalaries = new List<MonthlySalaryModel>();
            var monthlySalaryAdditions = new List<MonthlySalaryAdditionModel>();
            var monthlySalaryDeductions = new List<MonthlySalaryDeductionModel>();
            var monthlyAllowances = new List<MonthlyAllowanceModel>();
            var monthlyAllowanceAdditions = new List<MonthlyAllowanceAdditionModel>();
            var monthlyAllowanceDeductions = new List<MonthlyAllowanceDeductionModel>();

            foreach (var payroll in dataModel.MonthlyPayroll)
            {
                var monthlyBasicSalary = dataModel.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                if (monthlyBasicSalary != null)
                {
                    //// Calculate Salary
                    var salaryAdditions = Enum.GetValues(typeof(MonthlySalaryAdditionType)).Cast<MonthlySalaryAdditionType>()
                        .Where(x => (int)x != (int)MonthlySalaryAdditionType.BackPay).Select(x => new MonthlySalaryAdditionModel
                        {
                            EmployeeId = payroll.EmployeeId,
                            Type = (int)x
                        });
                    var salaryDeductions = Enum.GetValues(typeof(MonthlySalaryDeductionType)).Cast<MonthlySalaryDeductionType>()
                        .Select(x => new MonthlySalaryDeductionModel
                        {
                            EmployeeId = payroll.EmployeeId,
                            Type = (int)x
                        });
                    var backPay = new MonthlySalaryAdditionModel { EmployeeId = payroll.EmployeeId, Type = (int)MonthlySalaryAdditionType.BackPay };
                    var monthlySalary = new MonthlySalaryModel { EmployeeId = payroll.EmployeeId };
                    DecryptMonthlyBasicSalary(monthlyBasicSalary);

                    monthlyPayrollService.CalculateBackPayAndAfterAdjustment(dataModel.CurrencyExchangeRate, month, monthlyBasicSalary, backPay, payroll.GroupStartDate, payroll.GroupEndDate);
                    monthlyPayrollService.CalculateMonthlySalary(dataModel.CurrencyExchangeRate, monthlyBasicSalary, backPay, monthlySalary);

                    EncryptMonthlyBasicSalary(monthlyBasicSalary);
                    EncryptMonthlySalary(monthlySalary);
                    EncryptMonthlySalaryAddition(backPay);
                    monthlySalaries.Add(monthlySalary);
                    monthlySalaryAdditions.AddRange(salaryAdditions);
                    monthlySalaryAdditions.Add(backPay);
                    monthlySalaryDeductions.AddRange(salaryDeductions);

                    //// Calculate Allowance
                    var monthlyBasicAllowance = dataModel.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var monthlyAttendance = dataModel.MonthlyAttendance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var monthlyDSTNetSalary = dataModel.MonthlyDSTNetSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                    var actualWorkingDay = monthlyAttendance?.ActualWorkingDay ?? monthlyBasicSalary.WorkingDays;
                    DecryptMonthlyDSTNetSalary(monthlyDSTNetSalary);
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
                    monthlyAllowances.Add(allowance);
                    monthlyAllowanceAdditions.AddRange(allowanceAdditions);
                    monthlyAllowanceDeductions.AddRange(allowanceDeductions);
                }
            }

            var criteria = new GenerateMonthlyPayrollCriteria
            {
                CompanyId = companyId,
                Month = month,
                UpdatedBy = sessionService.UserId
            };

            criteria.PayrollXMLDoc = dataModel.MonthlyPayroll.ToXML().ToString();
            criteria.BasicSalaryXMLDoc = dataModel.MonthlyBasicSalary.ToXML().ToString();
            criteria.SalaryXMLDoc = monthlySalaries.ToXML().ToString();
            criteria.SalaryAdditionXMLDoc = monthlySalaryAdditions.ToXML().ToString();
            criteria.SalaryDeductionXMLDoc = monthlySalaryDeductions.ToXML().ToString();
            criteria.BasicAllowanceXMLDoc = dataModel.MonthlyBasicAllowance.ToXML().ToString();
            criteria.AllowanceXMLDoc = monthlyAllowances.ToXML().ToString();
            criteria.AllowanceAdditionXMLDoc = monthlyAllowanceAdditions.ToXML().ToString();
            criteria.AllowanceDeductionXMLDoc = monthlyAllowanceDeductions.ToXML().ToString();

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
            if (basicSalary == null)
            {
                return;
            }

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
            if (basicSalary == null)
            {
                return;
            }

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
            if (salary == null)
            {
                return;
            }

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
            if (addition == null)
            {
                return;
            }

            addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
            addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
        }

        /// <summary>
        /// Allowance Encrypt/Decrypt
        /// </summary>
        /// <param name="allowance"></param>
        private void DecryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel allowance)
        {
            if (allowance == null)
            {
                return;
            }

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
            if (salary == null)
            {
                return;
            }

            salary.NetSalaryReceived = cryptoService.Decrypt(salary.NetSalaryReceived);
        }

        private void EncryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            if (basicAllowance == null)
            {
                return;
            }

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
            if (allowance == null)
            {
                return;
            }

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