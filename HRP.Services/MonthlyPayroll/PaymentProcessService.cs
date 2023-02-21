namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Criteria.PaymentProcess;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    public class PaymentProcessService : IPaymentProcessService
    {
        private const int MultipleFileNumber = 2;
        private const int Coefficient = 1024;
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly ICommonService commonService;
        private readonly IMonthlySalaryService monthlySalaryService;
        private readonly IMonthlyAllowancesService monthlyAllowancesService;
        private readonly IPaymentProcessRepository paymentProcessRepository;
        private readonly IObjectStream fileStream;
        private readonly ITemplateReader templateReader;
        private readonly IAdditionDeductionTemplateReader additionDeductionTemplateReader;

#pragma warning disable S107 // Methods should not have too many parameters
        public PaymentProcessService(IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            ICommonService commonService,
            IMonthlySalaryService monthlySalaryService,
            IMonthlyAllowancesService monthlyAllowancesService,
            ITemplateReader templateReader,
            IPaymentProcessRepository paymentProcessRepository,
            IObjectStream fileStream,
            IAdditionDeductionTemplateReader additionDeductionTemplateReader)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.commonService = commonService;
            this.monthlySalaryService = monthlySalaryService;
            this.monthlyAllowancesService = monthlyAllowancesService;
            this.templateReader = templateReader;
            this.paymentProcessRepository = paymentProcessRepository;
            this.fileStream = fileStream;
            this.additionDeductionTemplateReader = additionDeductionTemplateReader;
        }

        public async Task<PaymentProcess> GetCurrentDuration()
        {
            var criteria = new GetCurrentPaymentProcessDurationCriteria();
            return await baseService.GetDataEntityAsync<PaymentProcess>(criteria);
        }

        public async Task<MemoryStream> CopyFileToMemoryStreamAsync(string filePath)
        {
            var memory = new MemoryStream();
            using (var stream = fileStream.CreateInstance(filePath))
            {
                await stream.CopyToAsync(memory);
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
            return await baseService.GetDataEntityAsync<PaymentProcess>(criteria);
        }

        public async Task<Result<AttendanceTemplate>> ReadAttendanceFileAsync(UploadFilesViewModel viewModel)
        {
            if (viewModel.AttendanceFile == null)
            {
                return new Success<AttendanceTemplate>(null);
            }

            var result = CheckValidExcelFile(viewModel.AttendanceFile);
            if (result.IsSuccessful)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId);
                var sheet = templateReader.CreateSheet(viewModel.AttendanceFile.OpenReadStream());
                var template = new AttendanceTemplate
                {
                    CompanyCode = viewModel.CompanyCode,
                    FileName = viewModel.AttendanceFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList
                };
                return templateReader.ReadAttendance(template);
            }

            return new Failure<AttendanceTemplate>(result.Message);
        }

        public async Task<Result<DSTNetSalaryTemplate>> ReadDSTNetSalaryFileAsync(UploadFilesViewModel viewModel)
        {
            if (viewModel.DSTNetSalaryFile == null)
            {
                return new Success<DSTNetSalaryTemplate>(null);
            }

            var result = CheckValidExcelFile(viewModel.DSTNetSalaryFile);
            if (result.IsSuccessful)
            {
                var employeeCodeList = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId);
                IEnumerable<Currency> currencies = await commonService.GetCurrencyAsync();
                var sheet = templateReader.CreateSheet(viewModel.DSTNetSalaryFile.OpenReadStream());
                var template = new DSTNetSalaryTemplate
                {
                    CompanyCode = viewModel.CompanyCode,
                    FileName = viewModel.DSTNetSalaryFile.FileName,
                    Sheet = sheet,
                    EmployeeCodeList = employeeCodeList,
                    Currencies = currencies
                };
                return templateReader.ReadDSTNetSalary(template);
            }

            return new Failure<DSTNetSalaryTemplate>(result.Message);
        }

        public async Task<Result<AdditionDeductionTemplate>> ReadAdditionDeductionFileAsync(UploadFilesViewModel viewModel)
        {
            if (viewModel.AdditionDeductionFile == null)
            {
                return new Success<AdditionDeductionTemplate>(null);
            }

            var result = CheckValidExcelFile(viewModel.AdditionDeductionFile);
            if (result.IsSuccessful)
            {
                var employeeCodes = await GetAllEmployeeCodeByCompanyIdAsync(viewModel.CompanyId);
                IEnumerable<Currency> currencies = await commonService.GetCurrencyAsync();
                var sheet = templateReader.CreateSheet(viewModel.AdditionDeductionFile.OpenReadStream());
                var template = new AdditionDeductionTemplate
                {
                    CompanyCode = viewModel.CompanyCode,
                    FileName = viewModel.AdditionDeductionFile.FileName,
                    Sheet = sheet,
                    EmployeeCodes = employeeCodes,
                    Currencies = currencies
                };
                return additionDeductionTemplateReader.ReadFile(template);
            }

            return new Failure<AdditionDeductionTemplate>(result.Message);
        }

        public async Task<CommandResult> UploadFilesAsync(
            int companyId,
            DateTime month,
            AttendanceTemplate attendanceTemplate,
            DSTNetSalaryTemplate dstNetSalaryTemplate,
            AdditionDeductionTemplate additionDeductionTemplate)
        {
            var criteria = new UploadFilesCommand
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

            if (additionDeductionTemplate != null)
            {
                criteria.AdditionDeductionSessionId = Guid.NewGuid().ToString();
                criteria.AdditionDeductionFileName = additionDeductionTemplate.FileName;
                criteria.AdditionDeductionUploadingXMLDoc = additionDeductionTemplate.Records.ToXML().ToString();
                criteria.AdditionDeductionResultXMLDoc = additionDeductionTemplate.ValidationResults.ToXML().ToString();
            }

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<UploadFilesViewModel> GetUploadingFilesResultAsync(int companyId, DateTime month)
        {
            var criteria = new GetUploadingFilesResultCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var result = new UploadFilesViewModel();
            if (criteria.IsValid())
            {
                (var paymentProcess, var attendanceErrors, var dstNetSalaryErrors, var additionDeductionErrors)
                    = await paymentProcessRepository.GetUploadingFilesResultAsync(criteria);
                if (paymentProcess != null)
                {
                    result.CompanyId = paymentProcess.CompanyId;
                    result.SelectedTime = paymentProcess.Month;
                    result.CompanyCode = paymentProcess.CompanyCode;
                    result.StartDate = paymentProcess.StartDate;
                    result.DueDate = paymentProcess.DueDate;

                    if (!string.IsNullOrWhiteSpace(paymentProcess.AttendanceFileName))
                    {
                        result.AttendanceFileName = paymentProcess.AttendanceFileName;
                        result.AttendanceResult = new FileResultViewModel
                        {
                            TotalRow = paymentProcess.AttendanceTotalRow,
                            ValidationErrors = attendanceErrors
                        };
                    }

                    if (!string.IsNullOrWhiteSpace(paymentProcess.DSTNetSalaryFileName))
                    {
                        result.DSTNetSalaryFileName = paymentProcess.DSTNetSalaryFileName;
                        result.DSTNetSalaryResult = new FileResultViewModel
                        {
                            TotalRow = paymentProcess.DSTNetSalaryTotalRow,
                            ValidationErrors = dstNetSalaryErrors
                        };
                    }

                    if (!string.IsNullOrWhiteSpace(paymentProcess.AdditionDeductionFileName))
                    {
                        result.AdditionDeductionFileName = paymentProcess.AdditionDeductionFileName;
                        result.AdditionDeductionResult = new FileResultViewModel
                        {
                            TotalRow = paymentProcess.AdditionDeductionTotalRow,
                            ValidationErrors = additionDeductionErrors
                        };
                    }
                }
            }

            return result;
        }

        public async Task<CommandResult> DeleteUploadedAttendance(int companyId, DateTime month)
        {
            var criteria = new DeleteUploadedAttendanceCriteria
            {
                CompanyId = companyId,
                Month = month
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteUploadedDSTNetSalary(int companyId, DateTime month)
        {
            var criteria = new DeleteUploadedDSTNetSalaryCriteria
            {
                CompanyId = companyId,
                Month = month
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> DeleteUploadedAdditionDeductionAsync(int companyId, DateTime month)
        {
            var criteria = new DeleteUploadedAdditionDeductionCommand
            {
                CompanyId = companyId,
                Month = month
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<IEnumerable<string>> GetAllEmployeeCodeByCompanyIdAsync(int companyId)
        {
            var criteria = new GetAllEmployeeCodeByCompanyIdCriteria { CompanyId = companyId };
            return await baseService.GetDataAsync<string>(criteria);
        }

        private CommandResult CheckValidExcelFile(IFormFile file)
        {
            var fileExt = Path.GetExtension(file.FileName);
            if (file.Length / (Coefficient * Coefficient) > MultipleFileNumber)
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

        #region GenerateMonthlyPayroll

        public async Task<CommandResult> GenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId = null)
        {
            GeneratePayrollModel dataModel = await GetDataForGenerateMonthlyPayrollAsync(companyId, month, employeeId);
            var monthlySalaries = new List<MonthlySalaryModel>();
            var monthlySalaryAdditions = new List<MonthlySalaryAdditionModel>();
            var monthlySalaryDeductions = new List<MonthlySalaryDeductionModel>();
            var monthlyAllowances = new List<MonthlyAllowanceModel>();
            var monthlyAllowanceAdditions = new List<MonthlyAllowanceAdditionModel>();
            var monthlyAllowanceDeductions = new List<MonthlyAllowanceDeductionModel>();

            foreach (MonthlyPayroll payroll in dataModel.MonthlyPayroll)
            {
                var monthlyBasicSalary = dataModel.MonthlyBasicSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
                if (monthlyBasicSalary != null)
                {
                    GenerateEmployeeBasicSalary(month, dataModel, monthlySalaries, monthlySalaryAdditions, monthlySalaryDeductions, payroll, monthlyBasicSalary);
                    GenerateEmployeeAllowance(month, dataModel, monthlyAllowances, monthlyAllowanceAdditions, monthlyAllowanceDeductions, payroll, monthlyBasicSalary);
                }
            }

            var criteria = new GenerateMonthlyPayrollCriteria
            {
                CompanyId = companyId,
                Month = month,
                UpdatedBy = sessionService.UserId,
                PayrollXMLDoc = dataModel.MonthlyPayroll.ToXML().ToString(),
                BasicSalaryXMLDoc = dataModel.MonthlyBasicSalary.ToXML().ToString(),
                SalaryXMLDoc = monthlySalaries.ToXML().ToString(),
                SalaryAdditionXMLDoc = monthlySalaryAdditions.ToXML().ToString(),
                SalaryDeductionXMLDoc = monthlySalaryDeductions.ToXML().ToString(),
                BasicAllowanceXMLDoc = dataModel.MonthlyBasicAllowance.ToXML().ToString(),
                AllowanceXMLDoc = monthlyAllowances.ToXML().ToString(),
                AllowanceAdditionXMLDoc = monthlyAllowanceAdditions.ToXML().ToString(),
                AllowanceDeductionXMLDoc = monthlyAllowanceDeductions.ToXML().ToString()
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private void GenerateEmployeeBasicSalary(
            DateTime month,
            GeneratePayrollModel dataModel,
            List<MonthlySalaryModel> monthlySalaries,
            List<MonthlySalaryAdditionModel> monthlySalaryAdditions,
            List<MonthlySalaryDeductionModel> monthlySalaryDeductions,
            MonthlyPayroll payroll,
            MonthlyBasicSalaryModel monthlyBasicSalary)
        {
            var backPay = new MonthlySalaryAdditionModel { EmployeeId = payroll.EmployeeId, Type = (int)MonthlySalaryAdditionType.BackPay };
            var monthlySalary = new MonthlySalaryModel { EmployeeId = payroll.EmployeeId };
            DecryptMonthlyBasicSalary(monthlyBasicSalary);

            var additionDeductionUpload = dataModel.AdditionDeductions.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
            var salaryAdditions = Enum.GetValues(typeof(MonthlySalaryAdditionType)).Cast<MonthlySalaryAdditionType>()
                .Where(x => (byte)x != (byte)MonthlySalaryAdditionType.BackPay).Select(x => new MonthlySalaryAdditionModel
                {
                    EmployeeId = payroll.EmployeeId,
                    Type = (byte)x
                }).ToList();
            var salaryDeductions = Enum.GetValues(typeof(MonthlySalaryDeductionType)).Cast<MonthlySalaryDeductionType>()
                .Select(x => new MonthlySalaryDeductionModel
                {
                    EmployeeId = payroll.EmployeeId,
                    Type = (byte)x
                }).ToList();
            if (additionDeductionUpload != null)
            {
                MappingMonthlySalaryAddition(salaryAdditions, additionDeductionUpload, monthlyBasicSalary.CurrencyCode);
                MappingMonthlySalaryDeduction(salaryDeductions, additionDeductionUpload, monthlyBasicSalary);
            }

            monthlySalaryService.CalculateBackPayAndAfterAdjustment(dataModel.CurrencyExchangeRate, month, monthlyBasicSalary, backPay, payroll.GroupStartDate, payroll.GroupEndDate);

            salaryAdditions.Add(backPay);

            monthlySalaryService.CalculateMonthlySalary(dataModel.CurrencyExchangeRate, monthlyBasicSalary, monthlySalary, salaryAdditions, salaryDeductions);

            EncryptMonthlyBasicSalary(monthlyBasicSalary);
            EncryptMonthlySalary(monthlySalary);
            EncryptMonthlySalaryAddition(salaryAdditions);
            EncryptMonthlySalaryDeduction(salaryDeductions);
            monthlySalaries.Add(monthlySalary);
            monthlySalaryAdditions.AddRange(salaryAdditions);
            monthlySalaryDeductions.AddRange(salaryDeductions);
        }

        private static void MappingMonthlySalaryAddition(
            List<MonthlySalaryAdditionModel> additions,
            AdditionDeductionUploading additionDeductionUpload,
            string currencyCode)
        {
            foreach(var addition in additions)
            {
                switch (addition.Type)
                {
                    case (byte)MonthlySalaryAdditionType.Adjustments:
                        MappingMonthlySalaryAddition(addition,
                            additionDeductionUpload.Salary_Addition_Adjustments_Amount,
                            additionDeductionUpload.Salary_Addition_Adjustments_NoOfDays,
                            currencyCode);
                        addition.Description = additionDeductionUpload.Salary_Addition_Adjustments_Comment;
                        break;

                    case (byte)MonthlySalaryAdditionType.Others1:
                        MappingMonthlySalaryAddition(addition,
                            additionDeductionUpload.Salary_Addition_Others1_Amount,
                            additionDeductionUpload.Salary_Addition_Others1_NoOfDays,
                            currencyCode);
                        addition.Description = additionDeductionUpload.Salary_Addition_Others1_Comment;
                        break;

                    case (byte)MonthlySalaryAdditionType.Others2:
                        MappingMonthlySalaryAddition(addition,
                            additionDeductionUpload.Salary_Addition_Others2_Amount,
                            additionDeductionUpload.Salary_Addition_Others2_NoOfDays,
                            currencyCode);
                        addition.Description = additionDeductionUpload.Salary_Addition_Others2_Comment;
                        break;

                    default:
                        // do nothing
                        break;
                }
            }
        }

        private static void MappingMonthlySalaryAddition(MonthlySalaryAdditionModel addition, string unitAmount, string noOfDays, string currencyCode)
        {
            decimal.TryParse(unitAmount, out decimal amount);
            decimal.TryParse(noOfDays, out decimal tempNoOfDays);
            amount = Math.Round(amount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            if (amount > 0)
            {
                amount = CalculationService.RoundAmountWithCurrency(amount, currencyCode);
                tempNoOfDays = Math.Round(tempNoOfDays, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
                var netAmount = CalculationService.RoundAmountWithCurrency(amount * (tempNoOfDays == 0 ? 1 : tempNoOfDays), currencyCode);
                addition.UnitAmount = amount.ToString(CultureInfo.InvariantCulture);
                addition.NoOfDays = tempNoOfDays == 0 ? (decimal?)null : tempNoOfDays;
                addition.NetSalary = netAmount.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void MappingMonthlySalaryDeduction(
            List<MonthlySalaryDeductionModel> deductions,
            AdditionDeductionUploading additionDeductionUpload,
            MonthlyBasicSalaryModel monthlyBasicSalary)
        {
            decimal amount;
            foreach (var deduction in deductions)
            {
                switch (deduction.Type)
                {
                    case (byte)MonthlySalaryDeductionType.Absenteeism:
                        MappingAbsenteeismDeduction(deduction, additionDeductionUpload, monthlyBasicSalary);
                        break;

                    case (byte)MonthlySalaryDeductionType.Tardiness:
                        MappingTardinessDeduction(deduction, additionDeductionUpload, monthlyBasicSalary);
                        break;

                    case (byte)MonthlySalaryDeductionType.SalaryAdvance:
                        decimal.TryParse(additionDeductionUpload.Salary_Deduction_Advance_Amount, out amount);
                        amount = Math.Round(amount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
                        amount = CalculationService.RoundAmountWithCurrency(amount, monthlyBasicSalary.CurrencyCode);

                        deduction.UnitAmount = amount > 0 ? amount.ToString(CultureInfo.InvariantCulture) : string.Empty;
                        deduction.NetSalary = deduction.UnitAmount;
                        deduction.Description = additionDeductionUpload.Salary_Deduction_Advance_Comment;
                        break;

                    case (byte)MonthlySalaryDeductionType.Others:
                        decimal.TryParse(additionDeductionUpload.Salary_Deduction_Others_Amount, out amount);
                        amount = Math.Round(amount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
                        amount = CalculationService.RoundAmountWithCurrency(amount, monthlyBasicSalary.CurrencyCode);

                        deduction.UnitAmount = amount > 0 ? amount.ToString(CultureInfo.InvariantCulture) : string.Empty;
                        deduction.NetSalary = deduction.UnitAmount;
                        deduction.Description = additionDeductionUpload.Salary_Deduction_Others_Comment;
                        break;

                    default:
                        // do nothing
                        break;
                }
            }
        }

        private static void MappingAbsenteeismDeduction(
            MonthlySalaryDeductionModel deduction,
            AdditionDeductionUploading dataUpload,
            MonthlyBasicSalaryModel monthlyBasicSalary)
        {
            decimal.TryParse(dataUpload.Salary_Deduction_Absenteeism_Days, out decimal days);
            days = Math.Round(days, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            var amount = monthlyBasicSalary.SalaryPerDay * days;
            amount = Math.Round(amount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            amount = CalculationService.RoundAmountWithCurrency(amount, monthlyBasicSalary.CurrencyCode);

            deduction.NoOfDays = days > 0 ? days : (decimal?)null;
            deduction.NetSalary = amount > 0 ? amount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.Description = dataUpload.Salary_Deduction_Absenteeism_Comment;
        }

        private static void MappingTardinessDeduction(
            MonthlySalaryDeductionModel deduction,
            AdditionDeductionUploading dataUpload,
            MonthlyBasicSalaryModel monthlyBasicSalary)
        {
            decimal.TryParse(dataUpload.Salary_Deduction_Tardiness_Minutes, out decimal minutes);
            minutes = Math.Round(minutes, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            var amount = (monthlyBasicSalary.SalaryPerDay / (8 * 60)) * minutes;
            amount = Math.Round(amount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            amount = CalculationService.RoundAmountWithCurrency(amount, monthlyBasicSalary.CurrencyCode);

            deduction.NoOfDays = minutes > 0 ? minutes : (decimal?)null;
            deduction.NetSalary = amount > 0 ? amount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.Description = dataUpload.Salary_Deduction_Tardiness_Comment;
        }

        private void GenerateEmployeeAllowance(
            DateTime month,
            GeneratePayrollModel dataModel,
            List<MonthlyAllowanceModel> monthlyAllowances,
            List<MonthlyAllowanceAdditionModel> monthlyAllowanceAdditions,
            List<MonthlyAllowanceDeductionModel> monthlyAllowanceDeductions,
            MonthlyPayroll payroll,
            MonthlyBasicSalaryModel monthlyBasicSalary)
        {
            var monthlyBasicAllowance = dataModel.MonthlyBasicAllowance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
            var monthlyAttendance = dataModel.MonthlyAttendance.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
            var monthlyDSTNetSalary = dataModel.MonthlyDSTNetSalary.FirstOrDefault(x => x.EmployeeId == payroll.EmployeeId);
            var actualWorkingDay = monthlyAttendance?.ActualWorkingDay ?? monthlyBasicSalary.WorkingDays;
            var additionDeductionUpload = dataModel.AdditionDeductions.FirstOrDefault(x => x.EmployeeId == monthlyBasicAllowance.EmployeeId);
            DecryptMonthlyDSTNetSalary(monthlyDSTNetSalary);
            DecryptMonthlyBasicAllowance(monthlyBasicAllowance);

            monthlyAllowancesService.CalculateMonthlyBasicAllowance(
                dataModel.CurrencyExchangeRate, monthlyBasicAllowance,
                month, payroll.GroupStartDate, payroll.GroupEndDate);
            var allowanceAdditions = monthlyAllowancesService.InitializeAllowanceAdditions(
                monthlyBasicSalary.WorkingDays, actualWorkingDay,
                monthlyBasicAllowance, dataModel);
            var allowanceDeductions = monthlyAllowancesService.InitializeAllowanceDeductions(
                monthlyBasicSalary.WorkingDays, actualWorkingDay,
                monthlyBasicAllowance,
                dataModel);
            MonthlyAllowanceModel allowance = monthlyAllowancesService.InitializeMonthlyAllowance(
                monthlyBasicAllowance, allowanceAdditions, allowanceDeductions,
                dataModel.CurrencyExchangeRate, monthlyDSTNetSalary);
            allowance.AdditionComment = additionDeductionUpload?.Allowance_Addition_Comment;
            allowance.DeductionComment = additionDeductionUpload?.Allowance_Deduction_Comment;

            EncryptMonthlyBasicAllowance(monthlyBasicAllowance);
            EncryptMonthlyAllowance(allowance);
            EncryptMonthlyAllowanceAdditions(allowanceAdditions);
            EncryptMonthlyAllowanceDeductions(allowanceDeductions);
            monthlyAllowances.Add(allowance);
            monthlyAllowanceAdditions.AddRange(allowanceAdditions);
            monthlyAllowanceDeductions.AddRange(allowanceDeductions);
        }

        public async Task<CommandResult> SubmitUploadingFilesAsync(int companyId, DateTime month)
        {
            var criteria = new SubmitUploadingFilesCriteria
            {
                CompanyId = companyId,
                Month = month
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private async Task<GeneratePayrollModel> GetDataForGenerateMonthlyPayrollAsync(int companyId, DateTime month, int? employeeId = null)
        {
            var criteria = new GetDataForGeneratePayrollCriteria
            {
                CompanyId = companyId,
                Month = month,
                EmployeeId = employeeId
            };
            GeneratePayrollModel data = await paymentProcessRepository.GetDataForGeneratePayrollAsync(criteria);
            data.Currencies = await commonService.GetCurrencyAsync();
            DescryptAdditionDeduction(data.AdditionDeductions);

            return data;
        }

        private void DescryptAdditionDeduction(IEnumerable<AdditionDeductionUploading> additionDeductions)
        {
            foreach(var item in additionDeductions)
            {
                item.Salary_Addition_Adjustments_Amount = cryptoService.Decrypt(item.Salary_Addition_Adjustments_Amount);
                item.Salary_Addition_Others1_Amount = cryptoService.Decrypt(item.Salary_Addition_Others1_Amount);
                item.Salary_Addition_Others2_Amount = cryptoService.Decrypt(item.Salary_Addition_Others2_Amount);
                item.Salary_Deduction_Advance_Amount = cryptoService.Decrypt(item.Salary_Deduction_Advance_Amount);
                item.Salary_Deduction_Others_Amount = cryptoService.Decrypt(item.Salary_Deduction_Others_Amount);

                item.Allowance_Addition_Subsistence_Amount = cryptoService.Decrypt(item.Allowance_Addition_Subsistence_Amount);
                item.Allowance_Addition_HomePassage_Amount = cryptoService.Decrypt(item.Allowance_Addition_HomePassage_Amount);
                item.Allowance_Addition_Housing_Amount = cryptoService.Decrypt(item.Allowance_Addition_Housing_Amount);
                item.Allowance_Addition_MobilePhone_Amount = cryptoService.Decrypt(item.Allowance_Addition_MobilePhone_Amount);
                item.Allowance_Addition_HPI_Amount = cryptoService.Decrypt(item.Allowance_Addition_HPI_Amount);
                item.Allowance_Addition_Secondment_Amount = cryptoService.Decrypt(item.Allowance_Addition_Secondment_Amount);
                item.Allowance_Addition_SpecialAllowance_Amount = cryptoService.Decrypt(item.Allowance_Addition_SpecialAllowance_Amount);
                item.Allowance_Addition_OT_Amount = cryptoService.Decrypt(item.Allowance_Addition_OT_Amount);
                item.Allowance_Addition_Others_Amount = cryptoService.Decrypt(item.Allowance_Addition_Others_Amount);

                item.Allowance_Deduction_Subsistence_Amount = cryptoService.Decrypt(item.Allowance_Deduction_Subsistence_Amount);
                item.Allowance_Deduction_HomePassage_Amount = cryptoService.Decrypt(item.Allowance_Deduction_HomePassage_Amount);
                item.Allowance_Deduction_Housing_Amount = cryptoService.Decrypt(item.Allowance_Deduction_Housing_Amount);
                item.Allowance_Deduction_MobilePhone_Amount = cryptoService.Decrypt(item.Allowance_Deduction_MobilePhone_Amount);
                item.Allowance_Deduction_HPI_Amount = cryptoService.Decrypt(item.Allowance_Deduction_HPI_Amount);
                item.Allowance_Deduction_Secondment_Amount = cryptoService.Decrypt(item.Allowance_Deduction_Secondment_Amount);
                item.Allowance_Deduction_Others_Amount = cryptoService.Decrypt(item.Allowance_Deduction_Others_Amount);
            }
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
            basicSalary.AfterAdjustmentReason = cryptoService.Encrypt(basicSalary.AfterAdjustmentReason);
        }

        private void EncryptMonthlySalary(MonthlySalaryModel salary)
        {
            if (salary != null)
            {
                salary.Salary = cryptoService.Encrypt(salary.Salary);
                salary.EbankingAmount = cryptoService.Encrypt(salary.EbankingAmount);
                salary.DirectBanking1Amount = cryptoService.Encrypt(salary.DirectBanking1Amount);
                salary.DirectBanking2Amount = cryptoService.Encrypt(salary.DirectBanking2Amount);
                salary.CashAmount = cryptoService.Encrypt(salary.CashAmount);
                salary.AdditionAmount = cryptoService.Encrypt(salary.AdditionAmount);
                salary.DeductionAmount = cryptoService.Encrypt(salary.DeductionAmount);
            }
        }

        private void EncryptMonthlySalaryAddition(List<MonthlySalaryAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
            }
        }

        private void EncryptMonthlySalaryDeduction(List<MonthlySalaryDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Encrypt(deduction.NetSalary);
            }
        }

        private void DecryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel allowance)
        {
            if (allowance != null)
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
        }

        private void DecryptMonthlyDSTNetSalary(MonthlyDSTNetSalary salary)
        {
            if (salary != null)
            {
                salary.NetSalaryReceived = cryptoService.Decrypt(salary.NetSalaryReceived);
            }
        }

        private void EncryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            if (basicAllowance != null)
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

                basicAllowance.DutyMealAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.DutyMealAfterAdjustmentAmount);
                basicAllowance.HomePassageAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.HomePassageAfterAdjustmentAmount);
                basicAllowance.HousingAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.HousingAfterAdjustmentAmount);
                basicAllowance.HPIAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.HPIAfterAdjustmentAmount);
                basicAllowance.MobilePhoneAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.MobilePhoneAfterAdjustmentAmount);
                basicAllowance.OthersAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.OthersAfterAdjustmentAmount);
                basicAllowance.SecondmentAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.SecondmentAfterAdjustmentAmount);
                basicAllowance.SubsistenceAfterAdjustmentAmount = cryptoService.Encrypt(basicAllowance.SubsistenceAfterAdjustmentAmount);
            }
        }

        private void EncryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            if (allowance != null)
            {
                allowance.TotalAmount = cryptoService.Encrypt(allowance.TotalAmount);
                allowance.DirectBanking1Amount = cryptoService.Encrypt(allowance.DirectBanking1Amount);
                allowance.DirectBanking2Amount = cryptoService.Encrypt(allowance.DirectBanking2Amount);
                allowance.CashAmount = cryptoService.Encrypt(allowance.CashAmount);
                allowance.DSTNetSalaryAmount = cryptoService.Encrypt(allowance.DSTNetSalaryAmount);
                allowance.TotalNetAmount = cryptoService.Encrypt(allowance.TotalNetAmount);
                allowance.AdditionAmount = cryptoService.Encrypt(allowance.AdditionAmount);
                allowance.DeductionAmount = cryptoService.Encrypt(allowance.DeductionAmount);
            }
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