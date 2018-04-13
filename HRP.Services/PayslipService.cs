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

    public class PayslipService : IPayslipService
    {
        private const string EmailTemplate = @"wwwroot\\EmailTemplates\\Payslip.cshtml";
        private const string cssFilePath = @"assets\\bundles\\payslip\\css\\pdf.min.css";
        private const string OddAmountFormat = "#,##0.00";
        private const string AmountFormat = "#,##0";
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
                await RenderPdfAndSendMailAsync(payslip).ConfigureAwait(false);
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

        private async Task RenderPdfAndSendMailAsync(PayslipDetailModel payslip)
        {
            var body = string.Format(CultureInfo.InvariantCulture, emailService.ReadTextFile(EmailTemplate), payslip.Nickname);
            var subject = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}", payslip.Month.ToString("MMMM_yyyy"));
            var fileName = string.Format(CultureInfo.InvariantCulture, "Payslip_{0}.pdf", payslip.Month.ToString("MMMM_yyyy"));
            var htmlString = await viewRenderService.RenderViewToStringAsync("Payslip/Pdf", payslip).ConfigureAwait(false);
            byte[] content = pdfRenderService.RenderContentWithPasswordProtected(cssFilePath, htmlString, payslip.PDFPassword, payslip.PDFPassword);

            using (var input = new MemoryStream(content))
            {
                var mailAttachments = new List<MailAttachment>();
                var mailAttachment = new MailAttachment(input, FileExtentions.PDF)
                {
                    FileName = fileName
                };
                mailAttachments.Add(mailAttachment);

                var mailModel = new MailModel
                {
                    Subject = subject,
                    ReceiverEmail = payslip.Email,
                    Body = body,
                    Attachments = mailAttachments
                };

                emailService.SendEmail(mailModel);
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
            payslip.SalaryAmount = RoundAmountWithCurrency(basicSalaryAmount, payslip.SalaryCurrencyId, 0).ToString(OddAmountFormat);

            foreach (MonthlySalaryAdditionModel addition in payslip.SalaryAdditions)
            {
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                additionAmount += amount;
                addition.NetSalaryDisplay = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(OddAmountFormat);
            }

            foreach (MonthlySalaryDeductionModel deduction in payslip.SalaryDeductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deductionAmount += amount;
                deduction.NetSalaryDisplay = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId).ToString(OddAmountFormat);
            }

            amount = basicSalaryAmount + additionAmount;
            payslip.GrossSalaryAmount = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId, 0).ToString(AmountFormat);

            amount = basicSalaryAmount + additionAmount - deductionAmount;
            payslip.NetSalaryAmount = RoundAmountWithCurrency(amount, payslip.SalaryCurrencyId, 0).ToString(AmountFormat);

            payslip.SalaryDeductionAmount = RoundAmountWithCurrency(deductionAmount, payslip.SalaryCurrencyId, 0).ToString(AmountFormat);
        }

        private void DescryptAndCalculateMonthlyAllowance(PayslipDetailModel payslip)
        {
            decimal amount;
            decimal additionAmount = 0;
            decimal deductionAmount = 0;
            payslip.AllowanceAmount = cryptoService.Decrypt(payslip.AllowanceAmount);

            decimal.TryParse(payslip.AllowanceAmount, out decimal basicAllowanceAmount);
            payslip.AllowanceAmount = RoundAmountWithCurrency(basicAllowanceAmount, payslip.AllowanceCurrencyId, 0).ToString(OddAmountFormat);

            foreach (MonthlyAllowanceAdditionModel addition in payslip.AllowanceAdditions)
            {
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);

                decimal.TryParse(addition.NetAllowance, out amount);
                additionAmount += amount;
                addition.NetAllowanceToDisplay = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(OddAmountFormat);
            }

            foreach (MonthlyAllowanceDeductionModel deduction in payslip.AllowanceDeductions)
            {
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);

                decimal.TryParse(deduction.NetAllowance, out amount);
                deductionAmount += amount;
                deduction.NetAllowanceToDisplay = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId).ToString(OddAmountFormat);
            }

            amount = basicAllowanceAmount + additionAmount;
            payslip.GrossAllowanceAmount = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId, 0).ToString(AmountFormat);

            amount = basicAllowanceAmount + additionAmount - deductionAmount;
            payslip.NetAllowanceAmount = RoundAmountWithCurrency(amount, payslip.AllowanceCurrencyId, 0).ToString(AmountFormat);

            payslip.AllowanceDeductionAmount = RoundAmountWithCurrency(deductionAmount, payslip.AllowanceCurrencyId, 0).ToString(AmountFormat);
        }

        private static decimal RoundAmountWithCurrency(decimal amount, int currencyId, int decimals = 2)
        {
            if (currencyId == Constants.VietnamCurrencyId)
            {
                return Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000;
            }

            return Math.Round(amount, decimals, MidpointRounding.AwayFromZero);
        }
    }
}