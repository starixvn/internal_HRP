namespace HRP.Core.Filters
{
    using System;
    using System.Net;
    using Enumerations;
    using Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;

    public class HRPPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private int _cacheDuration = 0;

        public string FunctionKey { get; set; }

        public Policy Policy { get; set; }

        public bool PermissionCache { get; set; }

        public int CacheDuration
        {
            get => _cacheDuration == 0 ? 20 : _cacheDuration;
            set => _cacheDuration = value;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                HRPAuthorizeAttribute.CloseConnection(context);
                return;
            }

            var currentSession = new SessionService(context.HttpContext.Session);

            if(currentSession.UserId <= 0 || !HasPermissionOnFunction(currentSession))
            {
                HRPAuthorizeAttribute.CloseConnection(context);
            }
        }

        private bool HasPermissionOnFunction(SessionService currentSession)
        {
            int policy = 0;

            if (PermissionCache)
            {
                var cacheKey = string.Format("{0}_{1}", currentSession.UserId, FunctionKey);
                var cachedPolicy = _cache.Get(cacheKey);

                if (cachedPolicy == null)
                {
                    policy = currentSession.GetUserPermissionOnFunction(currentSession.UserId, FunctionKey);
                    _cache.Set(cacheKey, policy, DateTime.Now.AddSeconds(CacheDuration));
                }
                else
                {
                    policy = (int) cachedPolicy;
                }
            }
            else
            {
                policy = currentSession.GetUserPermissionOnFunction(currentSession.UserId, FunctionKey);
            }

            return (int) policy < (int) Policy;
        }        
    }
}