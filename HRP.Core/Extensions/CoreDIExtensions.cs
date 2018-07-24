namespace HRP.Core.Extensions
{
    using Fanex.Data.Repository;
    using Microsoft.Extensions.DependencyInjection;
    using HRP.Core.Repositories;
    using HRP.Core.Services;

    public static class CoreDIExtensions
    {
        public static void ResolveCoreDI(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IBaseMailService, BaseMailService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<ICertificateService, CertificateService>();
        }
    }
}