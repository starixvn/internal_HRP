namespace HRP.Website.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Google.Authenticator;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.UserPermisson;
    using HRP.Website.Models;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class HomeController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public HomeController(
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            string SecretKey = "1234aa";
            string UniqueKey = sessionService.Username + SecretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode("HRP Authenticator", sessionService.Username, UniqueKey, 200, 200);
            var model = new UserProfileViewModel
            {
                QRImageUrl = setupInfo.QrCodeSetupImageUrl,
                SetupCode = setupInfo.ManualEntryKey
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignPermission()
        {
            var model = new AssignPermissonViewModel
            {
                Functions = await baseService.GetDataAsync<FunctionModel>(new GetAllFunctionCriteria { IsActive = true }),
                Companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true }).ConfigureAwait(false),
                UserPermissions = await baseService.GetDataAsync<UserPermissionModel>(new GetFunctionByUserIdCriteria { UserId = sessionService.UserId })
            };

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}