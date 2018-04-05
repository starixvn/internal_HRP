namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.MailDeliver.Model;
    using HRP.Core.Constants;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Payslip;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Repositories;
    using iTextSharp.text.pdf;

    public class PayslipService : IPayslipService
    {
        private const string EmailTemplate = @"wwwroot\\EmailTemplates\\Payslip.cshtml";
        private const string cssFilePath = @"assets\\styles\\pages\\payslip\\payslip-detail.css";
        private const string AmountFormat = "#,##0.00";
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseMailService emailService;
        private readonly IViewRenderService viewRenderService;
        private readonly IPdfRenderService pdfRenderService;
        private readonly IPayslipRepository payslipRepository;

        public PayslipService(
            IBaseService baseService,
            ISessionService sessionService,
            ICryptoService cryptoService,
            IBaseMailService emailService,
            IViewRenderService viewRenderService,
            IPdfRenderService pdfRenderService,
            IPayslipRepository payslipRepository = null)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.emailService = emailService;
            this.viewRenderService = viewRenderService;
            this.pdfRenderService = pdfRenderService;
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

            PayslipDetailModel payslip = (await payslipRepository.GetPayslipDetailsAsync(criteria).ConfigureAwait(false)).FirstOrDefault();

            if (payslip != null)
            {
                DescryptAndCalculateAmount(payslip);
            }

            return payslip;
        }

        public async Task<CommandResult> SendPayslipAsync(SendPayslipViewModel viewModel)
        {
            var commandResult = new CommandResult();
            var criteria = new GetPayslipDetailsCriteria
            {
                UserId = sessionService.UserId,
                PayrollIds = string.Join(",", viewModel.PayrollIds),
                CompanyId = viewModel.CompanyId
            };

            IEnumerable<PayslipDetailModel> payslips = await payslipRepository.GetPayslipDetailsAsync(criteria).ConfigureAwait(false);

            foreach (PayslipDetailModel payslip in payslips)
            {
                DescryptAndCalculateAmount(payslip);
                RenderPdfAndSendMail(payslip);
            }

            return commandResult;
        }

        private void RenderPdfAndSendMail(PayslipDetailModel payslip)
        {
            var body = emailService.ReadTextFile(EmailTemplate);
            body = string.Format(CultureInfo.InvariantCulture, body, payslip.Nickname);
            var subject = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}", payslip.Month.ToString("MMMM_yyyy"));

            var htmlString = viewRenderService.RenderViewToStringAsync("Payslip/Details", payslip).Result;
            byte[] streamContent = pdfRenderService.RenderContent(cssFilePath, htmlString);

            using (var input = new MemoryStream(streamContent))
            {
                using (var output = new MemoryStream())
                {
                    var reader = new PdfReader(input);
                    PdfEncryptor.Encrypt(reader, output, true, payslip.PDFPassword, payslip.PDFPassword, PdfWriter.ALLOW_SCREENREADERS);

                    var mailAttachments = new List<MailAttachment>();
                    var mailAttachment = new MailAttachment(output, FileExtentions.PDF)
                    {
                        FileName = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}.pdf", payslip.Month.ToString("MMMM_yyyy"))
                    };
                    mailAttachments.Add(mailAttachment);

                    var mailModel = new MailModel
                    {
                        Subject = subject,
                        ReceiverEmail = payslip.Email,
                        MailCC = string.Empty,
                        Body = body,
                        Attachments = mailAttachments
                    };

                    emailService.SendEmail(mailModel);
                }
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
            payslip.SalaryAmount = basicSalaryAmount.ToString(AmountFormat);

            foreach (MonthlySalaryAdditionModel addition in payslip.SalaryAdditions)
            {
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                additionAmount += amount;
                addition.NetSalaryDisplay = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(AmountFormat);
            }

            foreach (MonthlySalaryDeductionModel deduction in payslip.SalaryDeductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deductionAmount += amount;
                deduction.NetSalaryDisplay = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(AmountFormat);
            }

            amount = basicSalaryAmount + additionAmount;
            payslip.GrossSalaryAmount = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(AmountFormat);

            amount = basicSalaryAmount + additionAmount - deductionAmount;
            payslip.NetSalaryAmount = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(AmountFormat);

            payslip.SalaryDeductionAmount = RoundAmountWithCurrency(deductionAmount, payslip.SalaryCurrencyId).ToString(AmountFormat);
        }

        private void DescryptAndCalculateMonthlyAllowance(PayslipDetailModel payslip)
        {
            decimal amount;
            decimal additionAmount = 0;
            decimal deductionAmount = 0;
            payslip.AllowanceAmount = cryptoService.Decrypt(payslip.AllowanceAmount);

            decimal.TryParse(payslip.AllowanceAmount, out decimal basicAllowanceAmount);
            payslip.AllowanceAmount = basicAllowanceAmount.ToString(AmountFormat);

            foreach (MonthlyAllowanceAdditionModel addition in payslip.AllowanceAdditions)
            {
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);

                decimal.TryParse(addition.NetAllowance, out amount);
                additionAmount += amount;
                addition.NetAllowanceToDisplay = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(AmountFormat);
            }

            foreach (MonthlyAllowanceDeductionModel deduction in payslip.AllowanceDeductions)
            {
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);

                decimal.TryParse(deduction.NetAllowance, out amount);
                deductionAmount += amount;
                deduction.NetAllowanceToDisplay = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(AmountFormat);
            }

            amount = basicAllowanceAmount + additionAmount;
            payslip.GrossAllowanceAmount = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(AmountFormat);

            amount = basicAllowanceAmount + additionAmount - deductionAmount;
            payslip.NetAllowanceAmount = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(AmountFormat);

            payslip.AllowanceDeductionAmount = RoundAmountWithCurrency(deductionAmount, payslip.AllowanceCurrencyId).ToString(AmountFormat);
        }

        private static decimal RoundAmountWithCurrency(decimal amount, int currencyId)
        {
            if (currencyId == Constants.VietnamCurrencyId)
            {
                return Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000;
            }

            return Math.Round(amount, 2);
        }
    }
}