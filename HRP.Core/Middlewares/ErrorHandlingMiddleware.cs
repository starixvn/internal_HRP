namespace HRP.Core
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using HRP.Core.CustomException;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }

        private void HandleException(HttpContext context, Exception exception)
        {
            var exceptionType = exception.GetType();

            string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    context.Request.Headers["Host"],
                    exception.TargetSite,
                    exception.StackTrace);
            logger.LogError(message);

            if (exceptionType == typeof(CertificateNotFoundException))
            {
                context.Response.Redirect("/certificate/FileNotFound");
            }
            else if (exceptionType == typeof(CertificatePasswordException))
            {
                context.Response.Redirect("/certificate/password");
            }
            else
            {
                context.Response.Redirect("/error/500");
            }
        }
    }
}