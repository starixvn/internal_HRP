namespace HRP_Website.Controllers
{
    using System.Diagnostics;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Newtonsoft.Json.Linq;
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Models;    
    
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;

        public AccountController(
            IUserService userService,
            ITokenService tokenService,
            ISessionService sessionService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _sessionService = sessionService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
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

                    return Ok();
                }
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            _sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrWhiteSpace(_sessionService.Username);
        }
    }
}
