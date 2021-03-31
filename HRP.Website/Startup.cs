namespace HRP.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Model;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Services.Extensions;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            //// string encryptedPassword = Cryptography.EncryptPassword("@357$abc^FFZ%");
            var connections = Configuration.GetSection("ConnectionStrings")
                         .GetChildren()
                         .AsEnumerable()
                         .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                         .ToDictionary(x => x.GetValue<string>("Name"),
                             y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithSqlServer()
               .UseDefaultDbSettingProvider("~/App_Data")
               .Run();

            services.Configure<AppSettingsOptions>(options => Configuration.GetSection("AppSettings").Bind(options));
            services.Configure<OtpConfig>(options => Configuration.GetSection(nameof(OtpConfig)).Bind(options));

            // Cookie Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".HRP.Cookie";
                    options.AccessDeniedPath = new PathString("/Error/405");
                    options.LoginPath = new PathString("/Auth/Login");
                    options.LogoutPath = new PathString("/Auth/Logout");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(Configuration["AppSettings:SessionTimeout"]));
                    options.SlidingExpiration = true;
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
            services.AddScoped<HRPActionFilterAttribute>();
            services.ResolveCoreDI();
            services.ResolveServiceDI();
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