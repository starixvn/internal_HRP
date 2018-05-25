namespace HRP.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Model;
    using Core.Services;
    using Fanex.Data;
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Services.Interfaces;

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
            ////string encryptedPassword = Cryptography.EncryptPasswordOfConnectionString("Data Source=10.18.200.114;Initial Catalog=bodb_HRP;User Id=internalteam;Password=@357$abc^FFZ%");
            Dictionary<string, string> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key, connection => connection.Value, StringComparer.OrdinalIgnoreCase);

            DbSettingProviderManager
                .StartNewSession()
                .UseConnectionStrings(connections)
                .UseDefaultDbSettingProvider("~/App_Data")
                .Run();

            services.Configure<AppSettingsOptions>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.Configure<MailConfigs>(options => Configuration.GetSection("MailConfigs").Bind(options));
            services.Configure<OtpConfig>(options => Configuration.GetSection(nameof(OtpConfig)).Bind(options));

            // Cookie Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["AppSettings:SessionTimeout"]));
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.Events.OnSigningOut = context =>
                    {
                        context.Response.Redirect(options.LoginPath);
                        return Task.CompletedTask;
                    };
                });

            // Cookie Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddMemoryCache();
            services.AddMvc(options =>
                {
                    ////#if !DEBUG
                    ////options.Filters.Add(new RequireHttpsAttribute());
                    ////#endif
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }
            ).AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".HRP.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration["AppSettings:SessionTimeout"]));
                options.Cookie.HttpOnly = true;
            });

            // DI
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IBaseMailService, BaseMailService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<HRPActionFilterAttribute>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IPdfRenderService, PdfRenderService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITwoFactorAuthenticatorService, TwoFactorAuthenticatorService>();
            services.AddScoped<IFunctionService, FunctionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IEmployeePayrollService, EmployeePayrollService>();
            services.AddScoped<IDataConfigurationService, DataConfigurationService>();
            services.AddScoped<IPaymentProcessService, PaymentProcessService>();
            services.AddScoped<IMonthlyPayrollService, MonthlyPayrollService>();
            services.AddScoped<ITemplateReader, TemplateReader>();
            services.AddScoped<IPayslipService, PayslipService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IExportExcelService, ExportExcelService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            if (!env.IsDevelopment())
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}