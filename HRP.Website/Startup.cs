namespace HRP.Website
{
    using System;
    using Core;
    using Core.Model;
    using DinkToPdf;
    using DinkToPdf.Contracts;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using HRP.Core.Filters;
    using HRP.Core.Repositories;
    using HRP.Core.Services;
    using HRP.Repositories;
    using HRP.Services;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettingsOptions>();
            services.AddSingleton(appSettings);

            var otpConfig = Configuration.GetSection("OtpConfig").Get<OtpConfig>();
            services.AddSingleton(otpConfig);

            services.AddFanexSql(Configuration);

            // Cookie Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.Name = ".HRP.Cookie";
                options.AccessDeniedPath = new PathString("/Error/405");
                options.LoginPath = new PathString("/Auth/Login");
                options.LogoutPath = new PathString("/Auth/Logout");
                options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["AppSettings:SessionTimeout"]));
                options.SlidingExpiration = true;
            });

            services.AddMemoryCache();
            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())).AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".HRP.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration["AppSettings:SessionTimeout"]));
                options.Cookie.HttpOnly = true;
            });

            // DI
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<HRPActionFilterAttribute>();
            services.AddScoped<ISessionService, SessionService>();

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IMonthlyPayrollRepository, MonthlyPayrollRepository>();
            services.AddSingleton<IPaymentProcessRepository, PaymentProcessRepository>();
            services.AddSingleton<IPayslipRepository, PayslipRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();

            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IBaseMailService, BaseMailService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<ICertificateService, CertificateService>();
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
            services.AddScoped<ITemplateReader, TemplateReader>();
            services.AddScoped<IAdditionDeductionTemplateReader, AdditionDeductionTemplateReader>();
            services.AddScoped<IPaymentProcessService, PaymentProcessService>();
            services.AddScoped<IMonthlyPayrollService, MonthlyPayrollService>();
            services.AddScoped<IMonthlySalaryService, MonthlySalaryService>();
            services.AddScoped<IMonthlyAllowancesService, MonthlyAllowancesService>();
            services.AddScoped<IPayslipService, PayslipService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IExportExcelService, ExportExcelService>();
            services.AddScoped<IMailSettingService, MailSettingService>();

            services.AddScoped<IExportMonthlyPayrollReportService, ExportMonthlyPayrollReportService>();
            services.AddScoped<IExportMiriBankAndCashReportService, ExportMiriBankAndCashReportService>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}