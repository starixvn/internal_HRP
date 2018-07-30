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
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Payslip;
    using HRP.Models.MailSetting;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;
    using HRP.Resources;
    using MimeKit;

    public class PayslipService : IPayslipService
    {
        private const string EmailTemplate = @"wwwroot\\EmailTemplates\\Payslip.cshtml";
        private const string cssFilePath = @"assets\\bundles\\payslip\\css\\pdf.min.css";
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseMailService emailService;
        private readonly IViewRenderService viewRenderService;
        private readonly IPdfRenderService pdfRenderService;
        private readonly IPayslipRepository payslipRepository;
        private readonly IMailSettingService mailSettingService;

#pragma warning disable S107 // Methods should not have too many parameters

        public PayslipService(
            IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            IBaseMailService emailService,
            IViewRenderService viewRenderService,
            IPdfRenderService pdfRenderService,
            IMailSettingService mailSettingService,
            IPayslipRepository payslipRepository)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.emailService = emailService;
            this.viewRenderService = viewRenderService;
            this.pdfRenderService = pdfRenderService;
            this.mailSettingService = mailSettingService;
            this.payslipRepository = payslipRepository;
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

            PayslipDetailModel payslip = (await payslipRepository.GetPayslipDetailsAsync(criteria).ConfigureAwait(false)).FirstOrDefault();

            if (payslip != null)
            {
                DescryptAndCalculateAmount(payslip);
            }

            return payslip;
        }

        public async Task<CommandResult> SendPayslipAsync(SendPayslipViewModel viewModel)
        {
            var criteria = new GetPayslipDetailsCriteria
            {
                UserId = sessionService.UserId,
                PayrollIds = string.Join(",", viewModel.PayrollIds),
                CompanyId = viewModel.CompanyId
            };
            MailSettingModel mailSetting = await mailSettingService.GetMailSettingForSendMailAsync(viewModel.CompanyId).ConfigureAwait(false);

            if (mailSetting == null)
            {
                return new CommandResult
                {
                    Message = Message.MailSettingMissing
                };
            }

            IEnumerable<PayslipDetailModel> payslips = await payslipRepository.GetPayslipDetailsAsync(criteria).ConfigureAwait(false);

            foreach (PayslipDetailModel payslip in payslips)
            {
                DescryptAndCalculateAmount(payslip);
                await RenderPdfAndSendMailAsync(mailSetting, payslip).ConfigureAwait(false);
            }

            CommandResult commandResult = await UpdatePayslipAsync(viewModel).ConfigureAwait(false);

            return commandResult;
        }

        private async Task<CommandResult> UpdatePayslipAsync(SendPayslipViewModel viewModel)
        {
            var criteria = new UpdatePayslipCriteria
            {
                UserId = sessionService.UserId,
                PayrollIds = string.Join(",", viewModel.PayrollIds),
                CompanyId = viewModel.CompanyId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        private async Task RenderPdfAndSendMailAsync(MailSettingModel mailSetting, PayslipDetailModel payslip)
        {
            var body = string.Format(CultureInfo.InvariantCulture, emailService.ReadTextFile(EmailTemplate), payslip.Nickname);
            var subject = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}", payslip.Month.ToString("MMMM_yyyy"));
            var fileName = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}.pdf", payslip.Month.ToString("MMMM_yyyy"));
            var htmlString = await viewRenderService.RenderViewToStringAsync("Payslip/Pdf", payslip).ConfigureAwait(false);
            byte[] content = pdfRenderService.RenderContentWithPasswordProtected(cssFilePath, htmlString, payslip.PDFPassword, payslip.PDFPassword);

            using (var input = new MemoryStream(content))
            {
                var attachments = new AttachmentCollection
                {
                    { fileName, input }
                };

                var mailModel = new MailModel
                {
                    Server = mailSetting.Server,
                    Port = mailSetting.Port ?? 0,
                    Username = mailSetting.Username,
                    Password = mailSetting.Password,
                    IsAuth = mailSetting.IsAuth,
                    Priority = mailSetting.Priority,
                    Subject = subject,
                    ReceiverEmail = payslip.Email,
                    Body = body,
                    Attachments = attachments
                };

                await emailService.SendEmailAsync(mailModel).ConfigureAwait(false);
            }
        }

        private void DescryptAndCalculateAmount(PayslipDetailModel payslip)
        {
            payslip.PDFPassword = cryptoService.Decrypt(payslip.PDFPassword);

            DescryptAndCalculateMonthlySalary(payslip);
            DescryptAndCalculateMonthlyAllowance(payslip);
        }

        private void DescryptAndCalculateMonthlySalary(PayslipDetailModel payslip)
        {
            decimal amount;
            decimal additionAmount = 0;
            decimal deductionAmount = 0;
            payslip.SalaryAmount = cryptoService.Decrypt(payslip.SalaryAmount);

            decimal.TryParse(payslip.SalaryAmount, out decimal basicSalaryAmount);
            payslip.SalaryAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(basicSalaryAmount, payslip.SalaryCurrencyCode, roundingThousand: false),
                payslip.SalaryCurrencyCode);

            payslip.SalaryAdditions = payslip.SalaryAdditions.Any()
                ? payslip.SalaryAdditions : Enum.GetValues(typeof(MonthlySalaryAdditionType)).Cast<MonthlySalaryAdditionType>()
                                                .Select(x => new MonthlySalaryAdditionModel
                                                {
                                                    PayrollId = payslip.PayrollId,
                                                    Type = (int)x
                                                }).ToList();
            payslip.SalaryDeductions = payslip.SalaryDeductions.Any()
                ? payslip.SalaryDeductions : Enum.GetValues(typeof(MonthlySalaryDeductionType)).Cast<MonthlySalaryDeductionType>()
                                                .Select(x => new MonthlySalaryDeductionModel
                                                {
                                                    PayrollId = payslip.PayrollId,
                                                    Type = (int)x
                                                }).ToList();

            foreach (MonthlySalaryAdditionModel addition in payslip.SalaryAdditions)
            {
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                additionAmount += amount;
                addition.NetSalaryDisplay = CalculationService.FormatAmount(
                    CalculationService.RoundAmountWithCurrency(amount, payslip.SalaryCurrencyCode, roundingThousand: false),
                    payslip.SalaryCurrencyCode);
            }

            foreach (MonthlySalaryDeductionModel deduction in payslip.SalaryDeductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deductionAmount += amount;
                deduction.NetSalaryDisplay = CalculationService.FormatAmount(
                    CalculationService.RoundAmountWithCurrency(amount, payslip.SalaryCurrencyCode, roundingThousand: false),
                    payslip.SalaryCurrencyCode);
            }

            amount = basicSalaryAmount + additionAmount;
            payslip.GrossSalaryAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(amount, payslip.SalaryCurrencyCode, 0),
                Constants.VNDCurrencyCode);

            amount = basicSalaryAmount + additionAmount - deductionAmount;
            payslip.NetSalaryAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(amount, payslip.SalaryCurrencyCode, 0),
                Constants.VNDCurrencyCode);

            payslip.SalaryDeductionAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(deductionAmount, payslip.SalaryCurrencyCode, 0),
                Constants.VNDCurrencyCode);
        }

        private void DescryptAndCalculateMonthlyAllowance(PayslipDetailModel payslip)
        {
            decimal amount;
            decimal additionAmount = 0;
            decimal deductionAmount = 0;
            payslip.AllowanceAmount = cryptoService.Decrypt(payslip.AllowanceAmount);

            decimal.TryParse(payslip.AllowanceAmount, out decimal basicAllowanceAmount);
            payslip.AllowanceAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(basicAllowanceAmount, payslip.AllowanceCurrencyCode, roundingThousand: false),
                payslip.AllowanceCurrencyCode);

            payslip.AllowanceAdditions = payslip.AllowanceAdditions.Any()
                ? payslip.AllowanceAdditions : Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                                                .Select(x => new MonthlyAllowanceAdditionModel
                                                {
                                                    PayrollId = payslip.PayrollId,
                                                    Type = (int)x
                                                }).ToList();
            payslip.AllowanceDeductions = payslip.AllowanceDeductions.Any()
                ? payslip.AllowanceDeductions : Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                                                .Select(x => new MonthlyAllowanceDeductionModel
                                                {
                                                    PayrollId = payslip.PayrollId,
                                                    Type = (int)x
                                                }).ToList();

            foreach (var addition in payslip.AllowanceAdditions)
            {
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);

                decimal.TryParse(addition.NetAllowance, out amount);
                additionAmount += amount;
                addition.NetAllowanceToDisplay = CalculationService.FormatAmount(
                    CalculationService.RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyCode, roundingThousand: false),
                    payslip.AllowanceCurrencyCode);
            }

            foreach (MonthlyAllowanceDeductionModel deduction in payslip.AllowanceDeductions)
            {
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);

                decimal.TryParse(deduction.NetAllowance, out amount);
                deductionAmount += amount;
                deduction.NetAllowanceToDisplay = CalculationService.FormatAmount(
                    CalculationService.RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyCode, roundingThousand: false),
                    payslip.AllowanceCurrencyCode);
            }

            amount = basicAllowanceAmount + additionAmount;
            payslip.GrossAllowanceAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyCode, 0),
                Constants.VNDCurrencyCode);

            amount = basicAllowanceAmount + additionAmount - deductionAmount;
            payslip.NetAllowanceAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyCode, 0),
                Constants.VNDCurrencyCode);

            payslip.AllowanceDeductionAmount = CalculationService.FormatAmount(
                CalculationService.RoundAmountWithCurrency(deductionAmount, payslip.AllowanceCurrencyCode, 0),
                Constants.VNDCurrencyCode);
        }
    }
}