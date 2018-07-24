namespace HRP.Repositories.Extensions
{
    using Microsoft.Extensions.DependencyInjection;

    public static class RepositoryDIExtensions
    {
        public static void ResolveRepositoryDI(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IMonthlyPayrollRepository, MonthlyPayrollRepository>();
            services.AddScoped<IPaymentProcessRepository, PaymentProcessRepository>();
            services.AddScoped<IPayslipRepository, PayslipRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
        }
    }
}