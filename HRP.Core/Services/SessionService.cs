namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Enumerations;
    using HRP.Core.Constants;
    using Microsoft.AspNetCore.Http;
    using Model;

    public class SessionService : ISessionService
    {
        private readonly ISession _session;

        public SessionService(ISession session)
        {
            _session = session;
        }

        public SessionService(IHttpContextAccessor accessor)
        {
            _session = accessor.HttpContext.Session;
        }

        public int UserId
        {
            get => _session?.GetInt32(SessionKey.InternalUserId) ?? 0;
            set => _session?.SetInt32(SessionKey.InternalUserId, value);
        }

        public int OnUserId
        {
            get => _session?.GetInt32(SessionKey.InternalOnUserId) ?? 0;
            set => _session?.SetInt32(SessionKey.InternalOnUserId, value);
        }

        public string Username
        {
            get => _session.GetString(SessionKey.InternalUsername) ?? string.Empty;
            set => _session.SetString(SessionKey.InternalUsername, value);
        }

        public IEnumerable<MenuItem> MenuItems
        {
            get => _session.Get<List<MenuItem>>(SessionKey.InternalMenuItemList);
            set => _session.Set(SessionKey.InternalMenuItemList, value);
        }

        public IEnumerable<FunctionModel> Functions
        {
            get => _session.Get<List<FunctionModel>>(SessionKey.InternalFunctionList);
            set => _session.Set(SessionKey.InternalFunctionList, value);
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var functions = Functions?.Where(x => x.FunctionKey == functionKey);
            var result = false;

            if (functions != null && functions.Any())
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = functions.Max(x => x.IsRead) || functions.Max(x => x.IsHost);
                        break;

                    case Policy.Host:
                        result = functions.Max(x => x.IsHost);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public bool HasPermissionOnCompany(int companyId, string functionKey, Policy policy)
        {
            var functions = Functions?.Where(x => x.CompanyId == companyId && x.FunctionKey == functionKey);
            var result = false;

            if (functions != null && functions.Any())
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = functions.Max(x => x.IsRead) || functions.Max(x => x.IsHost);
                        break;

                    case Policy.Host:
                        result = functions.Max(x => x.IsHost);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public void ClearAll()
        {
            _session.Clear();
        }
    }
}