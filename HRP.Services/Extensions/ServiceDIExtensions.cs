namespace HRP.Services.Extensions
{
    using HRP.Repositories.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceDIExtensions
    {
        public static void ResolveServiceDI(this IServiceCollection services)
        {
            services.ResolveRepositoryDI();
            services.AddScoped<IObjectStream, ObjectStream>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IPdfRenderService, PdfRenderService>();
            services.AddScoped<IRijndaelService, RijndaelService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITwoFactorAuthenticatorService, TwoFactorAuthenticatorService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IEmployeePayrollService, EmployeePayrollService>();
            services.AddScoped<IDataConfigurationService, DataConfigurationService>();
            services.AddScoped<IPaymentProcessService, PaymentProcessService>();
            services.AddScoped<IMonthlyPayrollService, MonthlyPayrollService>();
            services.AddScoped<IMonthlySalaryService, MonthlySalaryService>();
            services.AddScoped<IMonthlyAllowancesService, MonthlyAllowancesService>();
            services.AddScoped<ITemplateReader, TemplateReader>();
            services.AddScoped<IPayslipService, PayslipService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IExportExcelService, ExportExcelService>();
            services.AddScoped<IMailSettingService, MailSettingService>();

            services.AddScoped<IExportMonthlyPayrollReportService, ExportMonthlyPayrollReportService>();
        }
    }
}