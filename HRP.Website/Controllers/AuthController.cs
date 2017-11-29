namespace HRP_Website.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Models;

    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(model.Username);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                    };

                    _sessionService.UserId = user.UserId;
                    _sessionService.Username = user.Username;
                    _sessionService.MenuItems = user.MenuItems;
                    _sessionService.Functions = user.Functions;

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Redirect("~/Home");;
                }
            }

            return View(model);
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            _sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
